using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filter that checks if message has appropriate reply chain.
    /// DOES NOT SHiFT MESSAGE FILTERS TARGET
    /// </summary>
    /// <param name="replyDepth">The depth of reply chain to traverse (default: 1).</param>
    public class MessageHasReplyFilter(int replyDepth = 1) : Filter<Message>
    {
        /// <summary>
        /// Gets the replied message at the specified depth in the reply chain.
        /// </summary>
        public Message Reply { get; private set; } = null!;

        /// <summary>
        /// Gets the depth of reply chain traversal.
        /// </summary>
        public int ReplyDepth { get; private set; } = replyDepth;

        /// <summary>
        /// Determines if the message can pass through the filter by first validating
        /// the reply chain and then applying specific filter logic.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the message passes both reply validation and specific filter criteria; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            Message reply = context.Input;
            for (int i = ReplyDepth; i > 0; i--)
            {
                if (reply.ReplyToMessage is not { Id: > 0 } replyMessage)
                    return false;

                reply = replyMessage;
            }

            Reply = reply;
            return true;
        }
    }

    /// <summary>
    /// Helper filter class for filters that operate on replied messages.
    /// Provides functionality to traverse reply chains and access replied message content
    /// and shifts any next message filter to filter the content of found reply.
    /// </summary>
    /// <param name="replyDepth"></param>
    public class FromReplyChainFilter(int replyDepth = 1) : Filter<Message>
    {
        /// <summary>
        /// Gets the replied message at the specified depth in the reply chain.
        /// </summary>
        public Message Reply { get; private set; } = null!;

        /// <summary>
        /// Gets the depth of reply chain traversal.
        /// </summary>
        public int ReplyDepth { get; private set; } = replyDepth;

        /// <summary>
        /// Determines if the message can pass through the filter by first validating
        /// the reply chain and then applying specific filter logic.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the message passes both reply validation and specific filter criteria; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            Message reply = context.Input;
            for (int i = ReplyDepth; i > 0; i--)
            {
                if (reply.ReplyToMessage is not { Id: > 0 } replyMessage)
                    return false;

                reply = replyMessage;
            }

            Reply = reply;
            return true;
        }
    }

    /// <summary>
    /// Filter that checks if the replied message was sent by the bot itself.
    /// <para>( ! ): REQUIRES <see cref="MessageHasReplyFilter"/> before</para>
    /// </summary>
    public class MeRepliedFilter : Filter<Message>
    {
        /// <summary>
        /// Checks if the replied message was sent by the bot.
        /// </summary>
        /// <param name="context">The filter execution context containing bot information.</param>
        /// <returns>True if the replied message was sent by the bot; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            MessageHasReplyFilter repliedFilter = context.CompletedFilters.Get<MessageHasReplyFilter>(0);
            return context.BotInfo.User == repliedFilter.Reply.From;
        }
    }
}
