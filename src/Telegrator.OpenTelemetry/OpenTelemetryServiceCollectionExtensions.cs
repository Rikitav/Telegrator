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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Telegrator.Core;
using Telegrator.Core.States;

namespace Telegrator.OpenTelemetry;

/// <summary>
/// Provides extension methods for registering Telegrator OpenTelemetry instrumentation.
/// </summary>
public static class OpenTelemetryServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenTelemetry metrics and tracing instrumentation for the Telegrator framework.
    /// Decorates <see cref="IUpdateHandlersPool"/>, <see cref="IAwaitingProvider"/>, and <see cref="IStateStorage"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTelegratorOpenTelemetry(this IServiceCollection services)
    {
        services.TryDecorate<IUpdateHandlersPool, OpenTelemetryHandlersPool>();
        services.TryDecorate<IAwaitingProvider, OpenTelemetryAwaitingProvider>();
        services.TryDecorate<IStateStorage, OpenTelemetryStateStorage>();

        return services;
    }
}
