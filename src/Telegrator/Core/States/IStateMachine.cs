namespace Telegrator.Core.States;

public interface IStateMachine<TState> where TState : IEquatable<TState>
{
    Task<TState?> Current(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);
    Task Advance(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);
    Task Retreat(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);
    Task Reset(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default);
}
