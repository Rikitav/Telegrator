/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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