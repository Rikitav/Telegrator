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
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Logging;

namespace Telegrator;

/// <summary>
/// Represents a continuous chat action that runs in the background until cancelled or disposed.
/// </summary>
public class ContinuousAction : IDisposable
{
    private readonly ITelegramBotClient _client;
    private readonly ChatAction _action;
    private readonly ChatId _chat;
    private readonly TimeSpan _delay;

    private readonly CancellationTokenSource _linkedCts;
    private readonly Task _workerTask;

    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousAction"/> class.
    /// </summary>
    /// <param name="client">The Telegram bot client.</param>
    /// <param name="chat">The target chat.</param>
    /// <param name="action">The action to perform continuously.</param>
    /// <param name="delay">The delay between actions. Defaults to 4 seconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ContinuousAction(ITelegramBotClient client, ChatId chat, ChatAction action, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        _client = client;
        _chat = chat;
        _action = action;
        _delay = delay ?? TimeSpan.FromSeconds(4);

        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _workerTask = StartActorAsync(_linkedCts.Token);
    }

    private async Task StartActorAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await _client.SendChatAction(_chat, _action, cancellationToken: token).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    TelegratorLogging.LogTrace("Continuous action throwed an exception.\n{0}", ex);
                }

                await Task.Delay(_delay, token);
            }
        }
        catch (OperationCanceledException)
        {
            TelegratorLogging.LogTrace("Continuous action cancelled");
        }
    }

    /// <summary>
    /// Cancels the continuous action.
    /// </summary>
    public void Cancel()
    {
        if (Interlocked.CompareExchange(ref _disposed, 0, 0) == 0)
        {
            try
            {
                _linkedCts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                _ = 0xDEADBEEF;
            }
        }
    }

    /// <summary>
    /// Waits for the background worker task to complete.
    /// </summary>
    public async Task WaitAsync()
    {
        await _workerTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Disposes the instance and stops the continuous action.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
            return;

        try
        {
            _linkedCts.Cancel();
        }
        finally
        {
            _linkedCts.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
