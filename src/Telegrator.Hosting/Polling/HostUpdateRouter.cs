using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator.Configuration;
using Telegrator.MadiatorCore;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    /// <inheritdoc/>
    public class HostUpdateRouter : UpdateRouter
    {
        /// <summary>
        /// <see cref="ILogger"/> of this router
        /// </summary>
        protected readonly ILogger<HostUpdateRouter> Logger;

        /// <inheritdoc/>
        public HostUpdateRouter(
            IHandlersProvider handlersProvider,
            IAwaitingProvider awaitingProvider,
            IOptions<TelegratorOptions> options,
            IUpdateHandlersPool handlersPool,
            ITelegramBotInfo botInfo,
            ILogger<HostUpdateRouter> logger) : base(handlersProvider, awaitingProvider, options.Value, handlersPool, botInfo)
        {
            Logger = logger;
            ExceptionHandler = new DefaultRouterExceptionHandler(HandleException);
        }

        /// <inheritdoc/>
        public override Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //Logger.LogInformation("Received update of type \"{type}\"", update.Type);
            return base.HandleUpdateAsync(botClient, update, cancellationToken);
        }

        /// <summary>
        /// Default exception handler of this router
        /// </summary>
        /// <param name="botClient"></param>
        /// <param name="exception"></param>
        /// <param name="source"></param>
        /// <param name="cancellationToken"></param>
        public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            if (exception is HandlerFaultedException handlerFaultedException)
            {
                Logger.LogError("\"{handler}\" handler's execution was faulted :\n{exception}",
                    handlerFaultedException.HandlerInfo.ToString(),
                    handlerFaultedException.InnerException?.ToString() ?? "No inner exception");
                return;
            }

            Logger.LogError("Exception was thrown during update routing faulted :\n{exception}", exception.ToString());
        }
    }
}
