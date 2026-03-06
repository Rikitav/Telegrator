using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.MadiatorCore;

namespace Telegrator.Polling
{
    /// <summary>
    /// Delegate used to handle <see cref="IUpdateRouter"/> exception
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    public delegate void RouterExceptionHandler(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken);

    /// <summary>
    /// Realizes <see cref="IRouterExceptionHandler"/> using function delegate
    /// </summary>
    /// <param name="handler"></param>
    public sealed class DefaultRouterExceptionHandler(RouterExceptionHandler handler) : IRouterExceptionHandler
    {
        private readonly RouterExceptionHandler _handler = handler;

        /// <inheritdoc/>
        public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            try
            {
                _handler.Invoke(botClient, exception, source, cancellationToken);
            }
            finally
            {
                _ = 0xBAD + 0xC0DE;
            }
        }}
}
