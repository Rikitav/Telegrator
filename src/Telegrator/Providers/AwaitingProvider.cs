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

using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;

namespace Telegrator.Providers;

/// <summary>
/// Provider for managing awaiting handlers that can wait for specific update types.
/// Extends HandlersProvider to provide functionality for creating and managing awaiter handlers.
/// </summary>
/// <param name="options">The bot configuration options.</param>
public class AwaitingProvider(TelegratorOptions options) : HandlersProvider([], options), IAwaitingProvider
{
    /// <summary>
    /// List of handler descriptors for awaiting handlers.
    /// </summary>
    protected readonly HandlerDescriptorList HandlersList = [];

    /// <inheritdoc/>
    public override bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
    {
        if (HandlersList.HandlingType != updateType)
        {
            list = null;
            return false;
        }

        list = HandlersList;
        return true;
    }

    /// <inheritdoc/>
    public IDisposable UseHandler(HandlerDescriptor handlerDescriptor)
    {
        HandlerToken handlerToken = new HandlerToken(HandlersList, handlerDescriptor);
        handlerToken.Register();
        return handlerToken;
    }

    /// <summary>
    /// Token for managing the lifetime of a handler in the awaiting provider.
    /// Implements IDisposable to automatically remove the handler when disposed.
    /// </summary>
    /// <param name="handlersList">The list of handler descriptors.</param>
    /// <param name="handlerDescriptor">The handler descriptor to manage.</param>
    private readonly struct HandlerToken(HandlerDescriptorList handlersList, HandlerDescriptor handlerDescriptor) : IDisposable
    {
        /// <summary>
        /// Registers the handler descriptor in the handlers list.
        /// </summary>
        /// <exception cref="Exception">Thrown when the handler descriptor has no singleton instance.</exception>
        public readonly void Register()
        {
            if (handlerDescriptor.SingletonInstance == null)
                throw new Exception("Handler descriptor has no singleton instance.");

            handlersList.Add(handlerDescriptor);
        }

        /// <summary>
        /// Disposes of the handler token by removing the handler descriptor from the list.
        /// </summary>
        public readonly void Dispose()
        {
            handlersList.Remove(handlerDescriptor.Indexer);
        }
    }
}
