using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Core.States;

namespace Telegrator.Core.Handlers;

/// <summary>
/// Represents an empty handler container that throws <see cref="NotImplementedException"/> for all members.
/// </summary>
public class EmptyHandlerContainer : IHandlerContainer
{
    /// <inheritdoc/>
    public Update HandlingUpdate => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public ITelegramBotClient Client => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public Dictionary<string, object> ExtraData => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public CompletedFiltersList CompletedFilters => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public IAwaitingProvider AwaitingProvider => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public IStateStorage StateStorage => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");
}
