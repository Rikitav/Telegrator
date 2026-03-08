using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.Core;
using Telegrator.Providers;

#pragma warning disable IDE0001
namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
    /// </summary>
    public class TelegramBotHostBuilder : IHostApplicationBuilder, ICollectingProvider
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

        /// <inheritdoc/>
        public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_innerBuilder).Properties;

        /// <inheritdoc/>
        public IMetricsBuilder Metrics => _innerBuilder.Metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, HostApplicationBuilderSettings? settings = null)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            this.AddTelegrator();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="options"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, TelegratorOptions? options, HostApplicationBuilderSettings? settings)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            this.AddTelegrator(options, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers, HostApplicationBuilderSettings? settings)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            this.AddTelegrator(null, handlers);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder"></param>
        /// <param name="handlers"></param>
        /// <param name="options"></param>
        /// <param name="settings"></param>
        public TelegramBotHostBuilder(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers, TelegratorOptions? options, HostApplicationBuilderSettings? settings)
        {
            _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
            _settings = settings ?? new HostApplicationBuilderSettings();

            this.AddTelegrator(options, handlers);
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

        /// <inheritdoc/>
        public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
        {
            this.ConfigureContainer(factory, configure);
        }
    }
}
