using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Represents a web hosted telegram bot
    /// </summary>
    public class TelegramBotWebHost : ITelegramBotWebHost
    {
        private readonly WebApplication _innerApp;
        private readonly IUpdateRouter _updateRouter;
        private readonly ILogger<TelegramBotWebHost> _logger;

        private bool _disposed;

        /// <inheritdoc/>
        public IServiceProvider Services => _innerApp.Services;

        /// <inheritdoc/>
        public IUpdateRouter UpdateRouter => _updateRouter;

        /// <inheritdoc/>
        public ICollection<EndpointDataSource> DataSources => ((IEndpointRouteBuilder)_innerApp).DataSources;

        /// <summary>
        /// Allows consumers to be notified of application lifetime events.
        /// </summary>
        public IHostApplicationLifetime Lifetime => _innerApp.Lifetime;

        /// <summary>
        /// This application's logger
        /// </summary>
        public ILogger<TelegramBotWebHost> Logger => _logger;

        // Private interface fields
        IServiceProvider IEndpointRouteBuilder.ServiceProvider => Services;
        IServiceProvider IApplicationBuilder.ApplicationServices { get => Services; set => throw new NotImplementedException(); }
        IFeatureCollection IApplicationBuilder.ServerFeatures => ((IApplicationBuilder)_innerApp).ServerFeatures;
        IDictionary<string, object?> IApplicationBuilder.Properties => ((IApplicationBuilder)_innerApp).Properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApplicationBuilder"/> class.
        /// </summary>
        /// <param name="webApplicationBuilder">The proxied instance of host builder.</param>
        /// <param name="handlers"></param>
        public TelegramBotWebHost(WebApplicationBuilder webApplicationBuilder, IHandlersCollection handlers)
        {
            // Registering this host in services for easy access
            RegisterHostServices(webApplicationBuilder.Services, handlers);

            // Building proxy application
            _innerApp = webApplicationBuilder.Build();
            _innerApp.UseTelegratorWeb();

            // Reruesting services for this host
            _updateRouter = Services.GetRequiredService<IUpdateRouter>();
            _logger = Services.GetRequiredService<ILogger<TelegramBotWebHost>>();
        }

        /// <summary>
        /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateBuilder(settings.ToWebApplicationOptions());
            TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramWebhook();
            return builder;
        }

        /// <summary>
        /// Creates new SLIM <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateSlimBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateSlimBuilder(settings.ToWebApplicationOptions());
            TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);
            builder.Services.AddTelegramBotHostDefaults();
            builder.Services.AddTelegramWebhook();
            return builder;
        }

        /// <summary>
        /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
        /// </summary>
        /// <returns></returns>
        public static TelegramBotWebHostBuilder CreateEmptyBuilder(TelegramBotWebOptions settings)
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            WebApplicationBuilder innerApp = WebApplication.CreateEmptyBuilder(settings.ToWebApplicationOptions());
            return new TelegramBotWebHostBuilder(innerApp, settings);
        }

        /// <inheritdoc/>
        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _innerApp.StartAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public async Task StopAsync(CancellationToken cancellationToken = default)
        {
            await _innerApp.StopAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public IApplicationBuilder CreateApplicationBuilder()
            => ((IEndpointRouteBuilder)_innerApp).CreateApplicationBuilder();

        /// <inheritdoc/>
        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
            => _innerApp.Use(middleware);

        /// <inheritdoc/>
        public IApplicationBuilder New()
            => ((IApplicationBuilder)_innerApp).New();

        /// <inheritdoc/>
        public RequestDelegate Build()
            => ((IApplicationBuilder)_innerApp).Build();

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await _innerApp.DisposeAsync();

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        /// <summary>
        /// Disposes the host.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            // Sorry for this, i really dont know how to handle such cases
            ValueTask disposeTask = _innerApp.DisposeAsync();
            while (!disposeTask.IsCompleted)
                Thread.Sleep(100);

            GC.SuppressFinalize(this);
            _disposed = true;
        }

        private void RegisterHostServices(IServiceCollection services, IHandlersCollection handlers)
        {
            //service.RemoveAll<IHost>();
            //service.AddSingleton<IHost>(this);

            services.AddSingleton<ITelegramBotHost>(this);
            services.AddSingleton<ITelegramBotWebHost>(this);
            services.AddSingleton<ITelegratorBot>(this);
        }
    }
}
