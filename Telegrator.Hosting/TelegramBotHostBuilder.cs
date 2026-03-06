using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Hosting.Components;
using Telegrator.Hosting.Providers;
using Telegrator.MadiatorCore;

#pragma warning disable IDE0001
namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotHostBuilder : ITelegramBotHostBuilder
    {
        private readonly HostApplicationBuilder _innerBuilder;
        private readonly TelegramBotHostBuilderSettings _settings;
        private readonly IHandlersCollection _handlers;

        /// <inheritdoc/>
        public IHandlersCollection Handlers => _handlers;

        /// <inheritdoc/>
        public IServiceCollection Services => _innerBuilder.Services;

        /// <inheritdoc/>
        public IConfigurationManager Configuration => _innerBuilder.Configuration;

        /// <inheritdoc/>
        public ILoggingBuilder Logging => _innerBuilder.Logging;

        /// <inheritdoc/>
        public IHostEnvironment Environment => _innerBuilder.Environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, TelegramBotHostBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new TelegramBotHostBuilderSettings();
            _handlers = new HostHandlersCollection(Services, _settings);

            _innerBuilder.AddTelegrator(_settings, _handlers);
            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers, TelegramBotHostBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new TelegramBotHostBuilderSettings();
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));

            _innerBuilder.AddTelegrator(_settings, _handlers);
            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotHost Build()
        {
            return new TelegramBotHost(_innerBuilder, _handlers);
        }
    }
}
