using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

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

