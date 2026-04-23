using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;
using Telegrator.Mediation;

namespace Telegrator.Polling;

/// <summary>
/// Service for receiving updates for Hosted telegram bots
/// </summary>
/// <param name="botClient"></param>
/// <param name="updateRouter"></param>
/// <param name="options"></param>
/// <param name="logger"></param>
public class HostedUpdateReceiver(ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
{
    private readonly ReceiverOptions _receiverOptions = options.Value;
    private readonly IUpdateRouter _updateRouter = updateRouter;

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting receiving updates via long-polling");
        _receiverOptions.AllowedUpdates = _updateRouter.HandlersProvider.AllowedTypes.ToArray();

        botClient.DeleteWebhook(options.Value.DropPendingUpdates).Wait();

        DefaultUpdateReceiver updateReceiver = new DefaultUpdateReceiver(botClient, _receiverOptions);
        await updateReceiver.ReceiveAsync(_updateRouter, stoppingToken).ConfigureAwait(false);
    }
}
