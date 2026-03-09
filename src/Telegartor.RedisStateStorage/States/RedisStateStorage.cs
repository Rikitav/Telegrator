using StackExchange.Redis;
using System.Text.Json;
using Telegrator.Core.States;

namespace Telegrator.States;

public class RedisStateStorage(IConnectionMultiplexer redis) : IStateStorage
{
    private readonly IDatabase _db = redis.GetDatabase();

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T state, CancellationToken cancellationToken)
    {
        string json = JsonSerializer.Serialize(state);
        await _db.StringSetAsync(key, json);
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        RedisValue json = await _db.StringGetAsync(key);
        string? jsonStr = json;

        if (jsonStr is null)
            return default;

        return JsonSerializer.Deserialize<T?>(json: jsonStr);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}