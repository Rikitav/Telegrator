namespace Telegrator.Core.States;

/// <summary>
/// Defines a contract for a state machine that manages transitions and retrieves states for specific updates.
/// </summary>
/// <typeparam name="TState">The type of the state. Must implement <see cref="IEquatable{T}"/>.</typeparam>
public interface IStateMachine<TState> where TState : IEquatable<TState>
{
    /// <summary>
    /// Gets the current state associated with the specified update key.
    /// </summary>
    /// <param name="storage">The storage mechanism used to persist the state.</param>
    /// <param name="updateKey">The unique key identifying the current update context (e.g., chat and user ID).</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the current state, or the default value if no state is found.</returns>
    Task<TState?> Current(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Advances the state machine to the next state in the sequence.
    /// </summary>
    /// <param name="storage">The storage mechanism used to persist the state.</param>
    /// <param name="updateKey">The unique key identifying the current update context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous transition operation.</returns>
    Task Advance(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves the state machine backward to the previous state in the sequence.
    /// </summary>
    /// <param name="storage">The storage mechanism used to persist the state.</param>
    /// <param name="updateKey">The unique key identifying the current update context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous transition operation.</returns>
    Task Retreat(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets the state machine to its initial or default state.
    /// </summary>
    /// <param name="storage">The storage mechanism used to persist the state.</param>
    /// <param name="updateKey">The unique key identifying the current update context.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous reset operation.</returns>
    Task Reset(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);
}