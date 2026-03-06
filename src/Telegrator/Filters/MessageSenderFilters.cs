using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on message senders.
    /// Provides functionality to access and validate the user who sent the message.
    /// </summary>
    public abstract class MessageSenderFilter : MessageFilterBase
    {
        /// <summary>
        /// Gets the user who sent the message.
        /// </summary>
        public User User { get; private set; } = null!;

        /// <summary>
        /// Determines if the message can pass through the filter by validating
        /// that the message has a valid sender.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the message has a valid sender; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!base.CanPassBase(context))
                return false;

            User = Target.From!;
            if (User is not { Id: > 0 })
                return false;

            return CanPassNext(context);
        }
    }

    /// <summary>
    /// Filter that checks if the message sender has a specific username.
    /// </summary>
    /// <param name="username">The username to check for.</param>
    public class FromUsernameFilter(string username) : MessageSenderFilter
    {
        /// <summary>
        /// The username to check for.
        /// </summary>
        private readonly string _username = username;
        
        /// <summary>
        /// The string comparison type to use for username matching.
        /// </summary>
        private readonly StringComparison _comparison = StringComparison.InvariantCulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromUsernameFilter"/> class with custom string comparison.
        /// </summary>
        /// <param name="username">The username to check for.</param>
        /// <param name="comparison">The string comparison type to use.</param>
        public FromUsernameFilter(string username, StringComparison comparison)
            : this(username) => _comparison = comparison;

        /// <summary>
        /// Checks if the message sender has the specified username.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified username; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => User.Username != null && User.Username.Equals(_username, _comparison);
    }

    /// <summary>
    /// Filter that checks if the message sender has specific first and/or last name.
    /// </summary>
    /// <param name="firstName">The first name to check for.</param>
    /// <param name="lastName">The last name to check for (optional).</param>
    /// <param name="comparison">The string comparison type to use.</param>
    public class FromUserFilter(string firstName, string? lastName, StringComparison comparison) : MessageSenderFilter
    {
        /// <summary>
        /// The first name to check for.
        /// </summary>
        private readonly string _firstName = firstName;
        
        /// <summary>
        /// The last name to check for (optional).
        /// </summary>
        private readonly string? _lastName = lastName;
        
        /// <summary>
        /// The string comparison type to use for name matching.
        /// </summary>
        private readonly StringComparison _comparison = comparison;

        /// <summary>
        /// Initializes a new instance of the <see cref="FromUserFilter"/> class with first and last name.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        /// <param name="lastName">The last name to check for.</param>
        public FromUserFilter(string firstName, string lastName)
            : this(firstName, lastName, StringComparison.InvariantCulture) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FromUserFilter"/> class with first name only.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        public FromUserFilter(string firstName)
            : this(firstName, null, StringComparison.InvariantCulture) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FromUserFilter"/> class with first name and custom comparison.
        /// </summary>
        /// <param name="firstName">The first name to check for.</param>
        /// <param name="comparison">The string comparison type to use.</param>
        public FromUserFilter(string firstName, StringComparison comparison)
            : this(firstName, null, comparison) { }

        /// <summary>
        /// Checks if the message sender has the specified first and/or last name.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified name(s); otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            if (User.LastName != null)
            {
                if (_lastName == null)
                    return false;

                if (!_firstName.Equals(User.LastName, _comparison))
                    return false;
            }

            return User.FirstName.Equals(_firstName, _comparison);
        }
    }

    /// <summary>
    /// Filter that checks if the message sender has a specific user ID.
    /// </summary>
    /// <param name="userId">The user ID to check for.</param>
    public class FromUserIdFilter(long userId) : MessageSenderFilter
    {
        /// <summary>
        /// The user ID to check for.
        /// </summary>
        private readonly long _userId = userId;

        /// <summary>
        /// Checks if the message sender has the specified user ID.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the sender has the specified user ID; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.Id == _userId;
    }

    /// <summary>
    /// Filter that checks if the message was sent by a bot.
    /// </summary>
    public class FromBotFilter() : MessageSenderFilter
    {
        /// <summary>
        /// Checks if the message was sent by a bot.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the message was sent by a bot; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.IsBot;
    }

    /// <summary>
    /// Filter that checks if the message was sent by a premium user.
    /// </summary>
    public class FromPremiumUserFilter() : MessageSenderFilter
    {
        /// <summary>
        /// Checks if the message was sent by a premium user.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the message was sent by a premium user; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => User.IsPremium;
    }
}
