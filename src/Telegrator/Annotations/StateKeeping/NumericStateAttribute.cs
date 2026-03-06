using Telegrator.StateKeeping;
using Telegrator.Attributes;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Annotations.StateKeeping
{
    /// <summary>
    /// Attribute for associating a handler or method with a numeric (integer) state keeper.
    /// Provides constructors for flexible state and key resolver configuration.
    /// </summary>
    public class NumericStateAttribute : StateKeeperAttribute<long, int, NumericStateKeeper>
    {
        /// <summary>
        /// Initializes the attribute with a special state and a custom key resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public NumericStateAttribute(SpecialState specialState, IStateKeyResolver<long> keyResolver)
            : base(specialState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a specific numeric state and a custom key resolver.
        /// </summary>
        /// <param name="myState">The integer state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public NumericStateAttribute(int myState, IStateKeyResolver<long> keyResolver)
            : base(myState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a special state and the default sender ID resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        public NumericStateAttribute(SpecialState specialState)
            : base(specialState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes the attribute with a specific numeric state and the default sender ID resolver.
        /// </summary>
        /// <param name="myState">The integer state to associate</param>
        public NumericStateAttribute(int myState)
            : this(myState, new SenderIdResolver()) { }
    }
}
