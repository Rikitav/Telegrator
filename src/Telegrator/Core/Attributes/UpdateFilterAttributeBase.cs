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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Core.Attributes;

/// <summary>
/// Defines the <see cref="IFilter{T}"/> to <see cref="Update"/> validation for entry into execution of the <see cref="UpdateHandlerBase"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public abstract class UpdateFilterAttributeBase : Attribute
{
    /// <summary>
    /// Gets the <see cref="UpdateType"/>'s that <see cref="UpdateHandlerBase"/> processing
    /// </summary>
    public abstract UpdateType[] AllowedTypes { get; }

    /// <summary>
    /// Gets the <see cref="IFilter{T}"/> that <see cref="UpdateHandlerBase"/> processing
    /// </summary>
    public abstract IFilter<Update> AnonymousFilter { get; protected set; }

    /// <summary>
    /// Gets or sets the filter modifiers that affect how this filter is combined with others.
    /// </summary>
    public FilterModifier Modifiers { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="UpdateHandlerAttributeBase"/>
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    protected internal UpdateFilterAttributeBase()
    {
        if (AllowedTypes.Length == 0)
            throw new ArgumentException("UpdateFilterAttributeBase must have at least one allowed UpdateType.");
    }

    /// <summary>
    /// Determines the logic of filter modifiers. Exceptionally internal implementation</summary>
    /// <param name="previous"></param>
    /// <returns></returns>
    public abstract bool ProcessModifiers(UpdateFilterAttributeBase? previous);
}
