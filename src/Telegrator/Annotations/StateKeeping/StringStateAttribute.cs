using Telegrator.StateKeeping;
using Telegrator.Attributes;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Annotations.StateKeeping
{
    /// <summary>
    /// Attribute for associating a handler or method with a string-based state keeper.
    /// Provides various constructors for flexible state and key resolver configuration.
    /// </summary>
    public class StringStateAttribute : StateKeeperAttribute<long, string, StringStateKeeper>
    {
        /// <summary>
        /// Initializes the attribute with a special state and a custom key resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public StringStateAttribute(SpecialState specialState, IStateKeyResolver<long> keyResolver)
            : base(specialState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a specific state and a custom key resolver.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        /// <param name="keyResolver">The key resolver for state keeping</param>
        public StringStateAttribute(string myState, IStateKeyResolver<long> keyResolver)
            : base(myState, keyResolver) { }

        /// <summary>
        /// Initializes the attribute with a special state and the default sender ID resolver.
        /// </summary>
        /// <param name="specialState">The special state to associate</param>
        public StringStateAttribute(SpecialState specialState)
            : base(specialState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes the attribute with a specific state and the default sender ID resolver.
        /// </summary>
        /// <param name="myState">The string state to associate</param>
        public StringStateAttribute(string myState)
            : base(myState, new SenderIdResolver()) { }
    }
}
