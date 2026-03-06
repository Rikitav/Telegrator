using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process message updates.
    /// This handler will be triggered when users send messages in chats.
    /// </summary>
    public class MessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<MessageHandler>(UpdateType.Message, importance)
    {
        /// <summary>
        /// Checks if the update contains a valid message.
        /// </summary>
        /// <param name="context">The filter execution context containing the update.</param>
        /// <returns>True if the update contains a message; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { Message: { } };
    }

    /// <summary>
    /// Abstract base class for handlers that process message updates.
    /// Provides convenient methods for sending replies and responses to messages.
    /// </summary>
    public abstract class MessageHandler() : AbstractUpdateHandler<Message>(UpdateType.Message)
    {
        /// <summary>
        /// Sends a reply message to the current message.
        /// </summary>
        /// <param name="text">The text of the message to send.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
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
        protected async Task<Message> Reply(
            string text,
            ParseMode parseMode = ParseMode.None,
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
            => await Container.Reply(
                text, parseMode,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);

        /// <summary>
        /// Sends a responce message to the current chat.
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
    }

    /// <summary>
    /// Abstract base class for branching handlers that process message updates.
    /// Provides convenient methods for sending replies and responses to messages in branching scenarios.
    /// </summary>
    public abstract class BranchingMessageHandler() : BranchingUpdateHandler<Message>(UpdateType.Message)
    {
        /// <summary>
        /// Sends a reply message to the current message.
        /// </summary>
        /// <param name="text">The text of the message to send.</param>
        /// <param name="parseMode">The parse mode for the message text.</param>
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
        protected async Task<Message> Reply(
            string text,
            ParseMode parseMode = ParseMode.None,
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
            => await Container.Reply(
                text, parseMode,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);

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
    }
}
