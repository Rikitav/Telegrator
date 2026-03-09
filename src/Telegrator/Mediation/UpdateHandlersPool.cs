using System.Threading.Channels;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;
using Telegrator.Logging;

namespace Telegrator.Mediation;

/// <summary>
/// Implementation of <see cref="IUpdateHandlersPool"/> that manages the execution of handlers.
/// Provides thread-safe queuing and execution of handlers with configurable concurrency limits.
/// </summary>
public class UpdateHandlersPool : IUpdateHandlersPool
{
    /// <summary>
    /// Synchronization object for thread-safe operations.
    /// </summary>
    protected readonly object SyncObj = new object();

    /// <summary>
    /// The task responsible for reading and processing handlers from the channel.
    /// </summary>
    protected readonly Task ChannelReaderTask;

    /// <summary>
    /// The channel used to queue handlers for execution.
    /// </summary>
    protected readonly Channel<DescribedHandlerDescriptor> ExecutionChannel;

    /// <summary>
    /// Semaphore for controlling the number of concurrently executing handlers.
    /// </summary>
    protected readonly SemaphoreSlim? ExecutionLimiter;

    /// <summary>
    /// The update router associated with this pool.
    /// </summary>
    protected readonly IUpdateRouter UpdateRouter;

    /// <summary>
    /// The bot configuration options.
    /// </summary>
    protected readonly TelegratorOptions Options;

    /// <summary>
    /// The global cancellation token for stopping all operations.
    /// </summary>
    protected readonly CancellationToken GlobalCancellationToken;

    /// <summary>
    /// Flag indicating whether the pool has been disposed.
    /// </summary>
    protected bool disposed = false;

    /// <inheritdoc/>
    public event HandlerEnqueued? HandlerEnqueued;

    /// <inheritdoc/>
    public event HandlerExecuting? HandlerExecuting;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateHandlersPool"/> class.
    /// </summary>
    /// <param name="router">The update handler that claims updates</param>
    /// <param name="options">The bot configuration options.</param>
    /// <param name="globalCancellationToken">The global cancellation token.</param>
    public UpdateHandlersPool(IUpdateRouter router, TelegratorOptions options, CancellationToken globalCancellationToken)
    {
        UpdateRouter = router;
        Options = options;
        GlobalCancellationToken = globalCancellationToken;
        
        ExecutionChannel = Channel.CreateUnbounded<DescribedHandlerDescriptor>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
            AllowSynchronousContinuations = false
        });

        if (options.MaximumParallelWorkingHandlers != null)
            ExecutionLimiter = new SemaphoreSlim(options.MaximumParallelWorkingHandlers.Value);

        GlobalCancellationToken.Register(() => ExecutionChannel.Writer.Complete());
        ChannelReaderTask = ReadChannel();
    }

    /// <inheritdoc/>
    public async Task Enqueue(params IEnumerable<DescribedHandlerDescriptor> handlers)
    {
        try
        {
            foreach (DescribedHandlerDescriptor handlerInfo in handlers)
            {
                if (handlerInfo.UpdateRouter != UpdateRouter)
                    throw new InvalidOperationException("Tried to enqueue update handler info from other router.");

                await ExecutionChannel.Writer.WriteAsync(handlerInfo, GlobalCancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _ = 0xDEADBEEF;
        }
    }

    private async Task ReadChannel()
    {
        try
        {
            await foreach (DescribedHandlerDescriptor handlerInfo in ExecutionChannel.Reader.ReadAllAsync(GlobalCancellationToken))
            {
                if (ExecutionLimiter != null)
                    await ExecutionLimiter.WaitAsync(GlobalCancellationToken);

                // Как только слот получен, "отстреливаем" задачу в ThreadPool 
                // и идем на следующий круг цикла за новым обработчиком из канала.
                _ = ProcessHandler(handlerInfo);
            }
        }
        catch (ChannelClosedException)
        {
            // TODO: add logging
        }
    }

    private async Task ProcessHandler(DescribedHandlerDescriptor handlerInfo)
    {
        try
        {
            TelegratorLogging.LogDebug("Described handler '{0}' (Update {1})", handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
            HandlerExecuting?.Invoke(handlerInfo);

            using UpdateHandlerBase instance = handlerInfo.HandlerInstance;
            Task<Result> task = instance.Execute(handlerInfo);
            HandlerEnqueued?.Invoke(handlerInfo);

            await task.ConfigureAwait(false);
            Result lastResult = task.Result;

            handlerInfo.ReportResult(lastResult);
            ExecutionLimiter?.Release(1);
        }
        catch (NotImplementedException)
        {
            _ = 0xBAD + 0xC0DE;
            handlerInfo.ReportResult(null);
        }
        catch (OperationCanceledException)
        {
            _ = 0xDEADBEEF;
            handlerInfo.ReportResult(null);
        }
        catch (Exception ex)
        {
            TelegratorLogging.LogError("Failed to process handler '{0}' (Update {1})", exception: ex, handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
            handlerInfo.ReportResult(null);
        }
    }

    /// <summary>
    /// Disposes of the handlers pool and releases all resources.
    /// </summary>
    public virtual void Dispose()
    {
        if (disposed)
            return;

        // do not dispose UpdateRouter
        ExecutionLimiter?.Dispose();
        
        GC.SuppressFinalize(this);
        disposed = true;
    }
}
