using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering BusinessConnection updates.
/// </summary>
public abstract class BusinessConnectionFilterAttribute(params IFilter<BusinessConnection>[] filters) : UpdateFilterAttribute<BusinessConnection>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.BusinessConnection];

    /// <inheritdoc/>
    public override BusinessConnection? GetFilterringTarget(Update update)
        => update.BusinessConnection;
}

