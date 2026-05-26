using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks <see cref="InlineQuery"/>'s text exactly
/// </summary>
public class InlineQueryTextFilter(string text, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<InlineQuery>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<InlineQuery> context)
    {
        return context.Input.Query?.Equals(text, comparison) == true;
    }
}

/// <summary>
/// Filter that checks if <see cref="InlineQuery"/>'s text contains specific string
/// </summary>
public class InlineQueryContainsFilter(string text, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<InlineQuery>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<InlineQuery> context)
    {
        return context.Input.Query?.Contains(text, comparison) == true;
    }
}

/// <summary>
/// Filter that checks if <see cref="InlineQuery"/>'s text starts with specific string
/// </summary>
public class InlineQueryStartsWithFilter(string text, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<InlineQuery>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<InlineQuery> context)
    {
        return context.Input.Query?.StartsWith(text, comparison) == true;
    }
}

/// <summary>
/// Filter that checks if <see cref="InlineQuery"/>'s text ends with specific string
/// </summary>
public class InlineQueryEndsWithFilter(string text, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<InlineQuery>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<InlineQuery> context)
    {
        return context.Input.Query?.EndsWith(text, comparison) == true;
    }
}

/// <summary>
/// Filters inline queries by matching their text with a regular expression.
/// </summary>
public class InlineQueryRegexFilter : RegexFilterBase<InlineQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InlineQueryRegexFilter"/> class with a pattern and options.
    /// </summary>
    /// <param name="pattern">The regex pattern.</param>
    /// <param name="regexOptions">The regex options.</param>
    public InlineQueryRegexFilter(string pattern, RegexOptions regexOptions = default)
        : base(q => q.Query, pattern, regexOptions) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="InlineQueryRegexFilter"/> class with a regex object.
    /// </summary>
    /// <param name="regex">The regex object.</param>
    public InlineQueryRegexFilter(Regex regex)
        : base(q => q.Query, regex) { }
}
