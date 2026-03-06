using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Provides usefull helper methods for abstract handler containers
    /// </summary>
    public static class AbstractHandlerContainerExtensions
    {
        /// <summary>
        /// Changes bot's reaction to message
        /// </summary>
        /// <param name="container"></param>
        /// <param name="reaction"></param>
        /// <param name="isBig"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task React(
            this IHandlerContainer<Message> container,
            ReactionType reaction,
            bool isBig = false,
            CancellationToken cancellationToken = default)
            => await container.Client.SetMessageReaction(
                container.ActualUpdate.Chat,
                container.ActualUpdate.Id,
                [reaction], isBig, cancellationToken);

        /// <summary>
        /// Changes bot's reaction to message
        /// </summary>
        /// <param name="container"></param>
        /// <param name="reactions"></param>
        /// <param name="isBig"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task React(
            this IHandlerContainer<Message> container,
            IEnumerable<ReactionType> reactions,
            bool isBig = false,
            CancellationToken cancellationToken = default)
            => await container.Client.SetMessageReaction(
                container.ActualUpdate.Chat,
                container.ActualUpdate.Id,
                reactions, isBig, cancellationToken);

        /// <summary>
        /// Sends a reply message to the current message.
        /// </summary>
        /// <param name="container"></param>
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
        public static async Task<Message> Reply(
            this IHandlerContainer<Message> container,
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
            => await container.Client.SendMessage(
                container.ActualUpdate.Chat, text, parseMode, container.ActualUpdate,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);

        /// <summary>
        /// Sends a responce message to the current chat.
        /// </summary>
        /// <param name="container"></param>
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
        public static async Task<Message> Responce(
            this IHandlerContainer<Message> container,
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
            => await container.Client.SendMessage(
                container.ActualUpdate.Chat, text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);

        /// <summary>
        /// Responces to message that this CallbackQuery was originated from
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="replyParameters"></param>
        /// <param name="replyMarkup"></param>
        /// <param name="linkPreviewOptions"></param>
        /// <param name="messageThreadId"></param>
        /// <param name="entities"></param>
        /// <param name="disableNotification"></param>
        /// <param name="protectContent"></param>
        /// <param name="messageEffectId"></param>
        /// <param name="businessConnectionId"></param>
        /// <param name="allowPaidBroadcast"></param>
        /// <param name="directMessageTopicId"></param>
        /// <param name="suggestedPostParameters"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Message> Responce(
            this IHandlerContainer<CallbackQuery> container,
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
        {
            CallbackQuery query = container.ActualUpdate;
            if (query.Message == null)
                throw new Exception("Callback origin message not found!");

            return await container.Client.SendMessage(
                query.Message.Chat, text, parseMode, replyParameters,
                replyMarkup, linkPreviewOptions,
                messageThreadId, entities,
                disableNotification, protectContent,
                messageEffectId, businessConnectionId,
                allowPaidBroadcast, directMessageTopicId,
                suggestedPostParameters, cancellationToken);
        }

        /// <summary>
        /// Edits message text that this CallbackQuery was originated from
        /// </summary>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="parseMode"></param>
        /// <param name="replyMarkup"></param>
        /// <param name="entities"></param>
        /// <param name="linkPreviewOptions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<Message> EditMessage(
            this IHandlerContainer<CallbackQuery> container,
            string text,
            ParseMode parseMode = ParseMode.None,
            InlineKeyboardMarkup? replyMarkup = null,
            IEnumerable<MessageEntity>? entities = null,
            LinkPreviewOptions? linkPreviewOptions = null,
            CancellationToken cancellationToken = default)
        {
            CallbackQuery query = container.ActualUpdate;
            if (query.Message == null)
                throw new Exception("Callback origin message not found!");

            return await container.Client.EditMessageText(
                query.Message.Chat,
                query.Message.MessageId,
                text: text,
                parseMode: parseMode,
                replyMarkup: replyMarkup,
                entities: entities,
                linkPreviewOptions: linkPreviewOptions,
                cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Use this method to send answers to callback queries sent from <a href="https://core.telegram.org/bots/features#inline-keyboards">inline keyboards</a>.
        /// The answer will be displayed to the user as a notification at the top of the chat screen or as an alert
        /// </summary>
        /// <remarks>
        /// Alternatively, the user can be redirected to the specified Game URL.
        /// For this option to work, you must first create a game for your bot via <a href="https://t.me/botfather">@BotFather</a> and accept the terms.
        /// Otherwise, you may use links like <c>t.me/your_bot?start=XXXX</c> that open your bot with a parameter.
        /// </remarks>
        /// <param name="container"></param>
        /// <param name="text"></param>
        /// <param name="showAlert"></param>
        /// <param name="url"></param>
        /// <param name="cacheTime"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task AnswerCallbackQuery(
            this IHandlerContainer<CallbackQuery> container,
            string? text = null,
            bool showAlert = false,
            string? url = null,
            int cacheTime = 0,
            CancellationToken cancellationToken = default)
            => await container.Client.AnswerCallbackQuery(
                callbackQueryId: container.ActualUpdate.Id,
                text: text,
                showAlert: showAlert,
                url: url,
                cacheTime: cacheTime,
                cancellationToken: cancellationToken);

        /// <summary>
        /// Answers inline query
        /// </summary>
        /// <param name="container"></param>
        /// <param name="results"></param>
        /// <param name="cacheTime"></param>
        /// <param name="isPersonal"></param>
        /// <param name="nextOffset"></param>
        /// <param name="button"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task AnswerInlineQuery(
            this IHandlerContainer<InlineQuery> container,
            IEnumerable<InlineQueryResult> results,
            int? cacheTime = null,
            bool isPersonal = false,
            string? nextOffset = null,
            InlineQueryResultsButton? button = null,
            CancellationToken cancellationToken = default)
        {
            string id = container.ActualUpdate.Id;
            await container.Client.AnswerInlineQuery(id, results.Take(50), cacheTime, isPersonal, nextOffset, button, cancellationToken);
        }
    }
}
