using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Handlers;

namespace Telegrator.Extensions;

/// <summary>
/// Extension methods for starting continuous chat actions.
/// </summary>
public static class ContinuousActionExtensions
{
    /// <summary>
    /// Starts a continuous chat action in the chat associated with the current update.
    /// </summary>
    /// <param name="container">The handler container.</param>
    /// <param name="action">The chat action to repeat.</param>
    /// <param name="delay">The delay between repeated actions. Defaults to 4 seconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A disposable <see cref="ContinuousAction"/> instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the update has no associated chat.</exception>
    public static ContinuousAction StartContinuousAction(
        this IHandlerContainer container,
        ChatAction action,
        TimeSpan? delay = null,
        CancellationToken cancellationToken = default)
    {
        if (container is null)
            throw new ArgumentNullException(nameof(container));

        long? chatId = container.HandlingUpdate.GetChatId();
        if (chatId is null)
            throw new InvalidOperationException("Cannot start a continuous action for an update without a chat.");

        return new ContinuousAction(container.Client, chatId.Value, action, delay, cancellationToken);
    }

    /// <summary>
    /// Starts a continuous "typing" action in the current chat.
    /// </summary>
    public static ContinuousAction StartTypingAction(this IHandlerContainer container, TimeSpan? delay = null, CancellationToken cancellationToken = default)
        => container.StartContinuousAction(ChatAction.Typing, delay, cancellationToken);
}
