using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegrator.Core;

namespace Telegrator.Hosting;

/// <summary>
/// Represents a hosted telegram bots and services builder that helps manage configuration, logging, lifetime, and more.
/// </summary>
public interface ITelegramBotHostBuilder : ICollectingProvider
{
    /// <summary>
    /// Gets a central location for sharing state between components during the host building process.
    /// </summary>
    IDictionary<object, object> Properties { get; }

    /// <summary>
    /// Gets the set of key/value configuration properties.
    /// </summary>
    IConfiguration Configuration { get; }

    /// <summary>
    /// Gets the information about the hosting environment an application is running in.
    /// </summary>
    IHostEnvironment Environment { get; }

    /// <summary>
    /// Gets a collection of logging providers for the application to compose. This is useful for adding new logging providers.
    /// </summary>
    ILoggingBuilder Logging { get; }

    /// <summary>
    /// Gets a builder that allows enabling metrics and directing their output.
    /// </summary>
    IMetricsBuilder Metrics { get; }

    /// <summary>
    /// Gets a collection of services for the application to compose. This is useful for adding user provided or framework provided services.
    /// </summary>
    IServiceCollection Services { get; }
}
