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
using Telegrator.Core;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted wide telegram bots and queuing them to router
/// </summary>
/// <param name="logger"></param>
/// <param name="botClient"></param>
/// <param name="updateRouter"></param>
/// <param name="options"></param>
public class HostedWideBotUpdateReceiver(ILogger<HostedWideBotUpdateReceiver> logger, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<WideBotOptions>? options) : BackgroundService
{
    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (botClient is not WTelegramBotClient wideBotClient)
            throw new InvalidOperationException("Registered ITelegramBotClient was not a wide client (WTelegramBotClient)! Please, use `AddWideTelegrator` instead.");

        if (options?.Value.DropPendingUpdates is true)
            await wideBotClient.DropPendingUpdates();

        logger.LogInformation("Hosted update receiver starting");
        logger.LogInformation("Receiving mode : MTProto");

        await botClient.DeleteWebhook(options?.Value.DropPendingUpdates ?? false, cancellationToken: stoppingToken).ConfigureAwait(false);

        WideUpdateReceiver updateReceiver = new WideUpdateReceiver(wideBotClient);
        await updateReceiver.ReceiveAsync(updateRouter, stoppingToken).ConfigureAwait(false);
    }
}
