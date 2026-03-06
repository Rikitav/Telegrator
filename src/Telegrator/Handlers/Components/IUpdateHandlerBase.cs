using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Diagnostics;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Components;

/// <summary>
/// Abstraction for update handlers, providing execution and lifetime management for Telegram updates.
/// </summary>
public interface IUpdateHandlerBase : IDisposable
{
    /// <summary>
    /// Gets the <see cref="UpdateType"/> that this handler processes.
    /// </summary>
    UpdateType HandlingUpdateType { get; }

    /// <summary>
    /// Gets the <see cref="HandlerLifetimeToken"/> associated with this handler instance.
    /// </summary>
    HandlerLifetimeToken LifetimeToken { get; }

    /// <summary>
    /// Executes the handler logic and marks the lifetime as ended after execution.
    /// </summary>
    /// <param name="described"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result> Execute(DescribedHandlerDescriptor described, CancellationToken cancellationToken = default);

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
    Task<Result> FiltersFallback(FiltersFallbackReport report, ITelegramBotClient client, CancellationToken cancellationToken = default);
}