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
    /// Registers Telegrator and configures it to receive updates via long-polling.
    /// </summary>
    public static ITelegramBotHostBuilder WithPolling(this ITelegramBotHostBuilder builder)
    {
        if (!builder.Services.Any(srvc => srvc.ServiceType == typeof(IOptions<ReceiverOptions>)))
        {
            IConfigurationSection section = builder.Configuration.GetSection("Receiver");
            if (!section.Exists())
                section = builder.Configuration.GetSection(nameof(ReceiverOptions));

            if (!section.Exists())
                throw new MissingMemberException("Auto configuration enabled, yet no options of type 'ReceiverOptions' was registered. This configuration is runtime required!");

            ReceiverOptions receiverOptions = ParseReceiverOptions(section);
            builder.Services.AddSingleton(Options.Create(receiverOptions));
        }

        builder.Services.AddTelegramReceiver();
        return builder;
    }

    /// <summary>
    /// Registers Telegrator in DI container and returns ITelegramBotHostBuilder for further configuration.
    /// </summary>
    public static ITelegramBotHostBuilder AddTelegrator(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        AddTelegratorInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        return new TelegramBotHostBuilder(builder, handlers);
    }

    private static TelegratorOptions ParseTelegratorOptions(IConfiguration configuration)
    {
        return new TelegratorOptions
        {
            Token = configuration[nameof(TelegratorOptions.Token)] ?? throw new MissingMemberException("Token is required."),
            BaseUrl = configuration[nameof(TelegratorOptions.BaseUrl)],
            UseTestEnvironment = bool.TryParse(configuration[nameof(TelegratorOptions.UseTestEnvironment)], out bool useTestEnvironment) ? useTestEnvironment : false,
            RetryThreshold = int.TryParse(configuration[nameof(TelegratorOptions.RetryThreshold)], out int retryThreshold) ? retryThreshold : 60,
            RetryCount = int.TryParse(configuration[nameof(TelegratorOptions.RetryCount)], out int retryCount) ? retryCount : 3,
            MaximumParallelWorkingHandlers = int.TryParse(configuration[nameof(TelegratorOptions.MaximumParallelWorkingHandlers)], out int maximumParallelWorkingHandlers) ? maximumParallelWorkingHandlers : null,
            ExclusiveAwaitingHandlerRouting = bool.TryParse(configuration[nameof(TelegratorOptions.ExclusiveAwaitingHandlerRouting)], out bool exclusiveAwaitingHandlerRouting) ? exclusiveAwaitingHandlerRouting : false,
            ExceptIntersectingCommandAliases = bool.TryParse(configuration[nameof(TelegratorOptions.ExceptIntersectingCommandAliases)], out bool exceptIntersectingCommandAliases) ? exceptIntersectingCommandAliases : false,
        };
    }

    private static ReceiverOptions ParseReceiverOptions(IConfiguration configuration)
    {
        ReceiverOptions options = new ReceiverOptions();
        if (int.TryParse(configuration["Offset"], out int offset))
            options.Offset = offset;

        if (int.TryParse(configuration["Limit"], out int limit))
            options.Limit = limit;

        if (bool.TryParse(configuration["DropPendingUpdates"], out bool dropPending))
            options.DropPendingUpdates = dropPending;

        IConfigurationSection allowedUpdatesSection = configuration.GetSection("AllowedUpdates");
        if (allowedUpdatesSection.Exists())
        {
            List<UpdateType> updatesList = [];
            foreach (IConfigurationSection child in allowedUpdatesSection.GetChildren())
            {
                if (Enum.TryParse<UpdateType>(child.Value, ignoreCase: true, out var updateType))
                    updatesList.Add(updateType);
            }

            if (updatesList.Count > 0)
                options.AllowedUpdates = updatesList.ToArray();
        }

        return options;
    }

    /// <summary>
    /// Replaces TelegramBotHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    internal static void AddTelegratorInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
    {
        if (options == null)
        {
            IConfigurationSection section = configuration.GetSection("Telegrator");
            if (!section.Exists())
                section = configuration.GetSection(nameof(TelegratorOptions));

            if (!section.Exists())
                throw new MissingMemberException("Auto configuration enabled, yet no options of type 'TelegratorOptions' was registered. This configuration is runtime required!");

            options = ParseTelegratorOptions(section);
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
            IConfigurationSection section = configuration.GetSection("Receiver");
            if (!section.Exists())
                section = configuration.GetSection(nameof(ReceiverOptions));

            if (!section.Exists())
                throw new MissingMemberException("Auto configuration enabled, yet no options of type 'ReceiverOptions' was registered. This configuration is runtime required!");

            ReceiverOptions receiverOptions = ParseReceiverOptions(section);
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
    public static IServiceCollection AddStateStorage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TStorage>(this IServiceCollection services) where TStorage : IStateStorage
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
        ITelegramBotClient botClient = botHost.Services.GetRequiredService<ITelegramBotClient>();
        ITelegramBotInfo info = botHost.Services.GetRequiredService<ITelegramBotInfo>();
        IHandlersCollection handlers = botHost.Services.GetRequiredService<IHandlersCollection>();
        ILoggerFactory loggerFactory = botHost.Services.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
            logger.LogInformation("Initializing Telegrator Bot Host...");

        if (info is HostedTelegramBotInfo hostedInfo)
            hostedInfo.User = botClient.GetMe().ConfigureAwait(false).GetAwaiter().GetResult();

        if (logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Information))
        {
            logger.LogInformation("Telegrator Bot Host started");
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
    public static async Task<IHost> SetBotCommandsAsync(this IHost botHost)
    {
        ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
        IUpdateRouter router = botHost.Services.GetRequiredService<IUpdateRouter>();

        IEnumerable<BotCommand> aliases = router.HandlersProvider.GetBotCommands();
        if (aliases.Any())
        {
            await client.SetMyCommands(aliases).ConfigureAwait(false);
        }

        return botHost;
    }

    /// <summary>
    /// Configures bots available commands depending on what handlers was registered
    /// </summary>
    /// <param name="botHost"></param>
    /// <returns></returns>
    public static IHost SetBotCommands(this IHost botHost)
    {
        return SetBotCommandsAsync(botHost).GetAwaiter().GetResult();
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
            throw new InvalidOperationException("No update types were registered");

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
