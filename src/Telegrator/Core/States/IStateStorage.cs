namespace Telegrator.Core.States;

/// <summary>
/// Defines a contract for an asynchronous state storage mechanism.
/// </summary>
public interface IStateStorage
{
    /// <summary>
    /// Saves or updates a state value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the state object.</typeparam>
    /// <param name="key">The unique identifier for the state.</param>
    /// <param name="state">The state object to store.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SetAsync<T>(string key, T state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a state value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the state object to retrieve.</typeparam>
    /// <param name="key">The unique identifier for the state.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous retrieve operation. The task result contains the state object if found; otherwise, the default value of <typeparamref name="T"/>.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the state value associated with the specified key.
    /// </summary>
    /// <param name="key">The unique identifier for the state to remove.</param>
    /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}