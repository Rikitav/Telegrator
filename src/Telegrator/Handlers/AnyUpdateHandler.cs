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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process any type of update.
/// This handler will be triggered for all incoming updates regardless of their type.
/// </summary>
/// <param name="importance"></param>
public class AnyUpdateHandlerAttribute(int importance = -1) : UpdateHandlerAttribute<AnyUpdateHandler>(UpdateType.Unknown, importance)
{
    /// <summary>
    /// Always returns true, allowing any update to pass through this filter.
    /// </summary>
    /// <param name="context">The filter execution context (unused).</param>
    /// <returns>Always returns true to allow any update.</returns>
    public override bool CanPass(FilterExecutionContext<Update> context) => true;
}

/// <summary>
/// Abstract base class for handlers that can process any type of update.
/// Provides a foundation for creating handlers that respond to all incoming updates.
/// </summary>
public abstract class AnyUpdateHandler() : AbstractUpdateHandler<Update>(UpdateType.Unknown)
{

}

/// <summary>
/// Abstract base class for branching handlers that process AnyUpdateHandler updates.
/// </summary>
public abstract class BranchingAnyUpdateHandler() : BranchingUpdateHandler<Update>(UpdateType.Unknown)
{
}

