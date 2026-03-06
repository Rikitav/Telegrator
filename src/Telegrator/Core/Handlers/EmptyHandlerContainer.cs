using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Core.Handlers
{
    /// <summary>
    /// Represents an empty handler container that throws <see cref="NotImplementedException"/> for all members.
    /// </summary>
    public class EmptyHandlerContainer : IHandlerContainer
    {
        /// <inheritdoc/>
        public Update HandlingUpdate => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITelegramBotClient Client => throw new NotImplementedException();

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData => throw new NotImplementedException();

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters => throw new NotImplementedException();

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider => throw new NotImplementedException();
    }
}
