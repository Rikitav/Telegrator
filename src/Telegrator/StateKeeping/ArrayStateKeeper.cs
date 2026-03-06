using Telegrator.StateKeeping.Components;

namespace Telegrator.StateKeeping
{
    /// <summary>
    /// Abstract base class for state keepers that manage state transitions using an array of predefined states.
    /// Provides forward and backward navigation through a fixed sequence of states.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used to identify state contexts.</typeparam>
    /// <typeparam name="TState">The type of the state values. Must be non-null.</typeparam>
    /// <param name="states">The array of states that define the allowed state sequence.</param>
    public abstract class ArrayStateKeeper<TKey, TState>(params TState[] states) : StateKeeperBase<TKey, TState> where TState : notnull where TKey : notnull
    {
        /// <summary>
        /// The array of states that defines the allowed state sequence for navigation.
        /// </summary>
        protected readonly TState[] ArrayStates = states;

        /// <summary>
        /// Moves to the previous state in the array sequence.
        /// </summary>
        /// <param name="currentState">The current state to move backward from.</param>
        /// <param name="_">The key parameter (unused in this implementation).</param>
        /// <returns>The previous state in the array sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when the current state is not found in the array.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when trying to move backward from the first state.</exception>
        protected override TState MoveBackward(TState currentState, TKey _)
        {
            int index = Array.IndexOf(ArrayStates, currentState);
            if (index == -1)
                throw new ArgumentException("Cannot resolve current state");

            if (index == 0)
                throw new IndexOutOfRangeException("This state cannot be moved backward");

            return ArrayStates[index - 1];
        }

        /// <summary>
        /// Moves to the next state in the array sequence.
        /// </summary>
        /// <param name="currentState">The current state to move forward from.</param>
        /// <param name="_">The key parameter (unused in this implementation).</param>
        /// <returns>The next state in the array sequence.</returns>
        /// <exception cref="ArgumentException">Thrown when the current state is not found in the array.</exception>
        /// <exception cref="IndexOutOfRangeException">Thrown when trying to move forward from the last state.</exception>
        protected override TState MoveForward(TState currentState, TKey _)
        {
            int index = Array.IndexOf(ArrayStates, currentState);
            if (index == -1)
                throw new ArgumentException("Cannot resolve current state");

            if (index == ArrayStates.Length - 1)
                throw new IndexOutOfRangeException("This state cannot be moved forward");

            return ArrayStates[index + 1];
        }
    }
}
