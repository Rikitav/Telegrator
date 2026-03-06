using Telegrator.Handlers.Components;

namespace Telegrator.Aspects
{
    /// <summary>
    /// Interface for post-execution processors that are executed after handler execution.
    /// Implement this interface to add cross-cutting concerns like logging, cleanup, or metrics collection.
    /// </summary>
    public interface IPostProcessor
    {
        /// <summary>
        /// Executes after the handler's main execution logic.
        /// </summary>
        /// <param name="container">The handler container containing the current update and context.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A <see cref="Result"/> indicating the final execution result.</returns>
        public Task<Result> AfterExecution(IHandlerContainer container, CancellationToken cancellationToken);
    }
}
