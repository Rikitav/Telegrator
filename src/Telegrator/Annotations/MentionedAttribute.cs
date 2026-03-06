using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages that contain mentions.
    /// Allows handlers to respond only to messages that mention the bot or specific users.
    /// </summary>
    public class MentionedAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MentionedAttribute that matches any mention.
        /// </summary>
        public MentionedAttribute()
            : base(new MessageHasEntityFilter(MessageEntityType.Mention, null, null), new MentionedFilter()) { }

        /// <summary>
        /// Initializes a new instance of the MentionedAttribute that matches mentions at a specific offset.
        /// </summary>
        /// <param name="offset">The offset position where the mention should occur.</param>
        public MentionedAttribute(int offset)
            : base(new MessageHasEntityFilter(MessageEntityType.Mention, offset, null), new MentionedFilter()) { }

        /// <summary>
        /// Initializes a new instance of the MentionedAttribute that matches a specific mention.
        /// </summary>
        /// <param name="mention">The specific mention text to match.</param>
        public MentionedAttribute(string mention)
            : base(new MessageHasEntityFilter(MessageEntityType.Mention), new MentionedFilter(mention)) { }

        /// <summary>
        /// Initializes a new instance of the MentionedAttribute that matches a specific mention at a specific offset.
        /// </summary>
        /// <param name="mention">The specific mention text to match.</param>
        /// <param name="offset">The offset position where the mention should occur.</param>
        public MentionedAttribute(string mention, int offset)
            : base(new MessageHasEntityFilter(MessageEntityType.Mention, offset, null), new MentionedFilter(mention)) { }
    }
}
