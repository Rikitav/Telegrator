using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers.Building;

/// <summary>
/// Internal handler used for awaiting specific update types.
/// Provides synchronization mechanism for waiting for updates of a particular type.
/// </summary>
/// <param name="handlingUpdateType">The type of update this awaiter handler waits for.</param>
internal class AwaiterHandler(UpdateType handlingUpdateType) : UpdateHandlerBase(handlingUpdateType), IHandlerContainerFactory, IDisposable
{
    /// <summary>
    /// Manual reset event used for synchronization.
    /// </summary>
    private ManualResetEventSlim ResetEvent = new ManualResetEventSlim(false);
    
    /// <summary>
    /// Gets the update that triggered this awaiter handler.
    /// </summary>
    public Update HandlingUpdate { get; private set; } = null!;

    /// <summary>
    /// Waits for the specified update type to be received.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    public void Wait(CancellationToken cancellationToken)
    {
        ResetEvent.Reset();
        ResetEvent.Wait(cancellationToken);
    }

    /// <summary>
    /// Creates a handler container for this awaiter handler.
    /// </summary>
    /// <param name="describedHandler">The handler information containing the update.</param>
    /// <returns>An empty handler container.</returns>
    public IHandlerContainer CreateContainer(DescribedHandlerDescriptor describedHandler)
    {
        HandlingUpdate = describedHandler.HandlingUpdate;
        return new EmptyHandlerContainer();
    }

    /// <summary>
    /// Executes the awaiter handler by setting the reset event.
    /// </summary>
    /// <param name="container">The handler container (unused).</param>
    /// <param name="cancellation">The cancellation token (unused).</param>
    /// <returns>A completed task.</returns>
    protected override Task<Result> ExecuteInternal(IHandlerContainer container, CancellationToken cancellation)
    {
        ResetEvent.Set();
        return Task.FromResult(Result.Ok());
    }

    /// <inheritdoc/>
    protected override bool Dispose(bool disposing)
    {
        if (!disposing)
            return true;

        ResetEvent.Dispose();
        ResetEvent = null!;
        return true;
    }
}
