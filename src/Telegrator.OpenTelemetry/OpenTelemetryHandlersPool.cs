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

using System.Diagnostics;
using Telegrator.Core;
using Telegrator.Core.Descriptors;

namespace Telegrator.OpenTelemetry;

/// <summary>
/// Decorates an <see cref="IUpdateHandlersPool"/> to emit OpenTelemetry metrics
/// for handler execution duration, success, and errors.
/// </summary>
public sealed class OpenTelemetryHandlersPool : IUpdateHandlersPool
{
    private readonly IUpdateHandlersPool _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryHandlersPool"/> class.
    /// </summary>
    /// <param name="inner">The underlying handlers pool to decorate.</param>
    public OpenTelemetryHandlersPool(IUpdateHandlersPool inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _inner.HandlerExecuting += OnHandlerExecuting;
    }

    /// <inheritdoc />
    public event HandlerEnqueued? HandlerEnqueued
    {
        add => _inner.HandlerEnqueued += value;
        remove => _inner.HandlerEnqueued -= value;
    }

    /// <inheritdoc />
    public event HandlerExecuting? HandlerExecuting
    {
        add => _inner.HandlerExecuting += value;
        remove => _inner.HandlerExecuting -= value;
    }

    /// <inheritdoc />
    public Task Enqueue(params IEnumerable<DescribedHandlerDescriptor> handlers)
        => _inner.Enqueue(handlers);

    /// <inheritdoc />
    public void Dispose()
    {
        _inner.HandlerExecuting -= OnHandlerExecuting;
        _inner.Dispose();
    }

    private static void OnHandlerExecuting(DescribedHandlerDescriptor descriptor)
    {
        _ = TrackHandlerAsync(descriptor);
    }

    private static async Task TrackHandlerAsync(DescribedHandlerDescriptor descriptor)
    {
        Stopwatch sw = Stopwatch.StartNew();
        bool success = false;

        try
        {
            await descriptor.AwaitResult(CancellationToken.None);
            success = descriptor.Result?.Success ?? false;
        }
        catch
        {
            success = false;
        }
        finally
        {
            sw.Stop();
            TelegratorMetrics.RecordHandlerExecuted(success, sw.Elapsed);
        }
    }
}
