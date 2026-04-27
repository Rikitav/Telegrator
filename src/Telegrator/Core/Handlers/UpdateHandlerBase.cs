using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Handlers.Diagnostics;

namespace Telegrator.Core.Handlers;

/// <summary>
/// Base class for update handlers, providing execution and lifetime management for Telegram updates.
/// </summary>
public abstract class UpdateHandlerBase(UpdateType handlingUpdateType) : IUpdateHandlerBase
{
    /// <summary>
    /// Gets the <see cref="UpdateType"/> that this handler processes.
    /// </summary>
    public UpdateType HandlingUpdateType { get; } = handlingUpdateType;

    /// <summary>
    /// Gets the <see cref="HandlerLifetimeToken"/> associated with this handler instance.
    /// </summary>
    public HandlerLifetimeToken LifetimeToken { get; } = new HandlerLifetimeToken();

    /// <inheritdoc cref="Result.Ok"/>
    public static Result Ok => Result.Ok();

    /// <inheritdoc cref="Result.Fault"/>
    public static Result Fault => Result.Fault();

    /// <inheritdoc cref="Result.Next"/>
    public static Result Next => Result.Next();

    /// <summary>
    /// Executes the handler logic and marks the lifetime as ended after execution.
    /// </summary>
    /// <param name="described"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task<Result> Execute(DescribedHandlerDescriptor described, CancellationToken cancellationToken = default)
    {
        if (LifetimeToken.IsEnded)
            throw new Exception();

        try
        {
            // Creating container
            IHandlerContainer container = GetContainer(described);
            DescriptorAspectsSet? aspects = described.From.Aspects;

            // Executing pre processor
            if (aspects != null)
            {
                try
                {
                    Result? preResult = await aspects
                        .ExecutePre(this, container, cancellationToken)
                        .ConfigureAwait(false);

                    if (!preResult.InterruptRouter)
                        return preResult;
                }
                catch (NotImplementedException)
                {
                    _ = 0xBAD + 0xC0DE;
                }
            }

            try
            {
                // Executing handler
                Result execResult = await ExecuteInternal(container, cancellationToken).ConfigureAwait(false);
                if (!execResult.InterruptRouter)
                    return execResult;
            }
            catch (NotImplementedException)
            {
                _ = 0xBAD + 0xC0DE;
            }

            try
            {
                // Executing post processor
                if (aspects != null)
                {
                    Result postResult = await aspects
                        .ExecutePost(this, container, cancellationToken)
                        .ConfigureAwait(false);

                    if (!postResult.InterruptRouter)
                        return postResult;
                }
            }
            catch (NotImplementedException)
            {
                _ = 0xBAD + 0xC0DE;
            }

            // Success
            return Result.Ok();
        }
        catch (OperationCanceledException)
        {
            // Cancelled
            _ = 0xBAD + 0xC0DE;
            return Result.Ok();
        }
        catch (Exception exception)
        {
            try
            {
                await described.UpdateRouter
                    .HandleErrorAsync(described.Client, exception, HandleErrorSource.HandleUpdateError, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (NotImplementedException)
            {
                _ = 0xBAD + 0xC0DE;
            }

            return Result.Fault();
        }
        finally
        {
            LifetimeToken.LifetimeEnded();
        }
    }

    private IHandlerContainer GetContainer(DescribedHandlerDescriptor handlerInfo)
    {
        if (this is IHandlerContainerFactory handlerDefainedContainerFactory)
            return handlerDefainedContainerFactory.CreateContainer(handlerInfo);

        if (handlerInfo.UpdateRouter.DefaultContainerFactory is not null)
            return handlerInfo.UpdateRouter.DefaultContainerFactory.CreateContainer(handlerInfo);

        throw new Exception();
    }

    /// <summary>
    /// Executes the handler logic for the given container and cancellation token.
    /// </summary>
    /// <param name="container">The <see cref="IHandlerContainer"/> for the update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task<Result> ExecuteInternal(IHandlerContainer container, CancellationToken cancellationToken);

    /// <summary>
    /// Dispose resources of this handler. Override if needed
    /// </summary>
    /// <param name="disposing"></param>
    /// <returns>Return <see langword="true"/> if dispose was successfull and garbage collecting for this object can be supressed</returns>
    protected virtual bool Dispose(bool disposing)
    {
        return false;
    }

    /// <summary>
    /// Handles failed filters during handler describing.
    /// Use <see cref="Result"/> to control how router should treat this fail.
    /// <see cref="Result.Next"/> to silently continue decribing.
    /// <see cref="Result.Fault"/> to stop\break desribing sequence.
    /// </summary>
    /// <param name="report"></param>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual Task<Result> FiltersFallback(FiltersFallbackReport report, ITelegramBotClient client, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Ok());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (LifetimeToken.IsEnded)
            return;

        if (Dispose(true))
            GC.SuppressFinalize(this);
    }
}
