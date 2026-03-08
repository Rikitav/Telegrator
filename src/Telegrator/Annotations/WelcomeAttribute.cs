using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering message with command "start" in bot's private chats.
    /// Allows handlers to respond to "welcome" bot commands.
    /// </summary>
    public class WelcomeAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Creates new instance of <see cref="WelcomeAttribute"/>
        /// </summary>
        /// <param name="onlyFirst"></param>
        public WelcomeAttribute(bool onlyFirst = false) : base(
            new MessageChatTypeFilter(ChatType.Private),
            new CommandAlliasFilter("start"),
            Filter<Message>.If(ctx => !onlyFirst || ctx.Input.Id == 0))
        { }
    }
}
