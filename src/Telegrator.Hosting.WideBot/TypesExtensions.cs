using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;
using Telegrator.Hosting;
using Telegrator.Mediation;
using TLUpdate = TL.Update;
using WUpdate = WTelegram.Types.Update;

namespace Telegrator;

/// <summary>
/// Provides extensions memebrs for <see cref="UpdateHandlerBase"/> for easy access to Wider bot functions and update
/// </summary>
public static class HandlersExtensions
{
    /// <summary>
    /// Gets the value of a field with the specified name from the given object using reflection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    public static object? GetFieldValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)] T>(this T obj, string fieldName)
    {
        if (obj == null)
            return null;

        return typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(obj);
    }

    extension<TUpdate>(AbstractUpdateHandler<TUpdate> handler) where TUpdate : class
    {
        /// <summary>
        /// Casts Update to <see cref="WTelegramBotClient"/>
        /// </summary>
        public WTelegramBotClient WClient
        {
            get
            {
                object? client = handler.GetFieldValue("Client");
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
                object? update = handler.GetFieldValue("HandlingUpdate");
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
    private static WideBotOptions ParseWideBotOptions(IConfiguration configuration)
    {
        return new WideBotOptions()
        {
            ApiHash = configuration[nameof(WideBotOptions.ApiHash)] ?? throw new MissingMemberException($"Required configuration value '{nameof(WideBotOptions.ApiHash)}' is missing."),
            ApiId = int.TryParse(configuration[nameof(WideBotOptions.ApiId)], out int apiId) ? apiId : throw new MissingMemberException($"Required configuration value '{nameof(WideBotOptions.ApiId)}' is missing or invalid."),
            DropPendingUpdates = bool.TryParse(configuration[nameof(WideBotOptions.DropPendingUpdates)], out bool dropPending) ? dropPending : false,
            MTProxy = configuration[nameof(WideBotOptions.MTProxy)]
        };
    }

    /// <summary>
    /// Registers Telegrator and configures it to receive updates via long-polling using WTelegramBotClient.
    /// </summary>
    public static ITelegramBotHostBuilder WithWide(this ITelegramBotHostBuilder builder, Func<IServiceProvider, DbConnection> dbConnectionFactory)
    {
        if (builder.Services.Any(srvc => srvc.ServiceType == typeof(HostedUpdateReceiver)))
            throw new InvalidOperationException("`HostedUpdateReceiver` found in services. WideHost extension is not compatible with default long-polling receiver. Please, remove `WithPolling` invocation from your Host configuration.");

        if (builder.Services.Any(srvc => srvc.ServiceType.FullName == "Telegrator.Mediation.HostedUpdateWebhooker"))
            throw new InvalidOperationException("`HostedUpdateWebhooker` found in services. WideHost extension is not compatible with webhooking yet. Please, remove `WithWeb` invocation from your Host configuration.");

        if (!builder.Services.Any(srvc => srvc.ServiceType == typeof(IOptions<WTelegramBotClientOptions>)))
        {
            IConfigurationSection section = builder.Configuration.GetSection(nameof(WideBotOptions));
            if (!section.Exists())
                section = builder.Configuration.GetSection("WideBot");

            if (!section.Exists())
                throw new MissingMemberException("Auto configuration enabled, yet no options of type 'WideBotOptions' was registered. This configuration is runtime required!");

            WideBotOptions wideBotOptions = ParseWideBotOptions(section);

            builder.Services.AddSingleton(provider =>
            {
                TelegratorOptions options = provider.GetRequiredService<IOptions<TelegratorOptions>>().Value;
                IHostApplicationLifetime lifetime = provider.GetRequiredService<IHostApplicationLifetime>();

                DbConnection dbConnection = dbConnectionFactory.Invoke(provider);
                lifetime.ApplicationStopping.Register(() => dbConnection.Dispose());

                WTelegramBotClientOptions wideOptions = new WTelegramBotClientOptions(
                    token: options.Token,
                    apiId: wideBotOptions.ApiId,
                    apiHash: wideBotOptions.ApiHash,
                    dbConnection: dbConnection,
                    sqlCommands: wideBotOptions.SqlCommands,
                    useTestEnvironment: options.UseTestEnvironment,
                    mtproxy: wideBotOptions.MTProxy)
                {
                    RetryCount = options.RetryCount,
                    RetryThreshold = options.RetryThreshold
                };

                return Options.Create(wideOptions);
            });
        }

        builder.Services.AddMTProtoUpdateReceiver();
        return builder;
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
    /// Adds WTelegramBotClientOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureWideBot(this IServiceCollection services, WideBotOptions options)
    {
        services.RemoveAll<IOptions<WideBotOptions>>();
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
