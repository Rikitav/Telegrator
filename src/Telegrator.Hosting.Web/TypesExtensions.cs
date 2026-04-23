using Microsoft.AspNetCore.Builder;
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

namespace Telegrator
{
    /// <summary>
    /// Contains extensions for <see cref="IServiceCollection"/>
    /// Provides method to configure Telegram Bot WebHost
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// The key used to store the <see cref="IHandlersCollection"/> in the builder properties.
        /// </summary>
        public const string HandlersCollectionPropertyKey = nameof(IHandlersCollection);

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        public static ITelegramBotHostBuilder AddTelegratorWeb(this ITelegramBotHostBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
        {
            AddTelegratorWebInternal(builder.Services, builder.Configuration, builder.Properties, ref handlers, options);
            if (builder is TelegramBotWebHostBuilder telegramBotHostBuilder)
                telegramBotHostBuilder._handlers = handlers;

            action?.Invoke(builder);
            return builder;
        }

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        public static IHostApplicationBuilder AddTelegratorWeb(this WebApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
        {
            AddTelegratorWebInternal(builder.Services, builder.Configuration, ((IHostApplicationBuilder)builder).Properties, ref handlers, options);
            action?.Invoke(new TelegramBotWebHostBuilder(builder, handlers));
            return builder;
        }

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        internal static void AddTelegratorWebInternal(IServiceCollection services, IConfiguration configuration, IDictionary<object, object> properties, [NotNull] ref IHandlersCollection? handlers, TelegratorOptions? options = null)
        {
            if (options == null)
            {
                options = configuration.GetSection(nameof(TelegratorOptions)).Get<TelegratorOptions>();
                if (options == null)
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegratorOptions' wasn't registered. This configuration is runtime required!");
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

            if (!services.Any(srvc => srvc.ImplementationType == typeof(IOptions<WebhookerOptions>)))
            {
                WebhookerOptions? webhookerOptions = configuration.GetSection(nameof(WebhookerOptions)).Get<WebhookerOptions>();
                if (webhookerOptions == null)
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'WebhookerOptions' wasn't registered. This configuration is runtime required!");

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

        /// <summary>
        /// Replaces the initialization logic from TelegramBotWebHost constructor. 
        /// Initializes the bot and logs handlers on application startup.
        /// </summary>
        public static T UseTelegratorWeb<T>(this T app, bool dontMap = false) where T : IEndpointRouteBuilder, IHost
        {
            if (!app.ServiceProvider.TryFindWebhooker(out HostedUpdateWebhooker? webhooker))
                throw new InvalidOperationException("No service for type 'Telegrator.Mediation.HostedUpdateWebhooker' has been registered.");

            ITelegramBotInfo info = app.ServiceProvider.GetRequiredService<ITelegramBotInfo>();
            IHandlersCollection handlers = app.ServiceProvider.GetRequiredService<IHandlersCollection>();
            ILoggerFactory loggerFactory = app.ServiceProvider.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Telegrator Bot ASP.NET WebHost started");
                logger.LogInformation("Telegram Bot : {firstname}, @{usrname}, id:{id},", info.User.FirstName ?? "[NULL]", info.User.Username ?? "[NULL]", info.User.Id);
                logger.LogHandlers(handlers);
            }

            if (!dontMap)
                webhooker.MapWebhook(app);
            
            return app;
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
}
