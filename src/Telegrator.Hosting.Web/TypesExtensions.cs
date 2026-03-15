using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        extension(WebApplicationBuilder builder)
        {
            /// <summary>
            /// Gets the <see cref="IHandlersCollection"/> from the builder properties.
            /// </summary>
            public IHandlersCollection Handlers
            {
                get
                {
                    return (IHandlersCollection)builder.Host.Properties[HandlersCollectionPropertyKey];
                }
            }
        }

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        public static ITelegramBotHostBuilder AddTelegratorWeb(this ITelegramBotHostBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
        {
            builder.AddTelegratorWebInternal(options, handlers);
            action?.Invoke(builder);
            return builder;
        }

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        public static IHostApplicationBuilder AddTelegratorWeb(this WebApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null, Action<ITelegramBotHostBuilder>? action = null)
        {
            builder.AddTelegratorWebInternal(options, handlers);
            action?.Invoke(new TelegramBotWebHostBuilder(builder));
            return builder;
        }

        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        internal static IHostApplicationBuilder AddTelegratorWebInternal(this IHostApplicationBuilder builder, TelegratorOptions? options = null, IHandlersCollection? handlers = null)
        {
            IServiceCollection services = builder.Services;
            IConfigurationManager configuration = builder.Configuration;

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

            builder.Properties.Add(HandlersCollectionPropertyKey, handlers);
            if (builder is TelegramBotWebHostBuilder botHostBuilder)
                botHostBuilder._handlers = handlers;

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
            return builder;
        }

        /// <summary>
        /// Replaces the initialization logic from TelegramBotWebHost constructor. 
        /// Initializes the bot and logs handlers on application startup.
        /// </summary>
        public static T UseTelegratorWeb<T>(this T app) where T : IEndpointRouteBuilder, IHost
        {
            HostedUpdateWebhooker webhooker = app.ServiceProvider.GetRequiredService<HostedUpdateWebhooker>();
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

            webhooker.MapWebhook(app);
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
