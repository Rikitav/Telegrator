using Telegrator.Handlers.Components;
using Telegrator.Logging;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Polling
{
    /// <summary>
    /// Implementation of <see cref="IUpdateHandlersPool"/> that manages the execution of handlers.
    /// Provides thread-safe queuing and execution of handlers with configurable concurrency limits.
    /// </summary>
    public class UpdateHandlersPool : IUpdateHandlersPool
    {
        /// <summary>
        /// Synchronization object for thread-safe operations.
        /// </summary>
        protected object SyncObj = new object();

        /// <summary>
        /// Semaphore for controlling the number of concurrently executing handlers.
        /// </summary>
        protected SemaphoreSlim ExecutingHandlersSemaphore = null!;

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
        /// <param name="options">The bot configuration options.</param>
        /// <param name="globalCancellationToken">The global cancellation token.</param>
        public UpdateHandlersPool(TelegratorOptions options, CancellationToken globalCancellationToken)
        {
            Options = options;
            GlobalCancellationToken = globalCancellationToken;

            if (options.MaximumParallelWorkingHandlers != null)
            {
                ExecutingHandlersSemaphore = new SemaphoreSlim(options.MaximumParallelWorkingHandlers.Value);
            }
        }

        /// <inheritdoc/>
        public async Task Enqueue(IEnumerable<DescribedHandlerInfo> handlers)
        {
            if (ExecutingHandlersSemaphore != null)
            {
                await ExecutingHandlersSemaphore.WaitAsync().ConfigureAwait(false);
            }

            // Offload the entire processing of this update's handlers to a background task.
            // This allows the Receiver to continue polling for NEW updates immediately,
            // while this update acts as a self-contained unit of work.
            _ = Task.Run(async () =>
            {
                try
                {
                    Result? lastResult = null;
                    foreach (DescribedHandlerInfo handlerInfo in handlers)
                    {
                        if (lastResult?.NextType != null)
                        {
                            if (lastResult.NextType != handlerInfo.From.HandlerType)
                                continue;
                        }

                        try
                        {
                            Alligator.LogDebug("Described handler '{0}' (Update {1})", handlerInfo.DisplayString,
                                handlerInfo.HandlingUpdate.Id);
                            HandlerExecuting?.Invoke(handlerInfo);

                            using (UpdateHandlerBase instance = handlerInfo.HandlerInstance)
                            {
                                Task<Result> task = instance.Execute(handlerInfo);
                                HandlerEnqueued?.Invoke(handlerInfo);

                                await task.ConfigureAwait(false);
                                lastResult = task.Result;
                            }

                            if (lastResult.RouteNext)
                            {
                                Alligator.LogTrace("Handler '{0}' requested route continuation (Update {1})",
                                    handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
                            }
                        }
                        catch (NotImplementedException)
                        {
                            _ = 0xBAD + 0xC0DE;
                        }
                        catch (OperationCanceledException)
                        {
                            _ = 0xBAD + 0xC0DE;
                            break;
                        }
                        catch (Exception ex)
                        {
                            Alligator.LogError("Failed to process handler '{0}' (Update {1})", exception: ex,
                                handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
                        }

                        if (lastResult != null && !lastResult.RouteNext)
                            break;
                    }
                }
                finally
                {
                    ExecutingHandlersSemaphore?.Release(1);
                }
            }, GlobalCancellationToken);
        }

        /// <summary>
        /// Disposes of the handlers pool and releases all resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (disposed)
                return;

            if (ExecutingHandlersSemaphore != null)
            {
                ExecutingHandlersSemaphore.Dispose();
                ExecutingHandlersSemaphore = null!;
            }

            if (SyncObj != null)
                SyncObj = null!;

            GC.SuppressFinalize(this);
            disposed = true;
        }
    }
}
