using Telegrator.StateKeeping.Components;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Defines a builder for awaiting handler logic for a specific update type.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to await.</typeparam>
    public interface IAwaiterHandlerBuilder<TUpdate> : IHandlerBuilder where TUpdate : class
    {
        /// <summary>
        /// Awaits an update using the specified key resolver and cancellation token.
        /// </summary>
        /// <param name="keyResolver">The <see cref="IStateKeyResolver{TKey}"/> to resolve the key.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A <see cref="Task{TUpdate}"/> representing the awaited update.</returns>
        public Task<TUpdate> Await(IStateKeyResolver<long> keyResolver, CancellationToken cancellationToken = default);
    }
}
