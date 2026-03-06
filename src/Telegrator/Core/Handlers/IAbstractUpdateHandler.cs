using Telegrator.Handlers;

namespace Telegrator.Core.Handlers;

/// <summary>
/// Abstract handler for Telegram updates of type <typeparamref name="TUpdate"/>.
/// </summary>
public interface IAbstractUpdateHandler<TUpdate> where TUpdate : class
{
    /// <summary>
    /// Handler container for the current update.
    /// </summary>
    public IHandlerContainer<TUpdate> Container { get; }

    /// <summary>
    /// Abstract method to execute the update handling logic.
    /// </summary>
    /// <param name="container">The handler container.</param>
    /// <param name="cancellation">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<Result> Execute(IHandlerContainer<TUpdate> container, CancellationToken cancellation);
}