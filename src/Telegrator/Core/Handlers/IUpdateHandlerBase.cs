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
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Handlers.Diagnostics;

namespace Telegrator.Core.Handlers;

/// <summary>
/// Abstraction for update handlers, providing execution and lifetime management for Telegram updates.
/// </summary>
public interface IUpdateHandlerBase : IDisposable
{
    /// <summary>
    /// Gets the <see cref="UpdateType"/> that this handler processes.
    /// </summary>
    UpdateType HandlingUpdateType { get; }

    /// <summary>
    /// Gets the <see cref="HandlerLifetimeToken"/> associated with this handler instance.
    /// </summary>
    HandlerLifetimeToken LifetimeToken { get; }

    /// <summary>
    /// Executes the handler logic and marks the lifetime as ended after execution.
    /// </summary>
    /// <param name="described"></param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task<Result> Execute(DescribedHandlerDescriptor described, CancellationToken cancellationToken = default);

    /// <summary>
    /// Handles failed filters during handler describing.
    /// Use <see cref="Result"/> to control how router should treat this fail.
    /// <see cref="Result.Next"/> to silently continue decribing.
    /// <see cref="Result.Fault"/> to stop\break desribing sequence.
    /// </summary>
    /// <param name="report"></param>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result> FiltersFallback(FiltersFallbackReport report, ITelegramBotClient client, CancellationToken cancellationToken = default);
}