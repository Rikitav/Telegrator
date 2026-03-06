using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers.Components;

namespace Telegrator.StateKeeping
{
    /// <summary>
    /// State keeper implementation for enum-based states.
    /// Automatically creates an array of all enum values for state navigation.
    /// </summary>
    /// <typeparam name="TEnum">The enum type to be used for state management.</typeparam>
    public class EnumStateKeeper<TEnum>() : ArrayStateKeeper<long, TEnum>(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray()) where TEnum : Enum
    {
        /// <summary>
        /// Gets the default state, which is the first value in the enum.
        /// </summary>
        public override TEnum DefaultState => ArrayStates.ElementAt(0);
    }

    /// <summary>
    /// Extension methods for working with enum-based states in handler containers.
    /// Provides convenient methods for state management operations.
    /// </summary>
    public static partial class StateHandlerContainerExtensions
    {
        /// <summary>
        /// Gets the enum state keeper for the specified enum type.
        /// </summary>
        /// <typeparam name="TEnum">The enum type to get the state keeper for.</typeparam>
        /// <param name="_">The handler container (unused parameter for extension method syntax).</param>
        /// <returns>The enum state keeper instance.</returns>
        public static EnumStateKeeper<TEnum> EnumStateKeeper<TEnum>(this IHandlerContainer _) where TEnum : Enum
            => EnumStateAttribute<TEnum>.Shared;

        /// <summary>
        /// Creates a new enum state for the current update.
        /// </summary>
        /// <typeparam name="TEnum">The enum type for state management.</typeparam>
        /// <param name="container">The handler container.</param>
        public static void CreateEnumState<TEnum>(this IHandlerContainer container) where TEnum : Enum
            => container.EnumStateKeeper<TEnum>().CreateState(container.HandlingUpdate);

        /// <summary>
        /// Deletes the enum state for the current update.
        /// </summary>
        /// <typeparam name="TEnum">The enum type for state management.</typeparam>
        /// <param name="container">The handler container.</param>
        public static void DeleteEnumState<TEnum>(this IHandlerContainer container) where TEnum : Enum
            => container.EnumStateKeeper<TEnum>().DeleteState(container.HandlingUpdate);

        /// <summary>
        /// Sets the enum state to a specific value for the current update.
        /// </summary>
        /// <typeparam name="TEnum">The enum type for state management.</typeparam>
        /// <param name="container">The handler container.</param>
        /// <param name="newState">The new state value. If null, uses the default state.</param>
        public static void SetEnumState<TEnum>(this IHandlerContainer container, TEnum? newState) where TEnum : Enum
            => container.EnumStateKeeper<TEnum>().SetState(container.HandlingUpdate, newState ?? EnumStateAttribute<TEnum>.DefaultState);

        /// <summary>
        /// Moves the enum state forward to the next value in the enum sequence.
        /// </summary>
        /// <typeparam name="TEnum">The enum type for state management.</typeparam>
        /// <param name="container">The handler container.</param>
        public static void ForwardEnumState<TEnum>(this IHandlerContainer container) where TEnum : Enum
            => container.EnumStateKeeper<TEnum>().MoveForward(container.HandlingUpdate);

        /// <summary>
        /// Moves the enum state backward to the previous value in the enum sequence.
        /// </summary>
        /// <typeparam name="TEnum">The enum type for state management.</typeparam>
        /// <param name="container">The handler container.</param>
        public static void BackwardEnumState<TEnum>(this IHandlerContainer container) where TEnum : Enum
            => container.EnumStateKeeper<TEnum>().MoveBackward(container.HandlingUpdate);
    }
}
