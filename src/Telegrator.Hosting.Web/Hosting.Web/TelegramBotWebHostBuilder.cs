using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

#pragma warning disable IDE0001
namespace Telegrator.Hosting.Web;

/// <summary>
/// Represents a web hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
/// </summary>
public class TelegramBotWebHostBuilder : IHostApplicationBuilder, ICollectingProvider
{
    private readonly WebApplicationBuilder _innerBuilder;
    private readonly WebApplicationOptions _settings;
    internal IHandlersCollection _handlers = null!;

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

    /// <inheritdoc/>
    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_innerBuilder).Properties;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => _innerBuilder.Metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="settings"></param>
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, WebApplicationOptions? settings = null)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        this.AddTelegratorWeb();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="options"></param>
    /// <param name="settings"></param>
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, TelegratorOptions? options, WebApplicationOptions? settings)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        this.AddTelegratorWeb(options, null);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="handlers"></param>
    /// <param name="settings"></param>
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, IHandlersCollection handlers, WebApplicationOptions settings)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        this.AddTelegratorWeb(null, handlers);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="handlers"></param>
    /// <param name="options"></param>
    /// <param name="settings"></param>
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, IHandlersCollection handlers, TelegratorOptions? options, WebApplicationOptions settings)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        this.AddTelegratorWeb(options, handlers);
    }

    /// <summary>
    /// Builds the host.
    /// </summary>
    /// <returns></returns>
    public TelegramBotWebHost Build()
    {
        TelegramBotWebHost host = new TelegramBotWebHost(_innerBuilder);
        host.UseTelegrator();
        return host;
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        ((IHostApplicationBuilder)_innerBuilder).ConfigureContainer(factory, configure);
    }
}
