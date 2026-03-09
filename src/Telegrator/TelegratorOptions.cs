namespace Telegrator;

/// <summary>
/// Configuration options for Telegram bot behavior and execution settings.
/// Controls various aspects of bot operation including concurrency, routing, and execution policies.
/// </summary>
public class TelegratorOptions
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
    /// Gets or sets the maximum number of parallel working handlers. Null means no limit.
    /// </summary>
    public int? MaximumParallelWorkingHandlers { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether awaiting handlers should be routed separately from regular handlers.
    /// </summary>
    public bool ExclusiveAwaitingHandlerRouting { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to exclude intersecting command aliases.
    /// </summary>
    public bool ExceptIntersectingCommandAliases { get; set; } = true;

    /// <summary>
    /// Gets or sets the global cancellation token for all bot operations.
    /// </summary>
    public CancellationToken GlobalCancellationToken { get; set; } = default;
}
