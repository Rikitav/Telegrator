using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.Handlers;
using Telegrator.Hosting;
using Telegrator.Mediation;
using Telegrator.Providers;

using WUpdate = WTelegram.Types.Update;
using TLUpdate = TL.Update;

namespace Telegrator;

/// <summary>
/// Provides extensions memebrs for <see cref="UpdateHandlerBase"/> for easy access to Wider bot functions and update
/// </summary>
public static class HandlersExtensions
{
    extension<TUpdate>(AbstractUpdateHandler<TUpdate> handler) where TUpdate : class
    {
        /// <summary>
        /// Casts Update to <see cref="WTelegramBotClient"/>
        /// </summary>
        public WTelegramBotClient WClient
        {
            get
            {
                object? client = handler.GetType().GetField("Client")?.GetValue(handler);
                if (client is not WTelegramBotClient wideClient)
                    throw new InvalidCastException("Client is not assignable to `WTelegram.Bot.WTelegramBotClient`");

                return wideClient;
            }

        }

        /// <summary>
        /// Casts Update to <see cref="WUpdate"/>
        /// </summary>
        public WUpdate WideUpdate
        {
            get
            {
                object? update = handler.GetType().GetField("HandlingUpdate")?.GetValue(handler);
                if (update is not WUpdate wUpdate)
                    throw new InvalidCastException("Update is not assignable to `WTelegram.Types.Update`");

                return wUpdate;
            }
        }

        /// <summary>
        /// Casts Update to <see cref="TLUpdate"/>
        /// </summary>
        public TLUpdate? TLUpdate
        {
            get => handler.WideUpdate.TLUpdate;
        }
    }

    /// <summary>
    /// Casts Update to <see cref="WTelegramBotClient"/>
    /// </summary>
    public static WTelegramBotClient AsWClient(this ITelegramBotClient client)
    {
        return client as WTelegramBotClient
            ?? throw new InvalidCastException("Client is not assignable to `WTelegram.Bot.WTelegramBotClient`");
    }

    /// <summary>
    /// Casts Update to <see cref="WUpdate"/>
    /// </summary>
    public static WUpdate AsWUpdate(this Update update)
    {
        return update as WUpdate
            ?? throw new InvalidCastException("Update is not assignable to `WTelegram.Types.Update`");
    }
}

/// <summary>
/// Provides extension methods for <see cref="IHostApplicationBuilder"/> to configure Telegrator.
/// </summary>
public static class WideHostBuilderExtensions
{
    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddWideTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
    {
        AddWideTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        action?.Invoke(new TelegramBotHostBuilder(builder, handlers));
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddWideTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        AddWideTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    internal static void AddWideTelegratorInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
    {
        if (services.Any(srvc => srvc.ServiceType == typeof(HostedUpdateReceiver)))
            throw new InvalidOperationException("`HostedUpdateReceiver` found in services. WideHost extension is not compatible with long-polling receiving. Please, remove `AddTelegrator` invocation from your WebApp configuration.");

        if (options == null)
        {
            options = configuration.GetSection(nameof(TelegratorOptions)).Get<TelegratorOptions>();
            if (options == null)
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegratorOptions' was registered. This configuration is runtime required!");
        }

        CancellationTokenSource globallCancell = new CancellationTokenSource();
        options.GlobalCancellationToken = globallCancell.Token;
        services.AddSingleton(Options.Create(options));
        services.AddKeyedSingleton("cancell", globallCancell);

        if (handlers != null)
        {
            if (handlers is IHandlersManager manager)
            {
                ServiceDescriptor descriptor = new ServiceDescriptor(typeof(IHandlersProvider), manager);
                services.Replace(descriptor);
                services.AddSingleton(manager);
            }
        }

        handlers ??= new HostHandlersCollection(services, options);
        services.AddSingleton(handlers);
        properties.Add(HostBuilderExtensions.HandlersCollectionPropertyKey, handlers);

        if (!services.Any(srvc => srvc.ServiceType == typeof(IOptions<WTelegramBotClientOptions>)))
        {
            // For now, there's no way to configure this from IConfiguration, use `ConfigureWideTelegram` instead
            throw new MissingMemberException("No options of type 'WTelegramBotClientOptions' was registered. This configuration is runtime required! Use `ConfigureWideTelegram` to register options.");

            /*
            services.AddSingleton(Options.Create(new WTelegramBotClientOptions(options.Token, options.BaseUrl, options.UseTestEnvironment)
            {
                RetryCount = options.RetryCount,
                RetryThreshold = options.RetryThreshold
            }));
            */
        }

        services.AddTelegramBotHostDefaults();
        services.AddMTProtoUpdateReceiver();
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure Telegram Bot WebHost
/// </summary>
public static class WideBotServiceCollectionExtensions
{
    /// <summary>
    /// Adds WTelegramBotClientOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureWideTelegram(this IServiceCollection services, WTelegramBotClientOptions options)
    {
        services.RemoveAll<IOptions<WTelegramBotClientOptions>>();
        services.AddSingleton(Options.Create(options));
        return services;
    }

    /// <summary>
    /// Adds WTelegramBotClient 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="useHttp"></param>
    /// <returns></returns>
    public static IServiceCollection AddMTProtoUpdateReceiver(this IServiceCollection services, bool useHttp = false)
    {
        services.RemoveAll<ITelegramBotClient>();
        services.RemoveAll<HostedWideBotUpdateReceiver>();

        if (useHttp)
        {
            services.AddHttpClient<WTelegramBotClient>("tgmtproto").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        }
        else
        {
            services.AddSingleton(TypedTelegramBotClientFactory);
        }

        services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<WTelegramBotClient>());
        services.AddHostedService<HostedWideBotUpdateReceiver>();
        return services;
    }

#pragma warning disable CA2254
    private static WTelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
    {
        ILogger<WTelegramBotClient> logger = provider.GetRequiredService<ILogger<WTelegramBotClient>>();
        WTelegramBotClient client = new WTelegramBotClient(provider.GetRequiredService<IOptions<WTelegramBotClientOptions>>().Value, httpClient);

        WTelegram.Helpers.Log = (lvl, str) => logger.Log((LogLevel)lvl, str);
        return client;
    }

    private static WTelegramBotClient TypedTelegramBotClientFactory(IServiceProvider provider)
    {
        ILogger<WTelegramBotClient> logger = provider.GetRequiredService<ILogger<WTelegramBotClient>>();
        WTelegramBotClient client = new WTelegramBotClient(provider.GetRequiredService<IOptions<WTelegramBotClientOptions>>().Value);

        WTelegram.Helpers.Log = (lvl, str) => logger.Log((LogLevel)lvl, str);
        return client;
    }
#pragma warning restore CA2254
}

/// <summary>
/// Provides useful methods to adjust Telegram bot Host
/// </summary>
public static class WideTelegramBotHostExtensions
{
    /// <summary>
    /// Replaces the initialization logic from TelegramBotWebHost constructor. 
    /// Initializes the bot and logs handlers on application startup.
    /// </summary>
    public static IHost UseWideTelegrator(this IHost botHost)
    {
        if (!botHost.Services.TryFindWTelegramBotClient())
            throw new InvalidOperationException("No service for type 'Telegram.Bot.WTelegramBotClient' has been registered. Invoke `AddWideTelegrator`");

        ITelegramBotInfo info = botHost.Services.GetRequiredService<ITelegramBotInfo>();
        IHandlersCollection handlers = botHost.Services.GetRequiredService<IHandlersCollection>();
        ILoggerFactory loggerFactory = botHost.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Telegrator WIDE Bot Host started (Generic Host)");
            logger.LogInformation("Receiving mode : MTProto");
            logger.LogInformation("Telegram Bot : {firstname}, @{usrname}, id:{id},", info.User.FirstName ?? "[NULL]", info.User.Username ?? "[NULL]", info.User.Id);
            logger.LogHandlers(handlers);
        }

        botHost.AddLoggingAdapter();
        botHost.SetBotCommands();
        return botHost;
    }

    private static bool TryFindWTelegramBotClient(this IServiceProvider services)
    {
        return services.GetServices<IHostedService>().Any(s => s is HostedWideBotUpdateReceiver);
    }
}
