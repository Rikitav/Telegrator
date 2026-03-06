using Telegrator.Core.Handlers;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Represents a handler container for a specific update type.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update handled by the container.</typeparam>
    public interface IHandlerContainer<TUpdate> : IHandlerContainer where TUpdate : class
    {
        /// <summary>
        /// Gets the actual update object of type <typeparamref name="TUpdate"/>.
        /// </summary>
        public TUpdate ActualUpdate { get; }
    }
}
