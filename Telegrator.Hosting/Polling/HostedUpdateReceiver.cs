﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Hosting.Components;
using Telegrator.MadiatorCore;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    /// <summary>
    /// Service for receiving updates for Hosted telegram bots
    /// </summary>
    /// <param name="botHost"></param>
    /// <param name="botClient"></param>
    /// <param name="updateRouter"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public class HostedUpdateReceiver(ITelegramBotHost botHost, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<ReceiverOptions> options, ILogger<HostedUpdateReceiver> logger) : BackgroundService
    {
        private readonly ReceiverOptions _receiverOptions = options.Value;
        private readonly IUpdateRouter _updateRouter = updateRouter;

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Starting receiving updates via long-polling");
            _receiverOptions.AllowedUpdates = botHost.UpdateRouter.HandlersProvider.AllowedTypes.ToArray();
            ReactiveUpdateReceiver updateReceiver = new ReactiveUpdateReceiver(botClient, _receiverOptions);
            await updateReceiver.ReceiveAsync(_updateRouter, stoppingToken).ConfigureAwait(false);
        }
    }
}
