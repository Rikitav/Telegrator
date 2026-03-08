namespace Telegrator.Core.States;

public interface IStateStorage
{
    Task SetAsync<T>(string key, T state, CancellationToken cancellationToken = default);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
