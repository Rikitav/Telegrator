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
using Telegrator.Mediation;
using Telegrator.Providers;

namespace Telegrator;

/// <summary>
/// Provides extension methods for <see cref="IHostApplicationBuilder"/> to configure Telegrator.
/// </summary>
public static class WebHostBuilderExtensions
{
    /// <summary>
    /// Registers Telegrator to receive updates via WebHooks.
    /// </summary>
    public static ITelegramBotHostBuilder WithWeb(this ITelegramBotHostBuilder builder)
    {
        if (builder.Services.Any(srvc => srvc.ServiceType == typeof(HostedUpdateReceiver)))
            throw new InvalidOperationException("`HostedUpdateReceiver` found in services. WebHost extension is not compatible with long-polling receiving. Please, remove `WithPolling` invocation from your WebApp configuration.");

        if (!builder.Services.Any(srvc => srvc.ServiceType == typeof(IOptions<WebhookerOptions>)))
        {
            WebhookerOptions? webhookerOptions = builder.Configuration.GetSection(nameof(WebhookerOptions)).Get<WebhookerOptions>();
            if (webhookerOptions == null)
                throw new MissingMemberException("Auto configuration disabled, yet no options of type 'WebhookerOptions' was registered. This configuration is runtime required!");

            builder.Services.AddSingleton(Options.Create(webhookerOptions));
        }

        builder.Services.AddTelegramWebhook();
        return builder;
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure Telegram Bot WebHost
/// </summary>
public static class WebServicesCollectionExtensions
{
    /// <summary>
    /// Adds WebhookerOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureWebhooker(this IServiceCollection services, WebhookerOptions options)
    {
        services.RemoveAll<IOptions<WebhookerOptions>>();
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
        services.RemoveAll<IOptions<HostedUpdateWebhooker>>();
        services.RemoveAll<ITelegramBotClient>();

        services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        services.AddHostedService<HostedUpdateWebhooker>();
        return services;
    }

    private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
        => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
}
