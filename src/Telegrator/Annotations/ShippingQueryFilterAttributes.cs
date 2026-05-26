using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;
using Telegram.Bot.Types.Payments;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ShippingQuery updates.
/// </summary>
public abstract class ShippingQueryFilterAttribute(params IFilter<ShippingQuery>[] filters) : UpdateFilterAttribute<ShippingQuery>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ShippingQuery];

    /// <inheritdoc/>
    public override ShippingQuery? GetFilterringTarget(Update update)
        => update.ShippingQuery;
}

