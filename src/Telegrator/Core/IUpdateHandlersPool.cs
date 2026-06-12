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

using Telegrator.Core.Descriptors;

namespace Telegrator.Core;

/// <summary>
/// Represents a delegate for when a handler is enqueued.
/// </summary>
/// <param name="args">The <see cref="DescribedHandlerDescriptor"/> for the enqueued handler.</param>
public delegate void HandlerEnqueued(DescribedHandlerDescriptor args);

/// <summary>
/// Represents a delegate for when a handler is executing.
/// </summary>
/// <param name="args">The <see cref="DescribedHandlerDescriptor"/> for the executing handler.</param>
public delegate void HandlerExecuting(DescribedHandlerDescriptor args);

/// <summary>
/// Provides a pool for managing the execution and queuing of update handlers.
/// </summary>
public interface IUpdateHandlersPool : IDisposable
{
    /// <summary>
    /// Occurs when a handler is enqueued.
    /// </summary>
    public event HandlerEnqueued? HandlerEnqueued;

    /// <summary>
    /// Occurs when a handler is entering execution.
    /// </summary>
    public event HandlerExecuting? HandlerExecuting;

    /// <summary>
    /// Enqueues a collection of handlers for execution.
    /// </summary>
    /// <param name="handlers">The handlers to enqueue.</param>
    public Task Enqueue(params IEnumerable<DescribedHandlerDescriptor> handlers);
}
