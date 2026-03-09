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
