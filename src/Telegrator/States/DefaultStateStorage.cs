using System.Collections.Concurrent;
using Telegrator.Core.States;

namespace Telegrator.States;

public class DefaultStateStorage : IStateStorage
{
    private readonly ConcurrentDictionary<string, object> storage = [];

    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (!storage.TryRemove(key, out object value))
            throw new Exception("Failed to remove key '" + key + "' from storage.");

        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken ccancellationTokent = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (storage.TryGetValue(key, out object value) && value is T)
            return Task.FromResult((T?)value);

        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T state, CancellationToken cancellationToken = default)
    {
        if (key is null)
            throw new ArgumentNullException(nameof(key));

        if (state is null)
            throw new ArgumentNullException(nameof(state));

        storage.Set(key, state);
        return Task.CompletedTask;
    }
}
