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

using StackExchange.Redis;
using System.Text.Json;
using Telegrator.Core.States;

namespace Telegrator.States;

/// <summary>
/// Provides a Redis-based implementation of the <see cref="IStateStorage"/> interface.
/// Serializes state objects to JSON format before storing them in the Redis database.
/// </summary>
public class RedisStateStorage(IConnectionMultiplexer redis) : IStateStorage
{
    private readonly IDatabase _db = redis.GetDatabase();

    /// <inheritdoc/>
    public async Task SetAsync<T>(string key, T state, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        string json = JsonSerializer.Serialize(state);
        await _db.StringSetAsync(key, json);
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RedisValue json = await _db.StringGetAsync(key);
        string? jsonStr = json;

        if (jsonStr is null)
            return default;

        return JsonSerializer.Deserialize<T?>(json: jsonStr);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await _db.KeyDeleteAsync(key);
    }
}
