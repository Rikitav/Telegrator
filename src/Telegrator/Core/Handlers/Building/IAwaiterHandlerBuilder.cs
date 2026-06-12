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

using Telegrator.Core.States;

namespace Telegrator.Core.Handlers.Building;

/// <summary>
/// Defines a builder for awaiting handler logic for a specific update type.
/// </summary>
/// <typeparam name="TUpdate">The type of update to await.</typeparam>
public interface IAwaiterHandlerBuilder<TUpdate> : IHandlerBuilder where TUpdate : class
{
    /// <summary>
    /// Awaits an update using the specified key resolver and cancellation token.
    /// </summary>
    /// <param name="keyResolver">The <see cref="IStateKeyResolver"/> to resolve the key.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task{TUpdate}"/> representing the awaited update, or <see langword="null"/> if the operation does not yield an update.</returns>
    public Task<TUpdate?> Await(IStateKeyResolver keyResolver, CancellationToken cancellationToken = default);
}
