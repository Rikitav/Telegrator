/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;

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
        logger.LogInformation("Hosted update receiver starting");
        logger.LogInformation("Receiving mode : LONG-POLLING");

        _receiverOptions.AllowedUpdates = _updateRouter.HandlersProvider.AllowedTypes.ToArray();

        await botClient.DeleteWebhook(options.Value.DropPendingUpdates, cancellationToken: stoppingToken).ConfigureAwait(false);

        DefaultUpdateReceiver updateReceiver = new DefaultUpdateReceiver(botClient, _receiverOptions);
        await updateReceiver.ReceiveAsync(_updateRouter, stoppingToken).ConfigureAwait(false);
    }
}
