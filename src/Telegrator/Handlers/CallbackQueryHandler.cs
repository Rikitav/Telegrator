using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process callback query updates.
    /// This handler will be triggered when users interact with inline keyboards or other callback mechanisms.
    /// </summary>
    /// <param name="importance"></param>
    public sealed class CallbackQueryHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<CallbackQueryHandler>(UpdateType.CallbackQuery, importance)
    {
        /// <summary>
        /// Always returns true, allowing any callback query update to pass through this filter.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>Always returns true to allow any callback query update.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { CallbackQuery: { } };
    }

    /// <summary>
    /// Abstract base class for handlers that process callback query updates.
    /// Provides a foundation for creating handlers that respond to user interactions with inline keyboards.
    /// </summary>
    public abstract class CallbackQueryHandler() : AbstractUpdateHandler<CallbackQuery>(UpdateType.CallbackQuery)
    {
        /// <summary>
        /// Gets the type-specific data from the callback query.
        /// Returns the data string, chat instance, or game short name depending on the callback query type.
        /// </summary>
        protected string TypeData
        {
            get => Input switch
            {
                { Data: { } data } => data,
                { ChatInstance: { } chatInstance } => chatInstance,
                { GameShortName: { } gameShortName } => gameShortName
            };
        }

        /// <summary>
        /// Sends a response message to the current chat.
        /// </summary>
        /// <param name="text">The text of the message to send.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
        /// <param name="replyParameters">The reply parameters for the message.</param>
        /// <param name="replyMarkup">The reply markup for the message.</param>
        /// <param name="linkPreviewOptions">Options for link preview generation.</param>
        /// <param name="messageThreadId">The thread ID for forum topics.</param>
        /// <param name="entities">The message entities to include.</param>
        /// <param name="disableNotification">Whether to disable notification for the message.</param>
        /// <param name="protectContent">Whether to protect the message content.</param>
        /// <param name="messageEffectId">The message effect ID.</param>
        /// <param name="businessConnectionId">The business connection ID.</param>
        /// <param name="allowPaidBroadcast">Whether to allow paid broadcast.</param>
        /// <param name="directMessageTopicId"></param>
        /// <param name="suggestedPostParameters"></param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The sent message.</returns>
        protected async Task<Message> Responce(
            string text,
            ParseMode parseMode = ParseMode.None,
            ReplyParameters? replyParameters = null,
            ReplyMarkup? replyMarkup = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            int? messageThreadId = null,
            IEnumerable<MessageEntity>? entities = null,
            bool disableNotification = false,
            bool protectContent = false,
            string? messageEffectId = null,
            string? businessConnectionId = null,
            bool allowPaidBroadcast = false,
            int? directMessageTopicId = null,
            SuggestedPostParameters? suggestedPostParameters = null,
            CancellationToken cancellationToken = default)
            => await Container.Responce(
                text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);

        /// <summary>
        /// Edits the current callback message with new text.
        /// </summary>
        /// <param name="text">The new text of the message.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
        /// <param name="replyMarkup">The reply markup for the message.</param>
        /// <param name="entities">The message entities to include.</param>
        /// <param name="linkPreviewOptions">Options for link preview generation.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The edited message.</returns>
        protected async Task<Message> EditMessage(
            string text,
            ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup? replyMarkup = null,
            IEnumerable<MessageEntity>? entities = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            CancellationToken cancellationToken = default)
            => await Container.EditMessage(
                text, parseMode, replyMarkup,
                entities, linkPreviewOptions, cancellationToken);
        
        /// <summary>
        /// Answers the current callback query with optional alert or message.
        /// </summary>
        /// <param name="text">The text to display in the callback answer.</param>
        /// <param name="showAlert">Whether to show an alert popup instead of a toast.</param>
        /// <param name="url">A URL that will be opened by the client.</param>
        /// <param name="cacheTime">The maximum amount of time in seconds that the result of the callback query may be cached client-side.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        protected async Task Answer(
            string? text = null,
            bool showAlert = false,
            string? url = null,
            int cacheTime = 0,
            CancellationToken cancellationToken = default)
            => await Container.AnswerCallbackQuery(
                text, showAlert, url, cacheTime, cancellationToken);
    }
}
