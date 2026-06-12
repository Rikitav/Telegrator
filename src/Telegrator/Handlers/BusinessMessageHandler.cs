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
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process BusinessMessageHandler updates.
/// </summary>
public class BusinessMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<BusinessMessageHandler>([typeof(BranchingBusinessMessageHandler)], UpdateType.BusinessMessage, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { BusinessMessage: { } };
}

/// <summary>
/// Abstract base class for handlers that process BusinessMessageHandler updates.
/// </summary>
public abstract class BusinessMessageHandler() : AbstractUpdateHandler<Message>(UpdateType.BusinessMessage)
{
}


/// <summary>
/// Abstract base class for branching handlers that process BusinessMessageHandler updates.
/// </summary>
public abstract class BranchingBusinessMessageHandler() : BranchingUpdateHandler<Message>(UpdateType.BusinessMessage)
{
}

