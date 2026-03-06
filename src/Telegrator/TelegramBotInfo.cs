using Telegram.Bot.Types;
using Telegrator.Configuration;

namespace Telegrator
{
    /// <summary>
    /// Implementation of <see cref="ITelegramBotInfo"/> that provides bot information.
    /// Contains metadata about the Telegram bot including user details.
    /// </summary>
    public class TelegramBotInfo(User user) : ITelegramBotInfo
    {
        /// <summary>
        /// Gets the user information for the bot.
        /// </summary>
        public User User { get; } = user;
    }
}
