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
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks <see cref="Poll"/>'s type
/// </summary>
public class PollTypeFilter(PollType pollType) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.Type == pollType;
    }
}

/// <summary>
/// Filter that checks if <see cref="Poll"/> is closed
/// </summary>
public class PollIsClosedFilter(bool isClosed = true) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.IsClosed == isClosed;
    }
}

/// <summary>
/// Filter that checks if <see cref="Poll"/> is anonymous
/// </summary>
public class PollIsAnonymousFilter(bool isAnonymous = true) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.IsAnonymous == isAnonymous;
    }
}
