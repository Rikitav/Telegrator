using System.Diagnostics.CodeAnalysis;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Configuration options for Telegram bot behavior and execution settings.
    /// Controls various aspects of bot operation including concurrency, routing, webhook receiving, and execution policies.
    /// </summary>
    public class TelegratorWebOptions
    {
        /// <summary>
        /// Gets or sets HTTPS URL to send updates to. Use an empty string to remove webhook integration
        /// </summary>
        [StringSyntax(StringSyntaxAttribute.Uri)]
        public required string WebhookUri { get; set; }

        /// <summary>
        /// A secret token to be sent in a header “X-Telegram-Bot-Api-Secret-Token” in every webhook request, 1-256 characters.
        /// Only characters A-Z, a-z, 0-9, _ and - are allowed.
        /// The header is useful to ensure that the request comes from a webhook set by you.
        /// </summary>
        public string? SecretToken { get; set; }

        /// <summary>
        /// The maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery, 1-100. Defaults to 40.
        /// Use lower values to limit the load on your bot's server, and higher values to increase your bot's throughput.
        /// </summary>
        public int MaxConnections { get; set; } = 40;

        /// <summary>
        /// Pass true to drop all pending updates
        /// </summary>
        public bool DropPendingUpdates { get; set; }
    }
}
