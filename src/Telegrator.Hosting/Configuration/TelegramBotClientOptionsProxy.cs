using Telegram.Bot;

namespace Telegrator.Hosting.Configuration
{
    /// <summary>
    /// Internal proxy class for configuring Telegram bot client options from configuration.
    /// Extends ConfigureOptionsProxy to provide specific configuration for Telegram bot client options.
    /// </summary>
    public class TelegramBotClientOptionsProxy : ConfigureOptionsProxy<TelegramBotClientOptions>
    {
        /// <summary>
        /// Gets or sets the bot token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the base URL for the bot API.
        /// </summary>
        public string? BaseUrl { get; set; } = null;
        
        /// <summary>
        /// Gets or sets whether to use the test environment.
        /// </summary>
        public bool UseTestEnvironment { get; set; } = false;
        
        /// <summary>
        /// Gets or sets the retry threshold in seconds.
        /// </summary>
        public int RetryThreshold { get; set; } = 60;
        
        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Creates a TelegramBotClientOptions instance from the proxy configuration.
        /// </summary>
        /// <returns>The configured TelegramBotClientOptions instance.</returns>
        protected override TelegramBotClientOptions Realize() => new TelegramBotClientOptions(Token, BaseUrl, UseTestEnvironment)
        {
            RetryCount = RetryCount,
            RetryThreshold = RetryThreshold
        };
    }
}
