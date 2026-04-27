using Telegram.Bot.Polling;
using Telegrator.Core;

namespace Telegrator;

/// <summary>
/// Interface for reactive Telegram bot implementations.
/// Defines the core properties and capabilities of a reactive bot.
/// </summary>
public interface ITelegratorBot
{
    /// <summary>
    /// Gets the update router for handling incoming updates.
    /// </summary>
    public IUpdateRouter UpdateRouter { get; }

    /// <summary>
    /// Initializes the update router and begins polling for updates asynchronously.
    /// </summary>
    /// <param name="receiverOptions">Optional receiver options for configuring update polling.</param>
    /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
    /// <returns></returns>
    Task StartReceivingAsync(ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default);
}
