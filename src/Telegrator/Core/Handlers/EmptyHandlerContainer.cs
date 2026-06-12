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
/// Represents an empty handler container that throws <see cref="NotImplementedException"/> for all members.
/// </summary>
public class EmptyHandlerContainer : IHandlerContainer
{
    /// <inheritdoc/>
    public Update HandlingUpdate => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public ITelegramBotClient Client => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public Dictionary<string, object> ExtraData => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public CompletedFiltersList CompletedFilters => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public IAwaitingProvider AwaitingProvider => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");

    /// <inheritdoc/>
    public IStateStorage StateStorage => throw new NotImplementedException("EmptyHandlerContainer does not provide this implementation.");
}
