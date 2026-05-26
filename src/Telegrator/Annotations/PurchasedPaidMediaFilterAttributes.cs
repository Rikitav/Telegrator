using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;
using Telegram.Bot.Types.Payments;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering PurchasedPaidMedia updates.
/// </summary>
public abstract class PurchasedPaidMediaFilterAttribute(params IFilter<PaidMediaPurchased>[] filters) : UpdateFilterAttribute<PaidMediaPurchased>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.PurchasedPaidMedia];

    /// <inheritdoc/>
    public override PaidMediaPurchased? GetFilterringTarget(Update update)
        => update.PurchasedPaidMedia;
}

