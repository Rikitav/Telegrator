using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bot
    /// </summary>
    public class TelegramBotHost : IHost, ITelegratorBot
    {
        private readonly IHost _innerHost;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUpdateRouter _updateRouter;
        private readonly ILogger<TelegramBotHost> _logger;

        private bool _disposed;

        /// <inheritdoc/>
        public IServiceProvider Services => _serviceProvider;

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter => _updateRouter;

        /// <summary>
        /// This application's logger
        /// </summary>
        public ILogger<TelegramBotHost> Logger => _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramBotHost"/> class.
        /// </summary>
        /// <param name="hostApplicationBuilder">The proxied instance of host builder.</param>
        public TelegramBotHost(HostApplicationBuilder hostApplicationBuilder)
        {
            // Registering this host in services for easy access
            hostApplicationBuilder.Services.AddSingleton<ITelegratorBot>(this);

            // Building proxy hoster
            _innerHost = hostApplicationBuilder.Build();
            _serviceProvider = _innerHost.Services;
            _innerHost.UseTelegrator();

            // Reruesting services for this host
            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotHost>>();
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default configuration, services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder()
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings: null);
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, null);
            return builder;
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder(HostApplicationBuilderSettings? settings)
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings);
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, settings);
            return builder;
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateEmptyBuilder()
        {
            HostApplicationBuilder innerBuilder = Host.CreateEmptyApplicationBuilder(null);
            return new TelegramBotHostBuilder(innerBuilder, null);
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateEmptyBuilder(HostApplicationBuilderSettings? settings)
        {
            HostApplicationBuilder innerBuilder = Host.CreateEmptyApplicationBuilder(null);
            return new TelegramBotHostBuilder(innerBuilder, settings);
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _innerHost.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _innerHost.StopAsync(cancellationToken);
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _innerHost.Dispose();

            GC.SuppressFinalize(this);
            _disposed = true;
        }
    }
}
