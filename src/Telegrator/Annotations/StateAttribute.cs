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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.States;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute for filtering updates where resolved state matches target value.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
/// <param name="value"></param>
public class StateAttribute<TKey, TValue>(TValue? value) : UpdateFilterAttribute<Update>(new StateKeyFilter<TKey, TValue>(value))
    where TKey : IStateKeyResolver, new()
    where TValue : IEquatable<TValue>
{
    /// <summary>
    /// The targetting state value.
    /// </summary>
    public TValue? Value => value;

    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => Update.AllTypes;

    /// <inheritdoc/>
    public override Update? GetFilterringTarget(Update update)
    {
        return update;
    }
}
