/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
