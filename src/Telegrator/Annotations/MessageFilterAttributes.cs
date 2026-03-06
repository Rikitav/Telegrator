using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.Attributes;
using Telegrator.Filters.Components;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Abstract base attribute for filtering message-based updates.
    /// Supports various message types including regular messages, edited messages, channel posts, and business messages.
    /// </summary>
    /// <param name="filters">The filters to apply to messages</param>
    public abstract class MessageFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
    {
        /// <summary>
        /// Gets the allowed update types that this filter can process.
        /// </summary>
        public override UpdateType[] AllowedTypes =>
        [
            UpdateType.Message,
            UpdateType.EditedMessage,
            UpdateType.ChannelPost,
            UpdateType.EditedChannelPost,
            UpdateType.BusinessMessage,
            UpdateType.EditedBusinessMessage
        ];

        /// <summary>
        /// Extracts the message from various types of updates.
        /// </summary>
        /// <param name="update">The Telegram update</param>
        /// <returns>The message from the update, or null if not present</returns>
        public override Message? GetFilterringTarget(Update update)
        {
            return update switch
            {
                { Message: { } message } => message,
                { EditedMessage: { } message } => message,
                { ChannelPost: { } message } => message,
                { EditedChannelPost: { } message } => message,
                { BusinessMessage: { } message } => message,
                { EditedBusinessMessage: { } message } => message,
                _ => null
            };
        }
    }

    /// <summary>
    /// Attribute for filtering messages based on regular expression patterns.
    /// </summary>
    public class MessageRegexAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute with a regex pattern and options.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match</param>
        /// <param name="regexOptions">The regex options for matching</param>
        public MessageRegexAttribute(string pattern, RegexOptions regexOptions = default)
            : base(new MessageRegexFilter(pattern, regexOptions)) { }

        /// <summary>
        /// Initializes the attribute with a precompiled regex.
        /// </summary>
        /// <param name="regex">The precompiled regular expression</param>
        public MessageRegexAttribute(Regex regex)
            : base(new MessageRegexFilter(regex)) { }
    }

    /// <summary>
    /// Attribute for filtering messages that contain dice throws with specific values.
    /// </summary>
    public class DiceThrowedAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter dice throws with a specific value.
        /// </summary>
        /// <param name="value">The dice value to match</param>
        public DiceThrowedAttribute(int value)
            : base(new DiceThrowedFilter(value)) { }

        /// <summary>
        /// Initializes the attribute to filter dice throws with a specific type and value.
        /// </summary>
        /// <param name="diceType">The type of dice</param>
        /// <param name="value">The dice value to match</param>
        public DiceThrowedAttribute(DiceType diceType, int value)
            : base(new DiceThrowedFilter(diceType, value)) { }
    }

    /// <summary>
    /// Attribute for filtering messages that are automatically forwarded.
    /// </summary>
    public class IsAutomaticFormwardMessageAttribute()
        : MessageFilterAttribute(new IsAutomaticFormwardMessageFilter())
    { }

    /// <summary>
    /// Attribute for filtering messages sent while the user was offline.
    /// </summary>
    public class IsFromOfflineMessageAttribute()
        : MessageFilterAttribute(new IsFromOfflineMessageFilter())
    { }

    /// <summary>
    /// Attribute for filtering service messages (e.g., user joined, left, etc.).
    /// </summary>
    public class IsServiceMessageMessageAttribute()
        : MessageFilterAttribute(new IsServiceMessageMessageFilter())
    { }

    /// <summary>
    /// Attribute for filtering topic messages in forum chats.
    /// </summary>
    public class IsTopicMessageMessageAttribute()
        : MessageFilterAttribute(new IsServiceMessageMessageFilter())
    { }

    /// <summary>
    /// Attribute for filtering messages based on their entities (mentions, links, etc.).
    /// </summary>
    public class MessageHasEntityAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages with a specific entity type.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        public MessageHasEntityAttribute(MessageEntityType type)
            : base(new MessageHasEntityFilter(type)) { }

        /// <summary>
        /// Initializes the attribute to filter messages with a specific entity type at a specific position.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="offset">The starting position of the entity</param>
        /// <param name="length">The length of the entity (optional)</param>
        public MessageHasEntityAttribute(MessageEntityType type, int offset, int? length)
            : base(new MessageHasEntityFilter(type, offset, length)) { }

        /// <summary>
        /// Initializes the attribute to filter messages with a specific entity type and content.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="content">The content that the entity should contain</param>
        /// <param name="stringComparison">The string comparison method</param>
        public MessageHasEntityAttribute(MessageEntityType type, string content, StringComparison stringComparison = StringComparison.CurrentCulture)
            : base(new MessageHasEntityFilter(type, content, stringComparison)) { }

        /// <summary>
        /// Initializes the attribute to filter messages with a specific entity type, position, and content.
        /// </summary>
        /// <param name="type">The entity type to match</param>
        /// <param name="offset">The starting position of the entity</param>
        /// <param name="length">The length of the entity (optional)</param>
        /// <param name="content">The content that the entity should contain</param>
        /// <param name="stringComparison">The string comparison method</param>
        public MessageHasEntityAttribute(MessageEntityType type, int offset, int? length, string content, StringComparison stringComparison = StringComparison.CurrentCulture)
            : base(new MessageHasEntityFilter(type, offset, length, content, stringComparison)) { }
    }
}
