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

using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Telegrator.Core;

/// <summary>
/// Interface for handling exceptions that occur during update routing operations.
/// Provides a centralized way to handle and log errors that occur during bot operation.
/// </summary>
public interface IRouterExceptionHandler
{
    /// <summary>
    /// Handles exceptions that occur during update routing.
    /// </summary>
    /// <param name="botClient">The <see cref="ITelegramBotClient"/> instance.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="source">The <see cref="HandleErrorSource"/> indicating the source of the error.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken);
}
