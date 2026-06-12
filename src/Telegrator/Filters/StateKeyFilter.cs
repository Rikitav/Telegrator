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
using Telegrator.Core.Filters;
using Telegrator.Core.States;

namespace Telegrator.Filters;

/// <summary>
/// Filters updates by comparing a resolved state key with a target key.
/// </summary>
/// <typeparam name="TKey">The type of the key resolver used to get state key.</typeparam>
/// <typeparam name="TValue">The type of the key used for state resolution.</typeparam>
public class StateKeyFilter<TKey, TValue> : Filter<Update>
    where TKey : IStateKeyResolver, new()
    where TValue : IEquatable<TValue>
{
    private readonly TValue? TargetKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateKeyFilter{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="targetKey">The target key to compare with.</param>
    public StateKeyFilter(TValue? targetKey)
    {
        TargetKey = targetKey;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context)
    {
        string? key = new TKey().ResolveKey(context.Input);
        if (key is null)
            return TargetKey is null;

        TValue? value = context.UpdateRouter.StateStorage.GetAsync<TValue>(key).Result;
        if (value is null)
            return TargetKey is null;

        if (TargetKey is null)
            return false;

        return TargetKey.Equals(value);
    }
}
