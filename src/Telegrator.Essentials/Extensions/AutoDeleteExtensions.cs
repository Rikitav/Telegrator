using Telegram.Bot.Types;
using Telegrator.Core.Handlers;
using Telegrator.Essentials.Aspects;

namespace Telegrator.Essentials.Extensions;

/// <summary>
/// Extension methods for scheduling automatic message deletion.
/// </summary>
public static class AutoDeleteExtensions
{
    /// <summary>
    /// Schedules the given message for automatic deletion by <see cref="AutoDeletePostProcessor"/>.
    /// Requires the handler to be decorated with <see cref="AfterExecutionAttribute{T}"/>.
    /// </summary>
    /// <param name="container">The handler container.</param>
    /// <param name="message">The message to delete.</param>
    /// <param name="delay">The delay before deletion. Defaults to 5 seconds.</param>
    public static void ScheduleAutoDelete(this IHandlerContainer container, Message message, TimeSpan? delay = null)
    {
        if (container is null)
            throw new ArgumentNullException(nameof(container));

        if (message is null)
            throw new ArgumentNullException(nameof(message));

        container.ExtraData[AutoDeletePostProcessor.MessageKey] = message;
        container.ExtraData[AutoDeletePostProcessor.DelayKey] = delay ?? TimeSpan.FromSeconds(5);
    }
}
