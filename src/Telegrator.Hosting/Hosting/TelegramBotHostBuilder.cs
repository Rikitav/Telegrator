using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;
using Telegrator.Providers;

#pragma warning disable IDE0001
namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotHostBuilder : ICollectingProvider
    {
        private readonly HostApplicationBuilder _innerBuilder;
        private readonly HostApplicationBuilderSettings _settings;
        internal IHandlersCollection _handlers = null!;

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
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, HostApplicationBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            _innerBuilder.AddTelegrator(_settings);
            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers, HostApplicationBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            _innerBuilder.AddTelegrator(_settings, null, handlers);
            _innerBuilder.Logging.ClearProviders();
        }

        /// <summary>
        /// Builds the host.
        /// </summary>
        /// <returns></returns>
        public TelegramBotHost Build()
        {
            TelegramBotHost host = new TelegramBotHost(_innerBuilder);
            host.UseTelegrator();
            return host;
        }
    }
}
