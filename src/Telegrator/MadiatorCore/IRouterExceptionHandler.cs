using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Interface for handling exceptions that occur during update routing operations.
    /// Provides a centralized way to handle and log errors that occur during bot operation.
    /// </summary>
    public interface IRouterExceptionHandler
    {
        /// <summary>
        /// Handles exceptions that occur during update routing.
        /// </summary>
        /// <param name="botClient">The <see cref="ITelegramBotClient"/> instance.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="source">The <see cref="HandleErrorSource"/> indicating the source of the error.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken);
    }
}
