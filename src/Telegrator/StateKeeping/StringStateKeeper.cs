using Telegrator.Annotations.StateKeeping;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.StateKeeping
{
    /// <summary>
    /// State keeper that manages string-based states for chat sessions.
    /// </summary>
    public class StringStateKeeper() : StateKeeperBase<long, string>()
    {
        /// <summary>
        /// Gets the default state value, which is an empty string.
        /// </summary>
        public override string DefaultState => string.Empty;

        /// <inheritdoc/>
        protected override string MoveBackward(string currentState, long currentKey)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override string MoveForward(string currentState, long currentKey)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Provides extension methods for managing string states in handler containers.
    /// </summary>
    public static partial class StateHandlerContainerExtensions
    {
        /// <summary>
        /// Gets the string state keeper instance associated with the handler container.
        /// </summary>
        /// <param name="_">The handler container instance</param>
        /// <returns>The <see cref="StringStateKeeper"/> instance</returns>
        public static StringStateKeeper StringStateKeeper(this IHandlerContainer _)
            => StringStateAttribute.Shared;

        /// <summary>
        /// Creates a new string state for the current update being handled.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void CreateStringState(this IHandlerContainer container)
            => container.StringStateKeeper().CreateState(container.HandlingUpdate);

        /// <summary>
        /// Deletes the string state for the current update being handled.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void DeleteStringState(this IHandlerContainer container)
            => container.StringStateKeeper().DeleteState(container.HandlingUpdate);

        /// <summary>
        /// Sets the string state for the current update being handled.
        /// If the new state is null, uses the default state from the state keeper.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        /// <param name="newState">The new string state to set, or null to use default</param>
        public static void SetStringState(this IHandlerContainer container, string? newState)
            => container.StringStateKeeper().SetState(container.HandlingUpdate, newState ?? StringStateAttribute.DefaultState);

        /*
        public static string GetStringState(this IHandlerContainer container, string key)
            => container.StringStateKeeper().GetState()
        */

        /*
        /// <summary>
        /// Moves the string state forward to the next state in the sequence.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void ForwardStringState(this IHandlerContainer container)
            => container.StringStateKeeper().MoveForward(container.HandlingUpdate);

        /// <summary>
        /// Moves the string state backward to the previous state in the sequence.
        /// </summary>
        /// <param name="container">The handler container instance</param>
        public static void BackwardStringState(this IHandlerContainer container)
            => container.StringStateKeeper().MoveBackward(container.HandlingUpdate);
        */
    }
}
