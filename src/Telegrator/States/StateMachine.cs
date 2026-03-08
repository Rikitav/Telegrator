using Telegram.Bot.Types;
using Telegrator.Core.States;

namespace Telegrator.States;

public class StateMachine<TMachine, TState>(IStateStorage stateStorage, Update handlingUpdate)
    where TMachine : IStateMachine<TState>, new()
    where TState : IEquatable<TState>
{
    private readonly IStateStorage _stateStorage = stateStorage;
    private readonly Update _handlingUpdate = handlingUpdate;
    private readonly IStateMachine<TState> _stateMachine = new TMachine();

    public IStateKeyResolver? KeyResolver;

    public async Task Advance(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        await _stateMachine.Advance(_stateStorage, key, cancellationToken);
    }

    public async Task<TState?> Current(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        return await _stateMachine.Current(_stateStorage, key, cancellationToken);
    }

    public async Task Reset(CancellationToken cancellationToken = default)
    {
        if (KeyResolver is null)
            throw new InvalidOperationException("KeyResolver is not set.");

        string? key = KeyResolver.ResolveKey(_handlingUpdate);
        if (key is null)
            throw new InvalidOperationException("Failed to resolve Update key");

        await _stateMachine.Reset(_stateStorage, key, cancellationToken);
    }

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
