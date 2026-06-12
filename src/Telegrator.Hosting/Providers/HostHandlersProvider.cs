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

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;

namespace Telegrator.Providers;

/// <inheritdoc/>
public class HostHandlersProvider : HandlersProvider
{
    private readonly IServiceProvider Services;

    /// <inheritdoc/>
    public HostHandlersProvider(
        IHandlersCollection handlers,
        IOptions<TelegratorOptions> options,
        IServiceProvider serviceProvider) : base(handlers, options.Value)
    {
        Services = serviceProvider;
    }

    /// <inheritdoc/>
    public override UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IServiceScope scope = Services.CreateScope();

        object handlerInstance = descriptor.ServiceKey == null
            ? scope.ServiceProvider.GetRequiredService(descriptor.HandlerType)
            : scope.ServiceProvider.GetRequiredKeyedService(descriptor.HandlerType, descriptor.ServiceKey);

        if (handlerInstance is not UpdateHandlerBase updateHandler)
            throw new InvalidOperationException("Failed to resolve " + descriptor.HandlerType + " as UpdateHandlerBase");

        descriptor.LazyInitialization?.Invoke(updateHandler);
        updateHandler.LifetimeToken.OnLifetimeEnded += _ => scope.Dispose();
        return updateHandler;
    }
}
