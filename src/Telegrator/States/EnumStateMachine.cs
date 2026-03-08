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
    private TEnum _defaultState => _states.FirstOrDefault();

    /// <inheritdoc/>
    public async Task<TEnum> Current(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        TEnum state = await storage.GetAsync<TEnum>(key, cancellationToken);

        return EqualityComparer<TEnum>.Default.Equals(state, default)
            ? _defaultState : state;
    }

    /// <inheritdoc/>
    public async Task Advance(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        TEnum currentState = await storage.GetAsync<TEnum>(key, cancellationToken);

        int currentIndex = Array.IndexOf(_states, currentState);
        if (currentIndex < _states.Length - 1)
        {
            var nextState = _states[currentIndex + 1];
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
            var nextState = _states[currentIndex - 1];
            await storage.SetAsync(key, nextState, cancellationToken);
        }
    }

    /// <inheritdoc/>
    public async Task Reset(IStateStorage storage, string updateKey, CancellationToken cancellationToken = default)
    {
        string key = FormatKey(updateKey);
        await storage.SetAsync(key, _defaultState, cancellationToken);
    }

    private static string FormatKey(string updateKey)
        => typeof(TEnum).Name + ":" + updateKey;
}
