using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

#pragma warning disable IDE0001
namespace Telegrator.Hosting;

/// <inheritdoc/>
public class TelegramBotHostBuilder : ITelegramBotHostBuilder
{
    private readonly IHostApplicationBuilder _innerBuilder;
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
    public IDictionary<object, object> Properties => _innerBuilder.Properties;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => _innerBuilder.Metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
    /// </summary>
    /// <param name="hostApplicationBuilder"></param>
    public TelegramBotHostBuilder(IHostApplicationBuilder hostApplicationBuilder)
    {
        _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
    /// </summary>
    /// <param name="hostApplicationBuilder"></param>
    /// <param name="handlers"></param>
    public TelegramBotHostBuilder(IHostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers)
    {
        _innerBuilder = hostApplicationBuilder ?? throw new ArgumentNullException(nameof(hostApplicationBuilder));
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    /// <inheritdoc/>
    public void ConfigureContainer<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory, Action<TContainerBuilder>? configure = null) where TContainerBuilder : notnull
    {
        _innerBuilder.ConfigureContainer(factory, configure);
    }
}
