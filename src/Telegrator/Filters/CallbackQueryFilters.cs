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

using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter thet checks <see cref="CallbackQuery"/>'s data
/// </summary>
public class CallbackDataFilter : Filter<CallbackQuery>
{
    private readonly string _data;

    /// <summary>
    /// Initialize new instance of <see cref="CallbackDataFilter"/>
    /// </summary>
    /// <param name="data"></param>
    public CallbackDataFilter(string data)
    {
        _data = data;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        return context.Input.Data == _data;
    }
}

/// <summary>
/// Filter that checks if <see cref="CallbackQuery"/>'s data contains specific string
/// </summary>
public class CallbackDataContainsFilter : Filter<CallbackQuery>
{
    private readonly string _data;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Initialize new instance of <see cref="CallbackDataContainsFilter"/>
    /// </summary>
    public CallbackDataContainsFilter(string data, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        _data = data;
        _comparison = comparison;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        return context.Input.Data?.Contains(_data, _comparison) == true;
    }
}

/// <summary>
/// Filter that checks if <see cref="CallbackQuery"/>'s data starts with specific string
/// </summary>
public class CallbackDataStartsWithFilter : Filter<CallbackQuery>
{
    private readonly string _data;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Initialize new instance of <see cref="CallbackDataStartsWithFilter"/>
    /// </summary>
    public CallbackDataStartsWithFilter(string data, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        _data = data;
        _comparison = comparison;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        return context.Input.Data?.StartsWith(_data, _comparison) == true;
    }
}

/// <summary>
/// Filter that checks if <see cref="CallbackQuery"/>'s data ends with specific string
/// </summary>
public class CallbackDataEndsWithFilter : Filter<CallbackQuery>
{
    private readonly string _data;
    private readonly StringComparison _comparison;

    /// <summary>
    /// Initialize new instance of <see cref="CallbackDataEndsWithFilter"/>
    /// </summary>
    public CallbackDataEndsWithFilter(string data, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
    {
        _data = data;
        _comparison = comparison;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        return context.Input.Data?.EndsWith(_data, _comparison) == true;
    }
}
/// <summary>
/// Filter that checks if <see cref="CallbackQuery"/> belongs to a specific message
/// </summary>
public class CallbackInlineIdFilter : Filter<CallbackQuery>
{
    private readonly string _inlineMessageId;

    /// <summary>
    /// Initialize new instance of <see cref="CallbackInlineIdFilter"/>
    /// </summary>
    /// <param name="inlineMessageId"></param>
    public CallbackInlineIdFilter(string inlineMessageId)
    {
        _inlineMessageId = inlineMessageId;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
    {
        return context.Input.InlineMessageId == _inlineMessageId;
    }
}

/// <summary>
/// Filters callback queries by matching their data with a regular expression.
/// </summary>
public class CallbackRegexFilter : RegexFilterBase<CallbackQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackRegexFilter"/> class with a pattern and options.
    /// </summary>
    /// <param name="pattern">The regex pattern.</param>
    /// <param name="regexOptions">The regex options.</param>
    public CallbackRegexFilter(string pattern, RegexOptions regexOptions = default)
        : base(clb => clb.Data, pattern, regexOptions) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CallbackRegexFilter"/> class with a regex object.
    /// </summary>
    /// <param name="regex">The regex object.</param>
    public CallbackRegexFilter(Regex regex)
        : base(clb => clb.Data, regex) { }
}
