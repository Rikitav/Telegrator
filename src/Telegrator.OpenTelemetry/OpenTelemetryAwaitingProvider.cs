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
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;
using Telegrator.Core.Handlers.Building;

namespace Telegrator.OpenTelemetry;

/// <summary>
/// Decorates an <see cref="IAwaitingProvider"/> with OpenTelemetry tracing and metrics.
/// </summary>
public sealed class OpenTelemetryAwaitingProvider : IAwaitingProvider
{
    private readonly IAwaitingProvider _inner;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenTelemetryAwaitingProvider"/> class.
    /// </summary>
    /// <param name="inner">The underlying provider to decorate.</param>
    public OpenTelemetryAwaitingProvider(IAwaitingProvider inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    /// <inheritdoc />
    public IEnumerable<UpdateType> AllowedTypes => _inner.AllowedTypes;

    /// <inheritdoc />
    public bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
        => _inner.TryGetDescriptorList(updateType, out list);

    /// <inheritdoc />
    public UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
        => _inner.GetHandlerInstance(descriptor, cancellationToken);

    /// <inheritdoc />
    public bool IsEmpty() => _inner.IsEmpty();

    /// <inheritdoc />
    public IDisposable UseHandler(HandlerDescriptor handlerDescriptor)
    {
        Activity? activity = TelegratorActivitySource.StartAwaitActivity(nameof(UseHandler));
        activity?.SetTag("telegrator.await.handler", handlerDescriptor.HandlerType.Name);

        TelegratorMetrics.AwaitingActive.Add(1);

        var disposable = _inner.UseHandler(handlerDescriptor);
        return new DisposeAction(() =>
        {
            disposable.Dispose();
            TelegratorMetrics.AwaitingActive.Add(-1);
            activity?.Dispose();
        });
    }

    private sealed class DisposeAction : IDisposable
    {
        private readonly Action _action;
        private bool _disposed;

        public DisposeAction(Action action) => _action = action;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _action();
        }
    }
}
