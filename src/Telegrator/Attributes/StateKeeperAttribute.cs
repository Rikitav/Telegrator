using Telegram.Bot.Types;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Abstract attribute for associating a handler or method with a state keeper.
    /// Provides logic for state-based filtering and state management.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used for state keeping (e.g., chat ID).</typeparam>
    /// <typeparam name="TState">The type of the state value (e.g., string, int).</typeparam>
    /// <typeparam name="TKeeper">The type of the state keeper implementation.</typeparam>
    public abstract class StateKeeperAttribute<TKey, TState, TKeeper> : StateKeeperAttributeBase where TKey : notnull where TState : notnull where TKeeper : StateKeeperBase<TKey, TState>, new()
    {
        /*
        private static readonly TKeeper _shared = new TKeeper();
        private static readonly Dictionary<TKey, TKeeper> _keyed = [];
        */

        /// <summary>
        /// Gets or sets the singleton instance of the state keeper for this attribute type.
        /// </summary>
        public static TKeeper Shared { get; } = new TKeeper();

        /// <summary>
        /// Gets the default state value of this statekeeper.
        /// </summary>
        public static TState DefaultState => Shared.DefaultState;

        /// <summary>
        /// Gets the state value associated with this attribute instance.
        /// </summary>
        public TState MyState { get; private set; }

        /// <summary>
        /// Gets the special state mode for this attribute instance.
        /// </summary>
        public SpecialState SpecialState { get; private set; }

        /// <summary>
        /// Initializes the attribute with a specific state and a custom key resolver.
        /// </summary>
        /// <param name="myState">The state value to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        protected StateKeeperAttribute(TState myState, IStateKeyResolver<TKey> keyResolver) : base(typeof(TKeeper))
        {
            Shared.KeyResolver = keyResolver;
            MyState = myState;
            SpecialState = SpecialState.None;
        }

        /// <summary>
        /// Initializes the attribute with a special state and a custom key resolver.
        /// </summary>
        /// <param name="specialState">The special state mode</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        protected StateKeeperAttribute(SpecialState specialState, IStateKeyResolver<TKey> keyResolver) : base(typeof(TKeeper))
        {
            Shared.KeyResolver = keyResolver;
            MyState = Shared.DefaultState;
            SpecialState = specialState;
        }

        /// <summary>
        /// Determines whether the current update context passes the state filter.
        /// </summary>
        /// <param name="context">The filter execution context</param>
        /// <returns>True if the state matches the filter; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context)
        {
            if (SpecialState == SpecialState.AnyState)
                return true;

            if (!Shared.TryGetState(context.Input, out TState? state))
                return SpecialState == SpecialState.NoState;

            if (state == null)
                return false;

            return MyState.Equals(state);
        }
    }
}
