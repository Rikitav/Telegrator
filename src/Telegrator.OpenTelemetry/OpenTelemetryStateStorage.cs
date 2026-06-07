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

using System.Diagnostics;
using Telegrator.Core.States;

namespace Telegrator.OpenTelemetry;

/// <summary>
/// Decorates an <see cref="IStateStorage"/> with OpenTelemetry tracing and metrics.
/// </summary>
public sealed class OpenTelemetryStateStorage : IStateStorage
{
    private readonly IStateStorage _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryStateStorage"/> class.
    /// </summary>
    /// <param name="inner">The underlying state storage to decorate.</param>
    public OpenTelemetryStateStorage(IStateStorage inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    /// <inheritdoc />
    public Task DeleteAsync(string key, CancellationToken cancellation = default)
        => WrapAsync("delete", () => _inner.DeleteAsync(key, cancellation));

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellation = default)
        => WrapAsync("get", () => _inner.GetAsync<T>(key, cancellation));

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, CancellationToken cancellation = default)
        => WrapAsync("set", () => _inner.SetAsync(key, value, cancellation));

    private static async Task WrapAsync(string operation, Func<Task> action)
    {
        Activity? activity = TelegratorActivitySource.StartStateActivity(operation);
        TelegratorMetrics.StateOperations.Add(1, new KeyValuePair<string, object?>("telegrator.state.operation", operation));

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddTag("exception.message", ex.Message);
            activity?.AddTag("exception.stacktrace", ex.ToString());
            throw;
        }
        finally
        {
            activity?.Dispose();
        }
    }

    private static async Task<T> WrapAsync<T>(string operation, Func<Task<T>> action)
    {
        Activity? activity = TelegratorActivitySource.StartStateActivity(operation);
        TelegratorMetrics.StateOperations.Add(1, new KeyValuePair<string, object?>("telegrator.state.operation", operation));

        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddTag("exception.message", ex.Message);
            activity?.AddTag("exception.stacktrace", ex.ToString());
            throw;
        }
        finally
        {
            activity?.Dispose();
        }
    }
}
