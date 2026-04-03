using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

#pragma warning disable IDE0001
namespace Telegrator.Hosting.Web;

/// <inheritdoc/>
public class TelegramBotWebHostBuilder : ITelegramBotHostBuilder
{
    private readonly WebApplicationBuilder _innerBuilder;
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
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotWebHostBuilder"/> class.
    /// </summary>
    /// <param name="webApplicationBuilder"></param>
    /// <param name="handlers"></param>
    public TelegramBotWebHostBuilder(WebApplicationBuilder webApplicationBuilder, IHandlersCollection handlers)
    {
        _innerBuilder = webApplicationBuilder ?? throw new ArgumentNullException(nameof(webApplicationBuilder));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    /// <summary>
    /// Builds the host.
    /// </summary>
    /// <returns></returns>
    public TelegramBotWebHost Build()
    {
        TelegramBotWebHost host = new TelegramBotWebHost(_innerBuilder);
        host.UseTelegratorWeb();
        return host;
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        ((IHostApplicationBuilder)_innerBuilder).ConfigureContainer(factory, configure);
    }
}
