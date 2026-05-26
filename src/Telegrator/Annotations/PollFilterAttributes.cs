using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering Poll updates.
/// </summary>
public abstract class PollFilterAttribute(params IFilter<Poll>[] filters) : UpdateFilterAttribute<Poll>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.Poll];

    /// <inheritdoc/>
    public override Poll? GetFilterringTarget(Update update)
        => update.Poll;
}


/// <summary>
/// Attribute for filtering <see cref="Poll"/>'s type
/// </summary>
public class PollTypeAttribute(PollType pollType)
    : PollFilterAttribute(new PollTypeFilter(pollType))
{ }

/// <summary>
/// Attribute for filtering <see cref="Poll"/> by closed status
/// </summary>
public class PollIsClosedAttribute(bool isClosed = true)
    : PollFilterAttribute(new PollIsClosedFilter(isClosed))
{ }

/// <summary>
/// Attribute for filtering <see cref="Poll"/> by anonymous status
/// </summary>
public class PollIsAnonymousAttribute(bool isAnonymous = true)
    : PollFilterAttribute(new PollIsAnonymousFilter(isAnonymous))
{ }
