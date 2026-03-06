using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters that operate on the chat of the message being processed.
    /// </summary>
    public abstract class MessageChatFilter : MessageFilterBase
    {
        /// <summary>
        /// Gets the chat of the message being processed.
        /// </summary>
        public Chat Chat { get; private set; } = null!;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            Chat = Target.Chat;
            return CanPassNext(context.CreateChild(Chat));
        }

        /// <summary>
        /// Determines whether the filter passes for the given chat context.
        /// </summary>
        /// <param name="context">The filter execution context for the chat.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        protected abstract bool CanPassNext(FilterExecutionContext<Chat> context);
    }

    /// <summary>
    /// Filters messages whose chat is a forum.
    /// </summary>
    public class MessageChatIsForumFilter : MessageChatFilter
    {
        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
            => Chat.IsForum;
    }

    /// <summary>
    /// Filters messages whose chat ID matches the specified value.
    /// </summary>
    public class MessageChatIdFilter(long id) : MessageChatFilter
    {
        private readonly long Id = id;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
            => Chat.Id == Id;
    }

    /// <summary>
    /// Filters messages whose chat type matches the specified value.
    /// </summary>
    public class MessageChatTypeFilter : MessageChatFilter
    {
        private readonly ChatType? Type;
        private readonly ChatTypeFlags? Flags;

        /// <summary>
        /// Initialize new instance of <see cref="MessageChatTypeFilter"/>
        /// </summary>
        /// <param name="type"></param>
        public MessageChatTypeFilter(ChatType type)
            => Type = type;

        /// <summary>
        /// Initialize new instance of <see cref="MessageChatTypeFilter"/> with <see cref="ChatTypeFlags"/>
        /// </summary>
        /// <param name="type"></param>
        public MessageChatTypeFilter(ChatTypeFlags type)
            => Flags = type;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
        {
            if (Type.HasValue)
                return Chat.Type == Type.Value;

            if (Flags != null)
            {
                ChatTypeFlags? asFlag = ToFlag(Chat.Type);
                return asFlag.HasValue && Flags.Value.HasFlag(asFlag.Value);
            }

            return false;
        }

        private static ChatTypeFlags? ToFlag(ChatType type) => type switch
        {
            ChatType.Channel => ChatTypeFlags.Channel,
            ChatType.Group => ChatTypeFlags.Group,
            ChatType.Supergroup => ChatTypeFlags.Supergroup,
            ChatType.Sender => ChatTypeFlags.Sender,
            ChatType.Private => ChatTypeFlags.Private,
            _ => null
        };
    }

    /// <summary>
    /// Filters messages whose chat title matches the specified value.
    /// </summary>
    public class MessageChatTitleFilter : MessageChatFilter
    {
        private readonly string? Title;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatTitleFilter"/> class.
        /// </summary>
        /// <param name="title">The chat title to match.</param>
        public MessageChatTitleFilter(string? title) => Title = title;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatTitleFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="title">The chat title to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        public MessageChatTitleFilter(string? title, StringComparison comparison)
            : this(title) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
        {
            if (Chat.Title == null)
                return false;

            return Chat.Title.Equals(Title, Comparison);
        }
    }

    /// <summary>
    /// Filters messages whose chat username matches the specified value.
    /// </summary>
    public class MessageChatUsernameFilter : MessageChatFilter
    {
        private readonly string? UserName;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatUsernameFilter"/> class.
        /// </summary>
        /// <param name="userName">The chat username to match.</param>
        public MessageChatUsernameFilter(string? userName) => UserName = userName;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatUsernameFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="userName">The chat username to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        public MessageChatUsernameFilter(string? userName, StringComparison comparison)
            : this(userName) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
        {
            if (Chat.Username == null)
                return false;

            return Chat.Username.Equals(UserName, Comparison);
        }
    }

    /// <summary>
    /// Filters messages whose chat first and/or last name matches the specified values.
    /// </summary>
    public class MessageChatNameFilter : MessageChatFilter
    {
        private readonly string? FirstName;
        private readonly string? LastName;
        private readonly StringComparison Comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatNameFilter"/> class.
        /// </summary>
        /// <param name="firstName">The chat first name to match.</param>
        /// <param name="lastName">The chat last name to match.</param>
        public MessageChatNameFilter(string? firstName, string? lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageChatNameFilter"/> class with a specific string comparison.
        /// </summary>
        /// <param name="firstName">The chat first name to match.</param>
        /// <param name="lastName">The chat last name to match.</param>
        /// <param name="comparison">The string comparison to use.</param>
        public MessageChatNameFilter(string? firstName, string? lastName, StringComparison comparison)
            : this(firstName, lastName) => Comparison = comparison;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Chat> _)
        {
            if (LastName != null)
            {
                if (Chat.LastName == null)
                    return false;

                if (Chat.LastName.Equals(LastName, Comparison))
                    return false;
            }

            if (FirstName != null)
            {
                if (Chat.FirstName == null)
                    return false;

                if (Chat.FirstName.Equals(FirstName, Comparison))
                    return false;
            }

            return true;
        }
    }
}
