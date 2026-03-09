using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

namespace Telegrator.Hosting.Web;

/// <summary>
/// Represents a web hosted telegram bot
/// </summary>
public class TelegramBotWebHost : IHost, IApplicationBuilder, IEndpointRouteBuilder, IAsyncDisposable
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
    public TelegramBotWebHost(WebApplicationBuilder webApplicationBuilder)
    {
        // Building proxy application
        _innerApp = webApplicationBuilder.Build();

        // Reruesting services for this host
        _updateRouter = Services.GetRequiredService<IUpdateRouter>();
        _logger = Services.GetRequiredService<ILogger<TelegramBotWebHost>>();
    }

    /// <summary>
    /// Creates new <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
    /// </summary>
    /// <returns></returns>
    public static TelegramBotWebHostBuilder CreateBuilder(WebApplicationOptions settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        WebApplicationBuilder innerApp = WebApplication.CreateBuilder(settings);
        TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);

        builder.Services.AddTelegramBotHostDefaults();
        builder.Services.AddTelegramWebhook();
        return builder;
    }

    /// <summary>
    /// Creates new SLIM <see cref="TelegramBotHostBuilder"/> with default services and webhook update receiving scheme
    /// </summary>
    /// <returns></returns>
    public static TelegramBotWebHostBuilder CreateSlimBuilder(WebApplicationOptions settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        WebApplicationBuilder innerApp = WebApplication.CreateSlimBuilder(settings);
        TelegramBotWebHostBuilder builder = new TelegramBotWebHostBuilder(innerApp, settings);

        builder.Services.AddTelegramBotHostDefaults();
        builder.Services.AddTelegramWebhook();
        return builder;
    }

    /// <summary>
    /// Creates new EMPTY <see cref="TelegramBotHostBuilder"/> WITHOUT any services or update receiving schemes
    /// </summary>
    /// <returns></returns>
    public static TelegramBotWebHostBuilder CreateEmptyBuilder(WebApplicationOptions settings)
    {
        ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        WebApplicationBuilder innerApp = WebApplication.CreateEmptyBuilder(settings);
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

        ValueTask disposeTask = _innerApp.DisposeAsync();
        disposeTask.AsTask().Wait();

        GC.SuppressFinalize(this);
        _disposed = true;
    }
}
