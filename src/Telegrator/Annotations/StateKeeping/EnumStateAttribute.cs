using Telegrator.StateKeeping;
using Telegrator.Attributes;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Annotations.StateKeeping
{
    /// <summary>
    /// Attribute for managing enum-based states in Telegram bot handlers.
    /// Provides a convenient way to associate enum values with state management functionality.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used for state management.</typeparam>
    public class EnumStateAttribute<TEnum> : StateKeeperAttribute<long, TEnum, EnumStateKeeper<TEnum>> where TEnum : Enum
    {
        /// <summary>
        /// Initializes a new instance of the EnumStateAttribute with a special state and custom key resolver.
        /// </summary>
        /// <param name="specialState">The special state to be managed.</param>
        /// <param name="keyResolver">The resolver for extracting keys from updates.</param>
        public EnumStateAttribute(SpecialState specialState, IStateKeyResolver<long> keyResolver)
            : base(specialState, keyResolver) { }

        /// <summary>
        /// Initializes a new instance of the EnumStateAttribute with a specific enum state and custom key resolver.
        /// </summary>
        /// <param name="myState">The specific enum state to be managed.</param>
        /// <param name="keyResolver">The resolver for extracting keys from updates.</param>
        public EnumStateAttribute(TEnum myState, IStateKeyResolver<long> keyResolver)
            : base(myState, keyResolver) { }

        /// <summary>
        /// Initializes a new instance of the EnumStateAttribute with a special state and default sender ID resolver.
        /// </summary>
        /// <param name="specialState">The special state to be managed.</param>
        public EnumStateAttribute(SpecialState specialState)
            : base(specialState, new SenderIdResolver()) { }

        /// <summary>
        /// Initializes a new instance of the EnumStateAttribute with a specific enum state and default sender ID resolver.
        /// </summary>
        /// <param name="myState">The specific enum state to be managed.</param>
        public EnumStateAttribute(TEnum myState)
            : this(myState, new SenderIdResolver()) { }
    }
}
