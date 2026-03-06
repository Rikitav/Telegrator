using Telegrator.Configuration;

namespace Telegrator
{
    /// <summary>
    /// Configuration options for Telegram bot behavior and execution settings.
    /// Controls various aspects of bot operation including concurrency, routing, and execution policies.
    /// </summary>
    public class TelegratorOptions : ITelegratorOptions
    {
        /// <inheritdoc/>
        public int? MaximumParallelWorkingHandlers { get; set; }

        /// <inheritdoc/>
        public bool ExclusiveAwaitingHandlerRouting { get; set; }

        /// <inheritdoc/>
        public bool ExceptIntersectingCommandAliases { get; set; } = true;

        /// <inheritdoc/>
        public CancellationToken GlobalCancellationToken { get; set; }
    }
}
