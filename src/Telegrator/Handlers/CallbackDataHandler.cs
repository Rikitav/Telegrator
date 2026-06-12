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
/// Attribute that marks a handler to process callback queries with specific data format.
/// Similar to CommandHandler, it can extract the main action and its arguments.
/// </summary>
public class CallbackDataHandlerAttribute(int importance = 1) : UpdateHandlerAttribute<CallbackDataHandler>(UpdateType.CallbackQuery, importance)
{
    /// <summary>
    /// Gets the action string that was extracted from the callback data.
    /// </summary>
    public string Action { get; private set; } = null!;

    /// <summary>
    /// Callback data split by a separator (e.g., ':')
    /// </summary>
    public string[]? Arguments { get; internal set; } = null;

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context)
    {
        if (context.Input.CallbackQuery is not { Data.Length: > 0 } callback)
            return false;

        string[] split = callback.Data.Split([':', ' ', '_'], StringSplitOptions.RemoveEmptyEntries);
        if (split.Length == 0)
            return false;

        Action = split[0];
        Arguments = split.Skip(1).ToArray();

        return true;
    }
}

/// <summary>
/// Abstract base class for handlers that process structured callback query data.
/// </summary>
public abstract class CallbackDataHandler : CallbackQueryHandler
{
    /// <summary>
    /// Gets the action that was extracted from the callback data.
    /// </summary>
    protected string Action
    {
        get => CompletedFilters.Get<CallbackDataHandlerAttribute>(0).Action;
    }

    /// <summary>
    /// Gets the callback data arguments as an array of strings.
    /// </summary>
    protected string[] Arguments
    {
        get => CompletedFilters.Get<CallbackDataHandlerAttribute>(0).Arguments ?? [];
    }
}

/// <summary>
/// Abstract base class for branching handlers that process structured callback query data.
/// </summary>
public abstract class BranchingCallbackDataHandler : BranchingCallbackQueryHandler
{
    /// <summary>
    /// Gets the action that was extracted from the callback data.
    /// </summary>
    protected string Action
    {
        get => CompletedFilters.Get<CallbackDataHandlerAttribute>(0).Action;
    }

    /// <summary>
    /// Gets the callback data arguments as an array of strings.
    /// </summary>
    protected string[] Arguments
    {
        get => CompletedFilters.Get<CallbackDataHandlerAttribute>(0).Arguments ?? [];
    }
}
