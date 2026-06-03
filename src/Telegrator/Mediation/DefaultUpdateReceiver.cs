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

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegrator.Core;

namespace Telegrator.Mediation;

/// <summary>
/// Reactive implementation of <see cref="IUpdateReceiver"/> for polling updates from Telegram.
/// Provides custom update receiving logic with error handling and configuration options.
/// </summary>
/// <param name="client">The Telegram bot client for making API requests.</param>
/// <param name="options">Optional receiver options for configuring update polling behavior.</param>
public class DefaultUpdateReceiver(ITelegramBotClient client, ReceiverOptions? options) : IUpdateReceiver
{
    /// <summary>
    /// Gets the receiver options for configuring update polling behavior.
    /// </summary>
    public readonly ReceiverOptions? Options = options;

    /// <summary>
    /// Gets the Telegram bot client for making API requests.
    /// </summary>
    public readonly ITelegramBotClient Client = client;

    /// <summary>
    /// Receives updates from Telegram using long polling.
    /// Handles update processing, error handling, and cancellation.
    /// </summary>
    /// <param name="updateHandler">The update handler to process received updates.</param>
    /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
    /// <returns>A task representing the asynchronous update receiving operation.</returns>
    public async Task ReceiveAsync(IUpdateHandler updateHandler, CancellationToken cancellationToken)
    {
        using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cancellationToken = cts.Token;
        GetUpdatesRequest request = new GetUpdatesRequest()
        {
            AllowedUpdates = Options?.AllowedUpdates ?? [],
            Limit = Options?.Limit.GetValueOrDefault(100),
            Offset = Options?.Offset
        };

        if (Options?.DropPendingUpdates ?? false)
        {
            try
            {
                Update[] array = await Client.GetUpdates(-1, 1, 0, [], cancellationToken).ConfigureAwait(false);
                request.Offset = array.Length != 0 ? array[^1].Id + 1 : 0;
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                request.Timeout = (int)Client.Timeout.TotalSeconds;
                foreach (Update update in await Client.SendRequest(request, cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        request.Offset = update.Id + 1;
                        if (updateHandler is IUpdateRouter router)
                            await router.ConsumeUpdateAsync(Client, update, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                        else
                            await updateHandler.HandleUpdateAsync(Client, update, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                    }
                    catch (Exception exception2)
                    {
                        await updateHandler.HandleErrorAsync(Client, exception2, HandleErrorSource.HandleUpdateError, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception)
            {
                await updateHandler.HandleErrorAsync(Client, exception, HandleErrorSource.PollingError, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
