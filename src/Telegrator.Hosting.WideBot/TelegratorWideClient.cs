using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Net.Http;
using System.Threading;
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
    public ITelegramBotInfo BotInfo { get; }

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
        BotInfo = new TelegramBotInfo(GetMe(cancellationToken).Result);
    }

    /// <inheritdoc/>
    public void StartReceiving(ReceiverOptions? _, CancellationToken cancellationToken = default)
    {
        if (Options.GlobalCancellationToken == CancellationToken.None)
            Options.GlobalCancellationToken = cancellationToken;

        HandlersProvider handlerProvider = new HandlersProvider(Handlers, Options);
        AwaitingProvider awaitingProvider = new AwaitingProvider(Options);
        DefaultStateStorage stateStorage = new DefaultStateStorage();

        _updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, stateStorage, Options, BotInfo);
        TelegratorLogging.LogInformation($"TelegratorW bot starting up - BotId: {BotInfo.User.Id}, Username: {BotInfo.User.Username}");

        StartReceivingInternal(Options.GlobalCancellationToken)
            .ConfigureAwait(false).GetAwaiter().GetResult();
    }

    /// <inheritdoc/>
    public async Task StartReceivingAsync(ReceiverOptions? receiverOptions = null,CancellationToken cancellationToken = default)
    {
        if (Options.GlobalCancellationToken == CancellationToken.None)
            Options.GlobalCancellationToken = cancellationToken;

        HandlersProvider handlerProvider = new HandlersProvider(Handlers, Options);
        AwaitingProvider awaitingProvider = new AwaitingProvider(Options);
        DefaultStateStorage stateStorage = new DefaultStateStorage();

        _updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, stateStorage, Options, BotInfo);
        TelegratorLogging.LogInformation($"TelegratorW bot starting up - BotId: {BotInfo.User.Id}, Username: {BotInfo.User.Username}");

        await StartReceivingInternal(Options.GlobalCancellationToken);
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