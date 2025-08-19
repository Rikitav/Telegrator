﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;
using Telegrator.Hosting.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Represents a hosted telegram bot
    /// </summary>
    public class TelegramBotHost : ITelegramBotHost
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
        /// <param name="handlers"></param>
        public TelegramBotHost(HostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers)
        {
            // Registering this host in services for easy access
            RegisterHostServices(hostApplicationBuilder.Services, handlers);

            // Building proxy hoster
            _innerHost = hostApplicationBuilder.Build();
            _serviceProvider = _innerHost.Services;

            // Initializing bot info, as it requires to make a request via tg bot
            Services.GetRequiredService<ITelegramBotInfo>();

            // Reruesting services for this host
            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotHost>>();

            // Logging registering handlers in DEBUG purposes
            LogHandlers(handlers);
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default configuration, services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder()
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings: null);
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, null);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
            return builder;
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and long-polling update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotHostBuilder CreateBuilder(TelegramBotHostBuilderSettings? settings)
        {
            HostApplicationBuilder innerBuilder = new HostApplicationBuilder(settings?.ToApplicationBuilderSettings());
            TelegramBotHostBuilder builder = new TelegramBotHostBuilder(innerBuilder, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramReceiver();
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
        public static TelegramBotHostBuilder CreateEmptyBuilder(TelegramBotHostBuilderSettings? settings)
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

        private void LogHandlers(IHandlersCollection handlers)
        {
            StringBuilder logBuilder = new StringBuilder("Registered handlers : ");
            if (!handlers.Keys.Any())
                throw new Exception();

            foreach (UpdateType updateType in handlers.Keys)
            {
                HandlerDescriptorList descriptors = handlers[updateType];
                logBuilder.Append("\n\tUpdateType." + updateType + " :");

                foreach (HandlerDescriptor descriptor in descriptors.Reverse())
                {
                    logBuilder.AppendFormat("\n\t* {0} - {1}",
                        descriptor.Indexer.ToString(),
                        descriptor.ToString());
                }
            }

            Logger.LogInformation(logBuilder.ToString());
        }

        private void RegisterHostServices(IServiceCollection services, IHandlersCollection handlers)
        {
            //services.RemoveAll<IHost>();
            //services.AddSingleton<IHost>(this);

            services.AddSingleton<ITelegramBotHost>(this);
            services.AddSingleton<ITelegratorBot>(this);
            services.AddSingleton(handlers);

            if (handlers is IHandlersManager manager)
            {
                services.RemoveAll<IHandlersProvider>();
                services.AddSingleton<IHandlersProvider>(manager);
                services.AddSingleton(manager);
            }
        }
    }
}
