using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegrator.Core;
using Telegrator.Hosting;
using Telegrator.Hosting.Web;
using Telegrator.Mediation;
using Telegrator.Providers;

namespace Telegrator;

/// <summary>
/// Provides extension methods for <see cref="IHostApplicationBuilder"/> to configure Telegrator.
/// </summary>
public static class WebHostBuilderExtensions
{
    /// <summary>
    /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddTelegratorWeb(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
    {
        AddTelegratorWebInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    public static IHostApplicationBuilder AddTelegratorWeb(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
    {
        AddTelegratorWebInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
        action?.Invoke(new TelegramBotHostBuilder(builder, handlers));
        return builder;
    }

    /// <summary>
    /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
    /// </summary>
    internal static void AddTelegratorWebInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
    {
        if (services.Any(srvc => srvc.ServiceType == typeof(HostedUpdateReceiver)))
            throw new InvalidOperationException("`HostedUpdateReceiver` found in services. WebHost extension is not compatible with long-polling receiving. Please, remove `AddTelegrator` invocation from your WebApp configuration.");

        if (options is null)
        {
            options = configuration.GetSection(nameof(TelegratorOptions)).Get<TelegratorOptions>();
            if (options is null)
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegratorOptions' was registered. This configuration is runtime required!");
        }

        CancellationTokenSource globallCancell = new CancellationTokenSource();
        options.GlobalCancellationToken = globallCancell.Token;
        services.AddSingleton(Options.Create(options));
        services.AddKeyedSingleton("cancell", globallCancell);

        if (handlers is not null && handlers is IHandlersManager manager)
        {
            ServiceDescriptor descriptor = new ServiceDescriptor(typeof(IHandlersProvider), manager);
            services.Replace(descriptor);
            services.AddSingleton(manager);
        }

        handlers ??= new HostHandlersCollection(services, options);
        services.AddSingleton(handlers);
        properties.Add(HostBuilderExtensions.HandlersCollectionPropertyKey, handlers);

        if (!services.Any(srvc => srvc.ServiceType == typeof(IOptions<WebhookerOptions>)))
        {
            WebhookerOptions? webhookerOptions = configuration.GetSection(nameof(WebhookerOptions)).Get<WebhookerOptions>();
            if (webhookerOptions == null)
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'WebhookerOptions' was registered. This configuration is runtime required!");

            services.AddSingleton(Options.Create(webhookerOptions));
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
        services.AddTelegramWebhook();
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure Telegram Bot WebHost
/// </summary>
public static class WebServicesCollectionExtensions
{
    public static IServiceCollection ConfigureWebhooker(this IServiceCollection services, WebhookerOptions options)
    {
        services.AddSingleton(Options.Create(options));
        return services;
    }

    /// <summary>
    /// Searchs for <see cref="HostedUpdateWebhooker"/> hosted service inside hosts services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="webhooker"></param>
    /// <returns></returns>
    public static bool TryFindWebhooker(this IServiceProvider services, [NotNullWhen(true)] out HostedUpdateWebhooker? webhooker)
    {
        webhooker = services.GetServices<IHostedService>().FirstOrDefault(s => s is HostedUpdateWebhooker) as HostedUpdateWebhooker;
        return webhooker != null;
    }
}

/// <summary>
/// Provides useful methods to adjust Telegram bot Host
/// </summary>
public static class WebTelegramBotHostExtensions
{
    /// <summary>
    /// Replaces the initialization logic from TelegramBotWebHost constructor. 
    /// Initializes the bot and logs handlers on application startup.
    /// </summary>
    public static T UseTelegratorWeb<T>(this T botHost, bool dontMap = false) where T : IEndpointRouteBuilder, IHost
    {
        if (!botHost.ServiceProvider.TryFindWebhooker(out HostedUpdateWebhooker? webhooker))
            throw new InvalidOperationException("No service for type 'Telegrator.Mediation.HostedUpdateWebhooker' has been registered.");

        ITelegramBotInfo info = botHost.ServiceProvider.GetRequiredService<ITelegramBotInfo>();
        IHandlersCollection handlers = botHost.ServiceProvider.GetRequiredService<IHandlersCollection>();
        ILoggerFactory loggerFactory = botHost.ServiceProvider.GetRequiredService<ILoggerFactory>();
        ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Telegrator Bot Host started (ASP.NET WebHost)");
            logger.LogInformation("Receiving mode : WEB-HOOKING");
            logger.LogInformation("Telegram Bot : {firstname}, @{usrname}, id:{id},", info.User.FirstName ?? "[NULL]", info.User.Username ?? "[NULL]", info.User.Id);
            logger.LogHandlers(handlers);
        }

        if (!dontMap)
            webhooker.MapWebhook(botHost);

        botHost.AddLoggingAdapter();
        botHost.SetBotCommands();
        return botHost;
    }

    /// <summary>
    /// Allows to remap receiving webhook endpoint and map new route to webhost.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="webhookUri"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T RemapWebhook<T>(this T app, string webhookUri) where T : IEndpointRouteBuilder, IHost
    {
        if (!app.ServiceProvider.TryFindWebhooker(out HostedUpdateWebhooker? webhooker))
            throw new InvalidOperationException("No service for type 'Telegrator.Mediation.HostedUpdateWebhooker' has been registered.");

        webhooker.RemapWebhook(app, webhookUri, default).GetAwaiter().GetResult();
        return app;
    }

    /// <summary>
    /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateWebhooker"/> to receive updates using webhook
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramWebhook(this IServiceCollection services)
    {
        services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        services.AddHostedService<HostedUpdateWebhooker>();
        return services;
    }

    private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
        => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
}
