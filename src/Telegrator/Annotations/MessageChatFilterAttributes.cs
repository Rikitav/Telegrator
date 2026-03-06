using Telegram.Bot.Types.Enums;
using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages sent in forum chats.
    /// </summary>
    public class ChatIsForumAttribute()
        : MessageFilterAttribute(new MessageChatIsForumFilter())
    { }

    /// <summary>
    /// Attribute for filtering messages sent in a specific chat by ID.
    /// </summary>
    /// <param name="id">The chat ID to match</param>
    public class ChatIdAttribute(long id)
        : MessageFilterAttribute(new MessageChatIdFilter(id))
    { }

    /// <summary>
    /// Attribute for filtering messages sent in chats of a specific type.
    /// </summary>
    public class ChatTypeAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initialize new instance of <see cref="ChatTypeAttribute"/> to filter messages from chat from specific chats
        /// </summary>
        /// <param name="type"></param>
        public ChatTypeAttribute(ChatType type)
            : base(new MessageChatTypeFilter(type)) { }

        /// <summary>
        /// Initialize new instance of <see cref="ChatTypeAttribute"/> to filter messages from chat from specific chats (with flags)
        /// </summary>
        /// <param name="flags"></param>
        public ChatTypeAttribute(ChatTypeFlags flags)
            : base(new MessageChatTypeFilter(flags)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the chat title.
    /// </summary>
    public class ChatTitleAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages from chats with a specific title and comparison method.
        /// </summary>
        /// <param name="title">The chat title to match</param>
        /// <param name="comparison">The string comparison method</param>
        public ChatTitleAttribute(string? title, StringComparison comparison)
            : base(new MessageChatTitleFilter(title, comparison)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from chats with a specific title.
        /// </summary>
        /// <param name="title">The chat title to match</param>
        public ChatTitleAttribute(string? title)
            : base(new MessageChatTitleFilter(title)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the chat username.
    /// </summary>
    public class ChatUsernameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages from chats with a specific username and comparison method.
        /// </summary>
        /// <param name="userName">The chat username to match</param>
        /// <param name="comparison">The string comparison method</param>
        public ChatUsernameAttribute(string? userName, StringComparison comparison)
            : base(new MessageChatUsernameFilter(userName, comparison)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from chats with a specific username.
        /// </summary>
        /// <param name="userName">The chat username to match</param>
        public ChatUsernameAttribute(string? userName)
            : base(new MessageChatUsernameFilter(userName, StringComparison.InvariantCulture)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the chat name (first name and optionally last name).
    /// </summary>
    public class ChatNameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages from chats with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        /// <param name="comparison">The string comparison method</param>
        public ChatNameAttribute(string? firstName, string? lastName, StringComparison comparison)
            : base(new MessageChatNameFilter(firstName, lastName, comparison)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from chats with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        public ChatNameAttribute(string? firstName, string? lastName)
            : base(new MessageChatNameFilter(firstName, lastName)) { }
    }
}
