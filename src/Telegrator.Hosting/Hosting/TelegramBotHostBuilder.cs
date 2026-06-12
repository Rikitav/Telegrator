/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
    //private readonly IHostApplicationBuilder _innerBuilder;
    private readonly IHandlersCollection _handlers;
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;
    private readonly ILoggingBuilder _logging;
    private readonly IHostEnvironment _environment;
    private readonly IDictionary<object, object> _properties;
    private readonly IMetricsBuilder _metrics;

    /// <inheritdoc/>
    public IHandlersCollection Handlers => _handlers;

    /// <inheritdoc/>
    public IServiceCollection Services => _services;

    /// <inheritdoc/>
    public IConfiguration Configuration => _configuration;

    /// <inheritdoc/>
    public ILoggingBuilder Logging => _logging;

    /// <inheritdoc/>
    public IHostEnvironment Environment => _environment;

    /// <inheritdoc/>
    public IDictionary<object, object> Properties => _properties;

    /// <inheritdoc/>
    public IMetricsBuilder Metrics => _metrics;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
    /// </summary>
    /// <param name="hostApplicationBuilder"></param>
    /// <param name="handlers"></param>
    public TelegramBotHostBuilder(IHostApplicationBuilder hostApplicationBuilder, IHandlersCollection handlers)
    {
        if (hostApplicationBuilder == null)
            throw new ArgumentNullException(nameof(hostApplicationBuilder));

        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _services = hostApplicationBuilder.Services;
        _configuration = hostApplicationBuilder.Configuration;
        _logging = hostApplicationBuilder.Logging;
        _environment = hostApplicationBuilder.Environment;
        _properties = hostApplicationBuilder.Properties;
        _metrics = hostApplicationBuilder.Metrics;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegramBotHostBuilder"/> class.
    /// </summary>
    /// <param name="handlers"></param>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="logging"></param>
    /// <param name="environment"></param>
    /// <param name="properties"></param>
    /// <param name="metrics"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public TelegramBotHostBuilder(IHandlersCollection handlers, IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging, IHostEnvironment environment, IDictionary<object, object> properties, IMetricsBuilder metrics)
    {
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logging = logging ?? throw new ArgumentNullException(nameof(logging));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _properties = properties ?? throw new ArgumentNullException(nameof(properties));
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }
}
