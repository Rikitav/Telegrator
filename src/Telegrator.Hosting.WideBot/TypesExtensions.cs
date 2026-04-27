using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.Handlers;
using Telegrator.Hosting;
using Telegrator.Mediation;
using Telegrator.Providers;

using TLUpdate = TL.Update;
using WUpdate = WTelegram.Types.Update;

namespace Telegrator;

public static class HandlersExtensions
{
    extension<TUpdate>(AbstractUpdateHandler<TUpdate> handler) where TUpdate : class
    {
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

        public TLUpdate? TLUpdate
        {
            get => handler.WideUpdate.TLUpdate;
        }
    }

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
        AddTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        action?.Invoke(new TelegramBotHostBuilder(builder, handlers));
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddWideTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        AddTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    internal static void AddTelegratorInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
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

        if (!services.Any(srvc => srvc.ImplementationType == typeof(IOptions<WTelegramBotClientOptions>)))
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
    public static IServiceCollection ConfigureWideTelegram(this IServiceCollection services, WTelegramBotClientOptions options)
    {
        services.AddSingleton(Options.Create(options));
        return services;
    }

    public static IServiceCollection AddMTProtoUpdateReceiver(this IServiceCollection services)
    {
        services.AddHttpClient<WTelegramBotClient>("tgmtproto").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        services.AddSingleton<ITelegramBotClient>(sp => sp.GetRequiredService<WTelegramBotClient>());
        services.AddHostedService<HostedWideBotUpdateReceiver>();
        return services;
    }

    private static WTelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
        => new WTelegramBotClient(provider.GetRequiredService<IOptions<WTelegramBotClientOptions>>().Value, httpClient);
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
