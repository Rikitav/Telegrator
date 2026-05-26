using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ChosenInlineResult updates.
/// </summary>
public abstract class ChosenInlineResultFilterAttribute(params IFilter<ChosenInlineResult>[] filters) : UpdateFilterAttribute<ChosenInlineResult>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ChosenInlineResult];

    /// <inheritdoc/>
    public override ChosenInlineResult? GetFilterringTarget(Update update)
        => update.ChosenInlineResult;
}


/// <summary>
/// Attribute for filtering <see cref="ChosenInlineResult"/>'s ID
/// </summary>
public class ChosenInlineResultIdAttribute(string resultId)
    : ChosenInlineResultFilterAttribute(new ChosenInlineResultIdFilter(resultId))
{ }

/// <summary>
/// Attribute for filtering <see cref="ChosenInlineResult"/>'s original query
/// </summary>
public class ChosenInlineResultQueryAttribute(string query)
    : ChosenInlineResultFilterAttribute(new ChosenInlineResultQueryFilter(query))
{ }
