using Telegrator.Filters;

namespace Telegrator.Annotations
{
    /// <summary>
    /// Attribute for filtering messages based on the sender's username.
    /// </summary>
    public class FromUsernameAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages from a specific username.
        /// </summary>
        /// <param name="username">The username to match</param>
        public FromUsernameAttribute(string username)
            : base(new FromUsernameFilter(username)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from a specific username with custom comparison.
        /// </summary>
        /// <param name="username">The username to match</param>
        /// <param name="comparison">The string comparison method</param>
        public FromUsernameAttribute(string username, StringComparison comparison)
            : base(new FromUsernameFilter(username, comparison)) { }
    }

    /// <summary>
    /// Attribute for filtering messages based on the sender's name (first name and optionally last name).
    /// </summary>
    public class FromUserAttribute : MessageFilterAttribute
    {
        /// <summary>
        /// Initializes the attribute to filter messages from a user with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match (optional)</param>
        /// <param name="comparison">The string comparison method</param>
        public FromUserAttribute(string firstName, string? lastName, StringComparison comparison)
            : base(new FromUserFilter(firstName, lastName, comparison)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from a user with specific first and last names.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="lastName">The last name to match</param>
        public FromUserAttribute(string firstName, string? lastName)
            : base(new FromUserFilter(firstName, lastName, StringComparison.InvariantCulture)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from a user with a specific first name.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        public FromUserAttribute(string firstName)
            : base(new FromUserFilter(firstName, null, StringComparison.InvariantCulture)) { }

        /// <summary>
        /// Initializes the attribute to filter messages from a user with a specific first name and custom comparison.
        /// </summary>
        /// <param name="firstName">The first name to match</param>
        /// <param name="comparison">The string comparison method</param>
        public FromUserAttribute(string firstName, StringComparison comparison)
            : base(new FromUserFilter(firstName, null, comparison)) { }
    }

    /// <summary>
    /// Attribute for filtering messages from a specific user ID.
    /// </summary>
    /// <param name="userId">The user ID to match</param>
    public class FromUserIdAttribute(long userId)
        : MessageFilterAttribute(new FromUserIdFilter(userId))
    { }

    /// <summary>
    /// Attribute for filtering messages sent by not bots (users).
    /// </summary>
    public class NotFromBotAttribute()
        : MessageFilterAttribute(new FromBotFilter().Not())
    { }

    /// <summary>
    /// Attribute for filtering messages sent by bots.
    /// </summary>
    public class FromBotAttribute()
        : MessageFilterAttribute(new FromBotFilter())
    { }

    /// <summary>
    /// Attribute for filtering messages sent by premium users.
    /// </summary>
    public class FromPremiumUserAttribute()
        : MessageFilterAttribute(new FromPremiumUserFilter())
    { }
}
