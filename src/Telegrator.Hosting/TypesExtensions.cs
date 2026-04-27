using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.States;
using Telegrator.Hosting;
using Telegrator.Logging;
using Telegrator.Mediation;
using Telegrator.Providers;
using Telegrator.States;

namespace Telegrator;

/// <summary>
/// Provides extension methods for <see cref="IHostApplicationBuilder"/> to configure Telegrator.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// The key used to store the <see cref="IHandlersCollection"/> in the builder properties.
    /// </summary>
    public const string HandlersCollectionPropertyKey = nameof(IHandlersCollection);

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
    {
        AddTelegratorInternal(builder.Services, builder.Configuration, ((IHostApplicationBuilder)builder).Properties, ref handlers, options);
        action?.Invoke(new TelegramBotHostBuilder(builder, handlers));
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        AddTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostBuilder AddTelegrator(this IHostBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        builder.ConfigureServices((ctx, sp) => AddTelegratorInternal(sp, ctx.Configuration, builder.Properties, ref handlers, options));
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostBuilder AddTelegrator(this IHostBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<IHandlersCollection>? action = null)
    {
        builder.ConfigureServices((ctx, sp) => AddTelegratorInternal(sp, ctx.Configuration, builder.Properties, ref handlers, options));
        action?.Invoke(handlers!); // AddTelegratorInternal initializes `handlers`
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    internal static void AddTelegratorInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
    {
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
        properties.Add(HandlersCollectionPropertyKey, handlers);

        if (!services.Any(srvc => srvc.ServiceType == typeof(IOptions<ReceiverOptions>)))
        {
            ReceiverOptions? receiverOptions = configuration.GetSection(nameof(ReceiverOptions)).Get<ReceiverOptions>();
            if (receiverOptions == null)
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'ReceiverOptions' was registered. This configuration is runtime required!");

            services.AddSingleton(Options.Create(receiverOptions));
        }

        if (!services.Any(srvc => srvc.ImplementationType == typeof(IOptions<TelegramBotClientOptions>)))
        {
            services.AddSingleton(Options.Create(new TelegramBotClientOptions(options.Token, options.BaseUrl, options.UseTestEnvironment)
            {
                RetryCount = options.RetryCount,
                RetryThreshold = options.RetryThreshold
            }));
        }

        services.AddTelegramBotHostDefaults();
        services.AddTelegramReceiver();
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure Telegram Bot Host
/// </summary>
public static class HostServicesCollectionExtensions
{
    /// <summary>
    /// Adds TelegramBotClientOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureTelegram(this IServiceCollection services, TelegramBotClientOptions options)
    {
        services.RemoveAll<IOptions<TelegramBotClientOptions>>();
        services.AddSingleton(Options.Create(options));
        return services;
    }

    /// <summary>
    /// Adds ReceiverOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureReceiver(this IServiceCollection services, ReceiverOptions options)
    {
        services.RemoveAll<IOptions<ReceiverOptions>>();
        services.AddSingleton(Options.Create(options));
        return services;
    }

    /// <summary>
    /// Registers <see cref="IStateStorage"/> service
    /// </summary>
    /// <typeparam name="TStorage"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddStateStorage<TStorage>(this IServiceCollection services) where TStorage : IStateStorage
    {
        services.Replace(new ServiceDescriptor(typeof(IStateStorage), typeof(TStorage), ServiceLifetime.Singleton));
        return services;
    }

    /// <summary>
    /// Registers <see cref="Telegrator"/> default services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramBotHostDefaults(this IServiceCollection services)
    {
        services.AddSingleton<IAwaitingProvider, HostAwaitingProvider>();
        services.AddSingleton<IHandlersProvider, HostHandlersProvider>();
        services.AddSingleton<IUpdateRouter, HostUpdateRouter>();
        services.AddSingleton<ITelegramBotInfo, HostedTelegramBotInfo>();
        services.AddSingleton<IStateStorage, DefaultStateStorage>();

        return services;
    }

    /// <summary>
    /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateReceiver"/> to receive updates using long polling
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramReceiver(this IServiceCollection services)
    {
        services.RemoveAll<ITelegramBotClient>();
        services.RemoveAll<HostedUpdateReceiver>();

        services.AddHttpClient<ITelegramBotClient>("tgreceiver").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        services.AddHostedService<HostedUpdateReceiver>();
        return services;
    }

    /// <summary>
    /// <see cref="ITelegramBotClient"/> factory method
    /// </summary>
    /// <param name="httpClient"></param>
    /// <param name="provider"></param>
    /// <returns></returns>
    private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
        => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
}

/// <summary>
/// Provides useful methods to adjust Telegram bot Host
/// </summary>
public static class TelegramBotHostExtensions
{
    /// <summary>
    /// Replaces the initialization logic from TelegramBotWebHost constructor.
    /// Initializes the bot and logs handlers on application startup.
    /// </summary>
    public static IHost UseTelegrator(this IHost botHost)
    {
        ITelegramBotInfo info = botHost.Services.GetRequiredService<ITelegramBotInfo>();
        IHandlersCollection handlers = botHost.Services.GetRequiredService<IHandlersCollection>();
        ILoggerFactory loggerFactory = botHost.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
        {
            logger.LogInformation("Telegrator Bot Host started (Generic Host)");
            logger.LogInformation("Receiving mode : LONG-POLLING");
            logger.LogInformation("Telegram Bot : {firstname}, @{usrname}, id:{id},", info.User.FirstName ?? "[NULL]", info.User.Username ?? "[NULL]", info.User.Id);
            logger.LogHandlers(handlers);
        }

        botHost.AddLoggingAdapter();
        botHost.SetBotCommands();
        return botHost;
    }

    /// <summary>
    /// Configures bots available commands depending on what handlers was registered
    /// </summary>
    /// <param name="botHost"></param>
    /// <returns></returns>
    public static IHost SetBotCommands(this IHost botHost)
    {
        ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
        IUpdateRouter router = botHost.Services.GetRequiredService<IUpdateRouter>();

        IEnumerable<BotCommand> aliases = router.HandlersProvider.GetBotCommands();
        if (aliases.Any())
        {
            client.SetMyCommands(aliases)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        return botHost;
    }

    /// <summary>
    /// Adds a Microsoft.Extensions.Logging adapter to Alligator using a logger factory.
    /// </summary>
    /// <param name="host"></param>
    public static IHost AddLoggingAdapter(this IHost host)
    {
        ILoggerFactory loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Telegrator");

        MicrosoftLoggingAdapter adapter = new MicrosoftLoggingAdapter(logger);
        TelegratorLogging.AddAdapter(adapter);
        return host;
    }
}

/// <summary>
/// Provides extension methods for logging Telegrator-related information.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs the registered handlers to the specified logger.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    /// <param name="handlers">The collection of handlers to log.</param>
    public static void LogHandlers(this ILogger logger, IHandlersCollection handlers)
    {
        if (!logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            return;

        StringBuilder logBuilder = new StringBuilder("Registered handlers : ");
        if (!handlers.Keys.Any())
            throw new Exception("No update types were registered");

        foreach (UpdateType updateType in handlers.Keys)
        {
            HandlerDescriptorList descriptors = handlers[updateType];
            logBuilder.Append("\n\tUpdateType." + updateType + " :");

            foreach (HandlerDescriptor descriptor in descriptors.Reverse())
            {
                logBuilder.AppendFormat("\n\t* {0} - {1}",
                    descriptor.Indexer.ToString(),
                    descriptor.ToString());
            }
        }

        logger.LogInformation(logBuilder.ToString());
    }
}
