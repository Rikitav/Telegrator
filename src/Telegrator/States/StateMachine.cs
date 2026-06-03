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
using Telegrator.Core.States;

namespace Telegrator.States;

/// <inheritdoc cref="IStateMachine{TState}"/>
public class StateMachine<TMachine, TState>(IStateStorage stateStorage, Update handlingUpdate)
    where TMachine : IStateMachine<TState>, new()
    where TState : IEquatable<TState>
{
    private readonly IStateStorage _stateStorage = stateStorage;
    private readonly Update _handlingUpdate = handlingUpdate;
    private readonly IStateMachine<TState> _stateMachine = new TMachine();

    /// <summary>
    /// Chosen key resolver
    /// </summary>
    public IStateKeyResolver? KeyResolver;

    /// <inheritdoc cref="IStateMachine{TState}.Advance(IStateStorage, string, CancellationToken)"/>
    public async Task Advance(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        await _stateMachine.Advance(_stateStorage, key, cancellationToken);
    }

    /// <inheritdoc cref="IStateMachine{TState}.Current(IStateStorage, string, CancellationToken)"/>
    public async Task<TState?> Current(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        return await _stateMachine.Current(_stateStorage, key, cancellationToken);
    }

    /// <inheritdoc cref="IStateMachine{TState}.Reset(IStateStorage, string, CancellationToken)"/>
    public async Task Reset(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        await _stateMachine.Reset(_stateStorage, key, cancellationToken);
    }

    /// <inheritdoc cref="IStateMachine{TState}.Retreat(IStateStorage, string, CancellationToken)"/>
    public async Task Retreat(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        await _stateMachine.Retreat(_stateStorage, key, cancellationToken);
    }
}
