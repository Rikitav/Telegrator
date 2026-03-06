using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Telegram.Bot;
using Telegrator;
using Telegrator.Core;
using Telegrator.Hosting.Web;
using Telegrator.Mediation;
using Telegrator.Providers;

namespace Telegrator
{
    /// <summary>
    /// Contains extensions for <see cref="IServiceCollection"/>
    /// Provides method to configure <see cref="ITelegramBotWebHost"/>
    /// </summary>
    public static class ServicesCollectionExtensions
    {
        /// <summary>
        /// Replaces TelegramBotWebHostBuilder. Configures DI, options, and handlers.
        /// </summary>
        public static WebApplicationBuilder AddTelegratorWeb(this WebApplicationBuilder builder, TelegramBotWebOptions settings, IHandlersCollection? handlers = null)
        {
            if (settings is null)
                throw new ArgumentNullException(nameof(settings));

            IServiceCollection services = builder.Services;
            ConfigurationManager configuration = builder.Configuration;

            handlers ??= new HostHandlersCollection(services, settings);

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
                services.Configure<TelegratorWebOptions>(configuration.GetSection(nameof(TelegratorWebOptions)));
            }
            else
            {
                if (!services.Any(srvc => srvc.ImplementationType == typeof(IOptions<TelegratorWebOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegratorWebOptions' wasn't registered. This configuration is runtime required!");

                if (!services.Any(srvc => srvc.ImplementationType == typeof(IOptions<TelegramBotClientOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegramBotClientOptions' wasn't registered. This configuration is runtime required!");
            }

            IOptions<TelegramBotWebOptions> options = Options.Create(settings);
            services.AddSingleton((IOptions<TelegratorOptions>)options);
            services.AddSingleton(options);
            services.AddSingleton(handlers);

            if (handlers is IHandlersManager manager)
            {
                ServiceDescriptor descriptor = new ServiceDescriptor(typeof(IHandlersProvider), manager);
                services.Replace(descriptor);
                services.AddSingleton(manager);
            }

            services.AddTelegramBotHostDefaults();
            services.AddTelegramWebhook();

            return builder;
        }

        /// <summary>
        /// Replaces the initialization logic from TelegramBotWebHost constructor. 
        /// Initializes the bot and logs handlers on application startup.
        /// </summary>
        public static WebApplication UseTelegratorWeb(this WebApplication app)
        {
            ITelegramBotInfo info = app.Services.GetRequiredService<ITelegramBotInfo>();
            IHandlersCollection handlers = app.Services.GetRequiredService<IHandlersCollection>();
            ILoggerFactory loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            ILogger logger = loggerFactory.CreateLogger("Telegrator.Hosting.Web.TelegratorHost");

            logger.LogInformation("Telegrator Bot ASP.NET WebHost started");
            logger.LogHandlers(handlers);

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
