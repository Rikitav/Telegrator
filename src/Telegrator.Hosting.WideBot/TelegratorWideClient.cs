// Maybe later...

/*
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

public class TelegratorWClient : WTelegramBotClient, ITelegratorBot, ICollectingProvider
{
    private IUpdateRouter? _updateRouter = null;

    public TelegratorOptions Options { get; }
    
    public IHandlersCollection Handlers { get; }
    
    public ITelegramBotInfo BotInfo { get; }

    public IUpdateRouter UpdateRouter => _updateRouter ?? throw new InvalidOperationException("Router's not created yet. Invoke `StartReceiving` to initialize this property.");

    public TelegratorWClient(WTelegramBotClientOptions wOptions, TelegratorOptions? telegratorOptions = null, HttpClient? httpClient = null, CancellationToken cancellationToken = default)
        : base(wOptions, httpClient, cancellationToken)
    {
        Options = telegratorOptions ?? new TelegratorOptions();
        Handlers = new HandlersCollection(default);
        BotInfo = new TelegramBotInfo(GetMe(cancellationToken).Result);
    }

    public void StartReceiving(CancellationToken cancellationToken = default)
    {
        if (Options.GlobalCancellationToken == CancellationToken.None)
            Options.GlobalCancellationToken = cancellationToken;

        HandlersProvider handlerProvider = new HandlersProvider(Handlers, Options);
        AwaitingProvider awaitingProvider = new AwaitingProvider(Options);
        DefaultStateStorage stateStorage = new DefaultStateStorage();

        _updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, stateStorage, Options, BotInfo);
        TelegratorLogging.LogInformation($"TelegratorW bot starting up - BotId: {BotInfo.User.Id}, Username: {BotInfo.User.Username}");

        StartReceivingInternal(Options.GlobalCancellationToken);
    }

    private async void StartReceivingInternal(CancellationToken cancellationToken)
    {
        try
        {
            try
            {
                await new HostedWideBotUpdateReceiver(this)
                    .ReceiveAsync(UpdateRouter, cancellationToken)
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
*/