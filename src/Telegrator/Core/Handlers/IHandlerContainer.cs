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
using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Core.States;

namespace Telegrator.Core.Handlers;

/// <summary>
/// Interface for handler containers that provide context and resources for update handlers.
/// Contains all necessary information and services that handlers need during execution.
/// </summary>
public interface IHandlerContainer
{
    /// <summary>
    /// Gets the <see cref="Update"/> being handled.
    /// </summary>
    public Update HandlingUpdate { get; }

    /// <summary>
    /// Gets the <see cref="ITelegramBotClient"/> used for this handler.
    /// </summary>
    public ITelegramBotClient Client { get; }

    /// <summary>
    /// Gets the extra data associated with the handler execution.
    /// </summary>
    public Dictionary<string, object> ExtraData { get; }

    /// <summary>
    /// Gets the <see cref="CompletedFiltersList"/> for this handler.
    /// </summary>
    public CompletedFiltersList CompletedFilters { get; }

    /// <summary>
    /// Gets the <see cref="IAwaitingProvider"/> for awaiting operations.
    /// </summary>
    public IAwaitingProvider AwaitingProvider { get; }

    /// <summary>
    /// Gets the <see cref="IStateStorage"/> for state managment.
    /// </summary>
    public IStateStorage StateStorage { get; }
}
