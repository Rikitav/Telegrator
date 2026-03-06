using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.StateKeeping
{
    /// <summary>
    /// State keeper that manages numeric (integer) states for chat sessions.
    /// Inherits from <see cref="StateKeeperBase{TKey, TState}"/> with long keys and int states.
    /// Provides automatic increment/decrement functionality for state transitions.
    /// </summary>
    public class NumericStateKeeper : StateKeeperBase<long, int>
    {
        /// <summary>
        /// Gets the default state value, which is 1.
        /// </summary>
        public override int DefaultState => 1;

        /// <summary>
        /// Moves the numeric state backward by decrementing the current state value.
        /// </summary>
        /// <param name="currentState">The current numeric state value</param>
        /// <param name="_">The chat ID (unused in this implementation)</param>
        /// <returns>The decremented state value</returns>
        protected override int MoveBackward(int currentState, long _)
        {
            return currentState - 1;
        }

        /// <summary>
        /// Moves the numeric state forward by incrementing the current state value.
        /// </summary>
        /// <param name="currentState">The current numeric state value</param>
        /// <param name="_">The chat ID (unused in this implementation)</param>
        /// <returns>The incremented state value</returns>
        protected override int MoveForward(int currentState, long _)
        {
            return currentState + 1;
        }
    }

    /// <summary>
    /// Provides extension methods for managing numeric states in handler containers.
    /// </summary>
    public static partial class StateHandlerContainerExtensions
    {
        /// <summary>
        /// Gets the numeric state keeper instance associated with the handler container.
        /// </summary>
        /// <param name="_">The handler container instance</param>
        /// <returns>The <see cref="NumericStateKeeper"/> instance</returns>
        public static NumericStateKeeper NumericStateKeeper(this IHandlerContainer _)
            => NumericStateAttribute.Shared;

        /// <summary>
        /// Creates a new numeric state for the current update being handled.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void CreateNumericState(this IHandlerContainer container)
            => container.NumericStateKeeper().CreateState(container.HandlingUpdate);

        /// <summary>
        /// Deletes the numeric state for the current update being handled.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void DeleteNumericState(this IHandlerContainer container)
            => container.NumericStateKeeper().DeleteState(container.HandlingUpdate);

        /// <summary>
        /// Sets the numeric state for the current update being handled.
        /// If the new state is null, uses the default state from the state keeper.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        /// <param name="newState">The new numeric state to set, or null to use default</param>
        public static void SetNumericState(this IHandlerContainer container, int? newState)
            => container.NumericStateKeeper().SetState(container.HandlingUpdate, newState ?? NumericStateAttribute.DefaultState);

        /// <summary>
        /// Moves the numeric state forward by incrementing the current value.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void ForwardNumericState(this IHandlerContainer container)
            => container.NumericStateKeeper().MoveForward(container.HandlingUpdate);

        /// <summary>
        /// Moves the numeric state backward by decrementing the current value.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void BackwardNumericState(this IHandlerContainer container)
            => container.NumericStateKeeper().MoveBackward(container.HandlingUpdate);
    }
}
