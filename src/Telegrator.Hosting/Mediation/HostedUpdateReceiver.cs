using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;
using Telegrator.Hosting;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted telegram botsand queuing them to router
/// </summary>
/// <param name="botClient"></param>
/// <param name="updateRouter"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public class HostedUpdateReceiver(ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
{
    private readonly ReceiverOptions _receiverOptions = new ReceiverOptions()
    {
        AllowedUpdates = options.Value.AllowedUpdates,
        DropPendingUpdates = options.Value.DropPendingUpdates,
        Limit = options.Value.Limit,
        Offset = options.Value.Offset
    };

    private readonly IUpdateRouter _updateRouter = updateRouter;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting receiving updates via long-polling");

        if (_updateRouter.BotInfo is HostedTelegramBotInfo hostedInfo)
            hostedInfo.User = await botClient.GetMe(stoppingToken).ConfigureAwait(false);

        _receiverOptions.AllowedUpdates = _updateRouter.HandlersProvider.AllowedTypes.ToArray();

        await botClient.DeleteWebhook(options.Value.DropPendingUpdates, cancellationToken: stoppingToken).ConfigureAwait(false);

        DefaultUpdateReceiver updateReceiver = new DefaultUpdateReceiver(botClient, _receiverOptions);
        await updateReceiver.ReceiveAsync(_updateRouter, stoppingToken).ConfigureAwait(false);
    }
}
