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

using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;

namespace Telegrator.Core;

/// <summary>
/// Provides methods to retrieve and describe handler information for updates.
/// </summary>
public interface IHandlersProvider
{
    /// <summary>
    /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
    /// </summary>
    public IEnumerable<UpdateType> AllowedTypes { get; }

    /// <summary>
    /// Gets the collection of <see cref="UpdateType"/> keys for the handler lists.
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list);

    /// <summary>
    /// Instantiates a handler for the given descriptor, using the appropriate creation strategy based on descriptor type.
    /// Supports singleton, implicit, keyed, and general descriptor types with different instantiation patterns.
    /// </summary>
    /// <param name="descriptor">The handler descriptor.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>An instance of <see cref="UpdateHandlerBase"/> for the descriptor</returns>
    public UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the provider contains any handlers.
    /// </summary>
    /// <returns>True if the provider is empty; otherwise, false.</returns>
    public bool IsEmpty();
}
