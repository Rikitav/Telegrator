using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted wide telegram bots and queuing them to router
/// </summary>
/// <param name="logger"></param>
/// <param name="botClient"></param>
/// <param name="updateRouter"></param>
/// <param name="options"></param>
public class HostedWideBotUpdateReceiver(ILogger<HostedWideBotUpdateReceiver> logger, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions>? options) : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (botClient is not WTelegramBotClient wideBotClient)
            throw new Exception("Registered ITelegramBotClient was not a wide client (WTelegramBotClient)! Please, use `AddWideTelegrator` instead.");

        if (options?.Value.DropPendingUpdates is true)
            await wideBotClient.DropPendingUpdates();

        logger.LogInformation("Starting receiving updates via MTProto");
        
        // UIP (understanding in progress)
        //_receiverOptions.AllowedUpdates = updateRouter.HandlersProvider.AllowedTypes.ToArray();

        botClient.DeleteWebhook(options?.Value.DropPendingUpdates ?? false, cancellationToken: stoppingToken)
            .ConfigureAwait(false).GetAwaiter().GetResult();

        WideUpdateReceiver updateReceiver = new WideUpdateReceiver(wideBotClient);
        await updateReceiver.ReceiveAsync(updateRouter, stoppingToken).ConfigureAwait(false);
    }
}
