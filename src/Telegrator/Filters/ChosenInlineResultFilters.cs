using System;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks <see cref="ChosenInlineResult"/>'s ID exactly
/// </summary>
public class ChosenInlineResultIdFilter(string resultId, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<ChosenInlineResult>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<ChosenInlineResult> context)
    {
        return context.Input.ResultId?.Equals(resultId, comparison) == true;
    }
}

/// <summary>
/// Filter that checks <see cref="ChosenInlineResult"/>'s original query exactly
/// </summary>
public class ChosenInlineResultQueryFilter(string query, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase) : Filter<ChosenInlineResult>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<ChosenInlineResult> context)
    {
        return context.Input.Query?.Equals(query, comparison) == true;
    }
}
