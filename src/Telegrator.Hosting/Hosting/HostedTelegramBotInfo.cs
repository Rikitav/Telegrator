using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;

namespace Telegrator.Hosting
{
    /// <summary>
    /// Implementation of <see cref="ITelegramBotInfo"/> that provides bot information.
    /// Contains metadata about the Telegram bot including user details and service provider for wider filterring abilities
    /// </summary>
    /// <param name="client"></param>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public class HostedTelegramBotInfo(ITelegramBotClient client, IServiceProvider services, IConfiguration configuration) : ITelegramBotInfo
    {
        /// <inheritdoc/>
        public User User { get; } = client.GetMe().Result;
        
        /// <summary>
        /// Provides access to services of this Hosted telegram bot
        /// </summary>
        public IServiceProvider Services { get; } = services;

        /// <summary>
        /// Provides access to configuration of this Hosted telegram bot
        /// </summary>
        public IConfiguration Configuration { get; } = configuration;
    }
}
