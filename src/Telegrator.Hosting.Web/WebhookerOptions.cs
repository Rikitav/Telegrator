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

using System.Diagnostics.CodeAnalysis;

namespace Telegrator;

/// <summary>
/// Configuration options for Telegram bot behavior and execution settings.
/// Controls various aspects of bot operation including concurrency, routing, webhook receiving, and execution policies.
/// </summary>
public class WebhookerOptions
{
    /// <summary>
    /// Gets or sets HTTPS URL to send updates to. Use an empty string to remove webhook integration
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.Uri)]
    public string? WebhookUri { get; set; } = null;

    /// <summary>
    /// A secret token to be sent in a header “X-Telegram-Bot-Api-Secret-Token” in every webhook request, 1-256 characters.
    /// Only characters A-Z, a-z, 0-9, _ and - are allowed.
    /// The header is useful to ensure that the request comes from a webhook set by you.
    /// </summary>
    public string? SecretToken { get; set; } = null;

    /// <summary>
    /// The maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery, 1-100. Defaults to 40.
    /// Use lower values to limit the load on your bot's server, and higher values to increase your bot's throughput.
    /// </summary>
    public int MaxConnections { get; set; } = 40;

    /// <summary>
    /// Pass true to drop all pending updates
    /// </summary>
    public bool DropPendingUpdates { get; set; } = false;
}
