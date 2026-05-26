using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering InlineQuery updates.
/// </summary>
public abstract class InlineQueryFilterAttribute(params IFilter<InlineQuery>[] filters) : UpdateFilterAttribute<InlineQuery>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.InlineQuery];

    /// <inheritdoc/>
    public override InlineQuery? GetFilterringTarget(Update update)
        => update.InlineQuery;
}


/// <summary>
/// Attribute for filtering <see cref="InlineQuery"/>'s text
/// </summary>
public class InlineQueryTextAttribute(string text)
    : InlineQueryFilterAttribute(new InlineQueryTextFilter(text))
{ }

/// <summary>
/// Attribute for filtering <see cref="InlineQuery"/>'s text that contains specific string
/// </summary>
public class InlineQueryContainsAttribute(string text)
    : InlineQueryFilterAttribute(new InlineQueryContainsFilter(text))
{ }

/// <summary>
/// Attribute for filtering <see cref="InlineQuery"/>'s text that starts with specific string
/// </summary>
public class InlineQueryStartsWithAttribute(string text)
    : InlineQueryFilterAttribute(new InlineQueryStartsWithFilter(text))
{ }

/// <summary>
/// Attribute for filtering <see cref="InlineQuery"/>'s text that ends with specific string
/// </summary>
public class InlineQueryEndsWithAttribute(string text)
    : InlineQueryFilterAttribute(new InlineQueryEndsWithFilter(text))
{ }

/// <summary>
/// Attribute for filtering <see cref="InlineQuery"/>'s text with Regex
/// </summary>
public class InlineQueryRegexAttribute(string pattern)
    : InlineQueryFilterAttribute(new InlineQueryRegexFilter(pattern))
{ }
