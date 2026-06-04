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

using Telegrator.Core.States;

namespace Telegrator.States;

/// <summary>
/// State machine implementation for enum-based states.
/// Automatically creates an array of all enum values for state navigation.
/// </summary>
/// <typeparam name="TEnum">The enum type to be used for state management.</typeparam>
public class EnumStateMachine<TEnum> : IStateMachine<TEnum> where TEnum : struct, Enum, IEquatable<TEnum>
{
    private readonly TEnum[] _states = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
    private TEnum DefaultState => _states.FirstOrDefault();

    /// <inheritdoc/>
    public async Task<TEnum> Current(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        TEnum state = await storage.GetAsync<TEnum>(key, cancellationToken);

        return EqualityComparer<TEnum>.Default.Equals(state, default)
            ? DefaultState : state;
    }

    /// <inheritdoc/>
    public async Task Advance(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        TEnum currentState = await storage.GetAsync<TEnum>(key, cancellationToken);

        int currentIndex = Array.IndexOf(_states, currentState);
        if (currentIndex < _states.Length - 1)
        {
            TEnum nextState = _states[currentIndex + 1];
            await storage.SetAsync(key, nextState, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Retreat(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        TEnum currentState = await storage.GetAsync<TEnum>(key, cancellationToken);

        int currentIndex = Array.IndexOf(_states, currentState);
        if (currentIndex > 0)
        {
            TEnum nextState = _states[currentIndex - 1];
            await storage.SetAsync(key, nextState, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Reset(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        await storage.SetAsync(key, DefaultState, cancellationToken);
    }

    private static string FormatKey(string updateKey)
        => typeof(TEnum).Name + ":" + updateKey;
}
