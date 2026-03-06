namespace Telegrator.Configuration
{
    /// <summary>
    /// Interface for configuring Telegram bot behavior and execution settings.
    /// Controls various aspects of bot operation including concurrency, routing, collecting, and execution policies.
    /// </summary>
    public interface ITelegratorOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of parallel working handlers. Null means no limit.
        /// </summary>
        public int? MaximumParallelWorkingHandlers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether awaiting handlers should be routed separately from regular handlers.
        /// </summary>
        public bool ExclusiveAwaitingHandlerRouting { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude intersecting command aliases.
        /// </summary>
        public bool ExceptIntersectingCommandAliases { get; set; }

        /// <summary>
        /// Gets or sets the global cancellation token for all bot operations.
        /// </summary>
        public CancellationToken GlobalCancellationToken { get; set; }
    }
}
