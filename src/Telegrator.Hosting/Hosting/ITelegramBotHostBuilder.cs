/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
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
