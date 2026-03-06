using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages with reply to messages of this bot.
    /// </summary>
    public class MeRepliedAttribute()
        : MessageFilterAttribute(new MeRepliedFilter())
    { }

    /// <summary>
    /// Attribute for checking message's reply chain.
    /// </summary>
    public class HasReplyAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new MessageHasReplyFilter(replyDepth))
    { }

    /// <summary>
    /// Helper filter class for filters that operate on replied messages.
    /// Provides functionality to traverse reply chains and access replied message content.
    /// </summary>
    /// <param name="replyDepth"></param>
    public class FromReplyChainAttribute(int replyDepth = 1)
        : MessageFilterAttribute(new FromReplyChainFilter(replyDepth))
    { }
}
