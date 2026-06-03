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
using Telegrator.Core;
using WUpdate = WTelegram.Types.Update;

namespace Telegrator.Mediation;

/// <summary>
/// Reactive implementation of <see cref="IUpdateReceiver"/> for polling updates from Telegram.
/// Provides custom update receiving logic with error handling and configuration options.
/// </summary>
/// <param name="client">The Telegram bot client for making API requests.</param>
// <param name="options">Optional receiver options for configuring update polling behavior.</param>
public class WideUpdateReceiver(WTelegramBotClient client) : IUpdateReceiver
{
    private readonly WTelegramBotClient _client = client;
    private IUpdateHandler? _updateHandler = null;
    private CancellationToken _cancellation = default;

    /// <inheritdoc/>
    public async Task ReceiveAsync(IUpdateHandler updateHandler, CancellationToken cancellationToken = default)
    {
        _updateHandler = updateHandler;
        _cancellation = cancellationToken;

        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        await using CancellationTokenRegistration registration = cancellationToken.Register(() => tcs.TrySetResult(null!));

        try
        {
            _client.OnUpdate += OnUpdate;
            await tcs.Task.ConfigureAwait(false);
        }
        finally
        {
            _client.OnUpdate -= OnUpdate;
        }
    }

    private async Task OnUpdate(WUpdate update)
    {
        if (_updateHandler == null)
            throw new Exception("Router not initialized (got null)");

        try
        {
            if (_updateHandler is IUpdateRouter router)
                await router.ConsumeUpdateAsync(_client, update, _cancellation).ConfigureAwait(false);
            else
                await _updateHandler.HandleUpdateAsync(_client, update, _cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _updateHandler.HandleErrorAsync(_client, ex, HandleErrorSource.HandleUpdateError, _cancellation).ConfigureAwait(false);
        }
    }
}
