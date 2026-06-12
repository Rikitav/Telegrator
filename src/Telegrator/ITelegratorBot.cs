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

using Telegram.Bot.Polling;
using Telegrator.Core;

namespace Telegrator;

/// <summary>
/// Interface for reactive Telegram bot implementations.
/// Defines the core properties and capabilities of a reactive bot.
/// </summary>
public interface ITelegratorBot
{
    /// <summary>
    /// Gets the update router for handling incoming updates.
    /// </summary>
    public IUpdateRouter UpdateRouter { get; }

    /// <summary>
    /// Initializes the update router and begins polling for updates asynchronously.
    /// </summary>
    /// <param name="receiverOptions">Optional receiver options for configuring update polling.</param>
    /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
    /// <returns></returns>
    Task StartReceivingAsync(ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default);
}
