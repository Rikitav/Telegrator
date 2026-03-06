using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Handlers;
using Telegrator.Hosting;
using Telegrator.Logging;
using Telegrator.Polling;
using Telegrator.Providers;

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

    extension (IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Gets the <see cref="IHandlersCollection"/> from the builder properties.
        /// </summary>
        public IHandlersCollection Handlers => (IHandlersCollection)builder.Properties[HandlersCollectionPropertyKey];
    }

    /// <summary>
    /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddTelegrator(this IHostApplicationBuilder builder, TelegramBotHostBuilderSettings settings, IHandlersCollection? handlers = null)
    {
        if (settings is null)
            throw new ArgumentNullException(nameof(settings));

        IServiceCollection services = builder.Services;
        IConfigurationManager configuration = builder.Configuration;

        handlers ??= new HostHandlersCollection(services, settings);
        builder.Properties.Add(HandlersCollectionPropertyKey, handlers);

        if (handlers is IHostHandlersCollection hostHandlers)
        {
            foreach (PreBuildingRoutine preBuildRoutine in hostHandlers.PreBuilderRoutines)
            {
                try
                {
                    // TODO: fix
                    //preBuildRoutine.Invoke(builder);
                    Debug.WriteLine("Pre-Building routine was not executed");
                }
                catch (NotImplementedException)
                {
                    _ = 0xBAD + 0xC0DE;
                }
            }
        }

        if (!settings.DisableAutoConfigure)
        {
            services.Configure<ReceiverOptions>(configuration.GetSection(nameof(ReceiverOptions)));
            services.Configure(configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
        }
        else
        {
            if (null == services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<ReceiverOptions>)))
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'ReceiverOptions' wasn't registered. This configuration is runtime required!");

            if (null == services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<TelegramBotClientOptions>)))
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegramBotClientOptions' wasn't registered. This configuration is runtime required!");
        }

        IOptions<TelegramBotHostBuilderSettings> options = Options.Create(settings);
        services.AddSingleton((IOptions<TelegratorOptions>)options);
        services.AddTelegramBotHostDefaults();
        services.AddSingleton(options);
        services.AddSingleton(handlers);

        if (handlers is IHandlersManager manager)
        {
            ServiceDescriptor descriptor = new ServiceDescriptor(typeof(IHandlersProvider), manager);
            services.Replace(descriptor);
            services.AddSingleton(manager);
        }

        return builder;
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure <see cref="ITelegramBotHost"/>
/// </summary>
public static class ServicesCollectionExtensions
{
    /// <summary>
    /// Registers a configuration instance that strongly-typed <typeparamref name="TOptions"/> will bind against using <see cref="ConfigureOptionsProxy{TOptions}"/>.
    /// </summary>
    /// <typeparam name="TOptions"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="optionsProxy"></param>
    /// <returns></returns>
    public static IServiceCollection Configure<TOptions>(this IServiceCollection services, IConfiguration configuration, ConfigureOptionsProxy<TOptions> optionsProxy) where TOptions : class
    {
        optionsProxy.Configure(services, configuration);
        return services;
    }

    /// <summary>
    /// Registers <see cref="TelegramBotHost"/> default services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramBotHostDefaults(this IServiceCollection services)
    {
        services.AddLogging(builder => builder.AddConsole().AddDebug());
        services.AddSingleton<IUpdateHandlersPool, HostUpdateHandlersPool>();
        services.AddSingleton<IAwaitingProvider, HostAwaitingProvider>();
        services.AddSingleton<IHandlersProvider, HostHandlersProvider>();
        services.AddSingleton<IUpdateRouter, HostUpdateRouter>();
        services.AddSingleton<ITelegramBotInfo, HostedTelegramBotInfo>();

        return services;
    }

    /// <summary>
    /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateReceiver"/> to receive updates using long polling
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramReceiver(this IServiceCollection services)
    {
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
/// Provides useful methods to adjust <see cref="ITelegramBotHost"/>
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
            logger.LogInformation("Telegrator Bot .NET Host started");
            logger.LogInformation("Telegram Bot : {firstname}, @{usrname}, id:{id},", info.User.FirstName ?? "[NULL]", info.User.Username ?? "[NULL]", info.User.Id);
            logger.LogHandlers(handlers);
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
        ITelegramBotClient client = botHost.Services.GetRequiredService<ITelegramBotClient>();
        IUpdateRouter router = botHost.Services.GetRequiredService<IUpdateRouter>();

        IEnumerable<BotCommand> aliases = router.HandlersProvider.GetBotCommands();
        client.SetMyCommands(aliases).Wait();
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
/// Provides extension methods for reflection and type inspection.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Checks if a type implements the <see cref="IPreBuildingRoutine"/> interface.
    /// </summary>
    /// <param name="handlerType">The type to check.</param>
    /// <param name="routineMethod"></param>
    /// <returns>True if the type implements IPreBuildingRoutine; otherwise, false.</returns>
    public static bool IsPreBuildingRoutine(this Type handlerType, [NotNullWhen(true)] out MethodInfo? routineMethod)
    {
        routineMethod = null;
        if (handlerType.GetInterface(nameof(IPreBuildingRoutine)) == null)
            return false;

        routineMethod = handlerType.GetMethod(nameof(IPreBuildingRoutine.PreBuildingRoutine), BindingFlags.Static | BindingFlags.Public);
        return routineMethod != null;
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
            throw new Exception();

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
