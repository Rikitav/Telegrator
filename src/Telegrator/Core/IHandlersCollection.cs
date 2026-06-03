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

using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;

namespace Telegrator.Core;

/// <summary>
/// Collection class for managing handler descriptors organized by update type.
/// Provides functionality for collecting, adding, and organizing handlers.
/// </summary>
public interface IHandlersCollection
{
    /// <summary>
    /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
    /// </summary>
    public IEnumerable<UpdateType> AllowedTypes { get; }

    /// <summary>
    /// Gets the collection of <see cref="UpdateType"/> keys for the handler lists.
    /// </summary>
    public IEnumerable<UpdateType> Keys { get; }

    /// <summary>
    /// Gets the collection of <see cref="HandlerDescriptorList"/> values.
    /// </summary>
    public IEnumerable<HandlerDescriptorList> Values { get; }

    /// <summary>
    /// Gets the <see cref="HandlerDescriptorList"/> for the specified <see cref="UpdateType"/>.
    /// </summary>
    /// <param name="updateType">The update type key.</param>
    /// <returns>The handler descriptor list for the given update type.</returns>
    public HandlerDescriptorList this[UpdateType updateType] { get; }

    /// <summary>
    /// Tryes to get the <see cref="HandlerDescriptorList"/> for the specified <see cref="UpdateType"/>.
    /// </summary>
    /// <param name="updateType"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    public bool TryGetDescriptorList(UpdateType updateType, [NotNullWhen(true)] out HandlerDescriptorList? list);

    /// <summary>
    /// Creates a class handler descriptor for the specified handler type.
    /// </summary>
    /// <param name="handlerType">The type of the handler.</param>
    /// <param name="precompiledAttributes">Precompiled attributes</param>
    /// <returns>A new <see cref="HandlerDescriptor"/>.</returns>
    public HandlerDescriptor CreateClassDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, Attribute[]? precompiledAttributes = null);

    /// <summary>
    /// Adds a <see cref="HandlerDescriptor"/> to the collection and returns the updated collection.
    /// </summary>
    /// <param name="descriptor">The handler descriptor to add.</param>
    /// <returns>The updated <see cref="IHandlersCollection"/>.</returns>
    public IHandlersCollection AddDescriptor(HandlerDescriptor descriptor);
}
