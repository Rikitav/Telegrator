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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process photo messages only.
/// </summary>
public class PhotoMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PhotoMessageHandler>([typeof(BranchingPhotoMessageHandler)], UpdateType.Message, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.Message is { Type: MessageType.Photo, Photo.Length: > 0 };
}

/// <summary>
/// Abstract base class for handlers that process photo messages only.
/// </summary>
public abstract class PhotoMessageHandler : MessageHandler
{
    /// <summary>
    /// Gets the photos attached to the message.
    /// </summary>
    protected PhotoSize[] Photos => Input.Photo!;

    /// <summary>
    /// Gets the largest photo (best quality) attached to the message.
    /// </summary>
    protected PhotoSize LargestPhoto => Photos.OrderByDescending(p => p.FileSize).First();
}

/// <summary>
/// Abstract base class for branching handlers that process photo messages only.
/// </summary>
public abstract class BranchingPhotoMessageHandler : BranchingMessageHandler
{
    /// <summary>
    /// Gets the photos attached to the message.
    /// </summary>
    protected PhotoSize[] Photos => Input.Photo!;

    /// <summary>
    /// Gets the largest photo (best quality) attached to the message.
    /// </summary>
    protected PhotoSize LargestPhoto => Photos.OrderByDescending(p => p.FileSize).First();
}
