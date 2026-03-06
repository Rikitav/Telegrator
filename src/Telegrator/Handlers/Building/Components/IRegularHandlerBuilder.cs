using Telegrator.Handlers.Building;
using Telegrator.MadiatorCore;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Defines a builder for regular handler logic for a specific update type.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
    public interface IRegularHandlerBuilder<TUpdate> : IHandlerBuilder where TUpdate : class
    {
        /// <summary>
        /// Builds the handler logic using the specified execution delegate.
        /// </summary>
        /// <param name="executeHandler">The delegate to execute the handler logic.</param>
        public IHandlersCollection Build(AbstractHandlerAction<TUpdate> executeHandler);
    }
}
