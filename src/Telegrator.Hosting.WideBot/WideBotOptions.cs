/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegram.Bot;
using WTelegram;

namespace Telegrator;

/// <summary>
/// Represents configuration options for initializing and customizing the behavior of a WideBot instance.
/// </summary>
/// <remarks>Use this class to specify required API credentials, optional proxy settings, update handling
/// preferences, and SQL command detection for WideBot. All required properties must be set before using the options
/// with a WideBot instance.
/// </remarks>
public class WideBotOptions
{
    /// <inheritdoc cref="WTelegramBotClientOptions.ApiId"/>
    public required int ApiId { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.ApiHash"/>
    public required string ApiHash { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.MTProxy"/>
    public string? MTProxy { get; set; }

    /// <inheritdoc cref="WTelegramBotClientOptions.SqlCommands"/>
    public SqlCommands SqlCommands { get; set; } = WTelegram.SqlCommands.Detect;

    /// <summary>
    /// Gets or sets a value indicating whether pending updates should be discarded.
    /// </summary>
    public bool DropPendingUpdates { get; set; }
}
