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
