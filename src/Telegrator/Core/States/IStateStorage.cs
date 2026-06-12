/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Telegrator.Core.States;

/// <summary>
/// Defines a contract for an asynchronous state storage mechanism.
/// </summary>
public interface IStateStorage
{
    /// <summary>
    /// Saves or updates a state value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the state object.</typeparam>
    /// <param name="key">The unique identifier for the state.</param>
    /// <param name="state">The state object to store.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SetAsync<T>(string key, T state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a state value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the state object to retrieve.</typeparam>
    /// <param name="key">The unique identifier for the state.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous retrieve operation. The task result contains the state object if found; otherwise, the default value of <typeparamref name="T"/>.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the state value associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the state to remove.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}