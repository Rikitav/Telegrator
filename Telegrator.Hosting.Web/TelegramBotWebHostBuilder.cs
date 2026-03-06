using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Configuration;
using Telegrator.Hosting.Providers;
using Telegrator.Hosting.Providers.Components;
using Telegrator.MadiatorCore;

#pragma warning disable IDE0001
namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Represents a web hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotWebHostBuilder : ITelegramBotHostBuilder
    {
        private readonly WebApplicationBuilder _innerBuilder;
        private readonly TelegramBotWebOptions _settings;
        private readonly IHandlersCollection _handlers;

        /// <inheritdoc/>
        public IHandlersCollection Handlers => _handlers;

        /// <inheritdoc/>
        public IConfigurationManager Configuration => _innerBuilder.Configuration;

        /// <inheritdoc/>
        public ILoggingBuilder Logging => _innerBuilder.Logging;

        /// <inheritdoc/>
        public IServiceCollection Services => _innerBuilder.Services;

        /// <inheritdoc/>
        public IHostEnvironment Environment => _innerBuilder.Environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
        /// </summary>
        /// <param name="webApplicationBuilder"></param>
        /// <param name="settings"></param>
        public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, TelegramBotWebOptions settings)
        {
            _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _handlers = new HostHandlersCollection(Services, _settings);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
        /// </summary>
        /// <param name="webApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="settings"></param>
        public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, TelegramBotWebOptions settings, IHandlersCollection handlers)
        {
            _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _handlers = handlers ?? throw new ArgumentNullException(nameof(settings));

            _innerBuilder.AddTelegratorWeb(settings);
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotWebHost Build()
        {
            if (_handlers is IHostHandlersCollection hostHandlers)
            {
                foreach (PreBuildingRoutine preBuildRoutine in hostHandlers.PreBuilderRoutines)
                {
                    try
                    {
                        preBuildRoutine.Invoke(this);
                    }
                    catch (NotImplementedException)
                    {
                        _ = 0xBAD + 0xC0DE;
                    }
                }
            }

            if (!_settings.DisableAutoConfigure)
            {
                Services.Configure<TelegratorWebOptions>(Configuration.GetSection(nameof(TelegratorWebOptions)));
                Services.Configure<TelegramBotClientOptions>(Configuration.GetSection(nameof(TelegramBotClientOptions)), new TelegramBotClientOptionsProxy());
            }
            else
            {
                if (null == Services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<TelegratorWebOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegratorWebOptions' wasn't registered. This configuration is runtime required!");

                if (null == Services.SingleOrDefault(srvc => srvc.ImplementationType == typeof(IOptions<TelegramBotClientOptions>)))
                    throw new MissingMemberException("Auto configuration disabled, yet no options of type 'TelegramBotClientOptions' wasn't registered. This configuration is runtime required!");
            }

            Services.AddSingleton<IConfigurationManager>(Configuration);
            Services.AddSingleton<IOptions<TelegratorOptions>>(Options.Create(_settings));
            return new TelegramBotWebHost(_innerBuilder, _handlers);
        }
    }
}
