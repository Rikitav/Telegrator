using Telegram.Bot.Types;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Filters.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Defines builder actions for configuring handler builders.
    /// </summary>
    public interface IHandlerBuilder
    {
        /// <summary>
        /// Sets the update validating action for the handler.
        /// </summary>
        /// <param name="validateAction">The <see cref="UpdateValidateAction"/> to use.</param>
        /// <returns>The builder instance.</returns>
        public void SetUpdateValidating(UpdateValidateAction validateAction);

        /// <summary>
        /// Sets the concurrency level for the handler.
        /// </summary>
        /// <param name="concurrency">The concurrency value.</param>
        /// <returns>The builder instance.</returns>
        public void SetConcurreny(int concurrency);

        /// <summary>
        /// Sets the priority for the handler.
        /// </summary>
        /// <param name="priority">The priority value.</param>
        /// <returns>The builder instance.</returns>
        public void SetPriority(int priority);

        /// <summary>
        /// Sets both concurrency and priority for the handler.
        /// </summary>
        /// <param name="concurrency">The concurrency value.</param>
        /// <param name="priority">The priority value.</param>
        /// <returns>The builder instance.</returns>
        public void SetIndexer(int concurrency, int priority);

        /// <summary>
        /// Adds a filter to the handler.
        /// </summary>
        /// <param name="filter">The <see cref="IFilter{Update}"/> to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddFilter(IFilter<Update> filter);

        /// <summary>
        /// Adds multiple filters to the handler.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddFilters(params IFilter<Update>[] filters);

        /// <summary>
        /// Sets a state keeper for the handler using a specific state and key resolver.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="myState">The state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <returns>The builder instance.</returns>
        public void SetStateKeeper<TKey, TState, TKeeper>(TState myState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new();

        /// <summary>
        /// Sets a state keeper for the handler using a special state and key resolver.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <returns>The builder instance.</returns>
        public void SetStateKeeper<TKey, TState, TKeeper>(SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new();

        /// <summary>
        /// Adds a targeted filter for a specific filter target type.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filter">The filter to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter)
            where TFilterTarget : class;

        /// <summary>
        /// Adds multiple targeted filters for a specific filter target type.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters)
            where TFilterTarget : class;
    }
}
