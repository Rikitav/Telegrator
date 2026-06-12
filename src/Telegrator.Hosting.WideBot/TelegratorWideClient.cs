/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;
using Telegrator.Logging;
using Telegrator.Mediation;
using Telegrator.Providers;
using Telegrator.States;

namespace Telegrator;

/// <summary>
/// Client class for the Telegrator library with Wider functionality, provided by WTelegramBotClient.
/// Extends TelegramBotClient with reactive capabilities for handling updates.
/// </summary>
public class TelegratorWClient : WTelegramBotClient, ITelegratorBot, ICollectingProvider
{
    private IUpdateRouter? _updateRouter = null;

    /// <inheritdoc/>
    public TelegratorOptions Options { get; }

    /// <inheritdoc/>
    public IHandlersCollection Handlers { get; }

    /// <inheritdoc/>
    public ITelegramBotInfo BotInfo { get; private set; } = null!;

    /// <inheritdoc/>
    public IUpdateRouter UpdateRouter => _updateRouter ?? throw new InvalidOperationException("Router's not created yet. Invoke `StartReceiving` to initialize this property.");

    /// <summary>
    /// Initializes new instance of <see cref="TelegratorWClient"/>
    /// </summary>
    /// <param name="wOptions"></param>
    /// <param name="telegratorOptions"></param>
    /// <param name="httpClient"></param>
    /// <param name="cancellationToken"></param>
    public TelegratorWClient(WTelegramBotClientOptions wOptions, TelegratorOptions? telegratorOptions = null, HttpClient? httpClient = null, CancellationToken cancellationToken = default)
        : base(wOptions, httpClient, cancellationToken)
    {
        Options = telegratorOptions ?? new TelegratorOptions();
        Handlers = new HandlersCollection(default);
    }

    /// <inheritdoc/>
    public void StartReceiving(ReceiverOptions? _, CancellationToken cancellationToken = default)
    {
        StartReceivingAsync(_, cancellationToken).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public async Task StartReceivingAsync(ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
    {
        if (Options.GlobalCancellationToken == CancellationToken.None)
            Options.GlobalCancellationToken = cancellationToken;

        BotInfo = new TelegramBotInfo(await GetMe(cancellationToken).ConfigureAwait(false));

        HandlersProvider handlerProvider = new HandlersProvider(Handlers, Options);
        AwaitingProvider awaitingProvider = new AwaitingProvider(Options);
        DefaultStateStorage stateStorage = new DefaultStateStorage();

        _updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, stateStorage, Options, BotInfo);
        TelegratorLogging.LogInformation($"TelegratorW bot starting up - BotId: {BotInfo.User.Id}, Username: {BotInfo.User.Username}");

        await StartReceivingInternal(Options.GlobalCancellationToken).ConfigureAwait(false);
    }

    private async Task StartReceivingInternal(CancellationToken cancellationToken)
    {
        try
        {
            try
            {
                await new HostedWideBotUpdateReceiver(NullLoggerFactory.Instance.CreateLogger<HostedWideBotUpdateReceiver>(), this, UpdateRouter, null)
                    .StartAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                await UpdateRouter
                    .HandleErrorAsync(this, exception, HandleErrorSource.FatalError, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Cancelled
            TelegratorLogging.LogInformation("Telegrator bot stopped (cancelled)");
        }
    }
}
