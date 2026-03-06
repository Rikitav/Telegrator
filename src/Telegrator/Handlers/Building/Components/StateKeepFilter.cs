using Telegram.Bot.Types;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Filters;
using Telegrator.Filters.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Filter for state keeping logic, allowing filtering based on state and special state conditions.
    /// </summary>
    /// <typeparam name="TKey">The type of the key for state resolution.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
    public class StateKeepFilter<TKey, TState, TKeeper> : Filter<Update>
        where TKey : notnull
        where TState : IEquatable<TState>
        where TKeeper : StateKeeperBase<TKey, TState>, new()
    {
        /// <summary>
        /// Gets or sets the state keeper instance.
        /// </summary>
        public static TKeeper StateKeeper { get; internal set; } = null!;

        /// <summary>
        /// Gets the state value for this filter.
        /// </summary>
        public TState MyState { get; private set; }

        /// <summary>
        /// Gets the special state value for this filter.
        /// </summary>
        public SpecialState SpecialState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateKeepFilter{TKey, TState, TKeeper}"/> class with a specific state.
        /// </summary>
        /// <param name="myState">The state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        public StateKeepFilter(TState myState, IStateKeyResolver<TKey> keyResolver)
        {
            StateKeeper ??= new TKeeper();
            StateKeeper.KeyResolver = keyResolver;
            MyState = myState;
            SpecialState = SpecialState.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateKeepFilter{TKey, TState, TKeeper}"/> class with a special state.
        /// </summary>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        public StateKeepFilter(SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
        {
            StateKeeper ??= new TKeeper();
            StateKeeper.KeyResolver = keyResolver;
            MyState = StateKeeper.DefaultState;
            SpecialState = specialState;
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context based on state logic.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context)
        {
            if (SpecialState == SpecialState.AnyState)
                return true;

            if (!StateKeeper.TryGetState(context.Input, out TState? state))
                return SpecialState == SpecialState.NoState;

            if (state == null)
                return false;

            return MyState.Equals(state);
        }
    }
}
