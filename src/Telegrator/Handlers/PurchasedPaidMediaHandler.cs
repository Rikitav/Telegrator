using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process PurchasedPaidMediaHandler updates.
/// </summary>
public class PurchasedPaidMediaHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PurchasedPaidMediaHandler>(UpdateType.PurchasedPaidMedia, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { PurchasedPaidMedia: { } };
}

/// <summary>
/// Abstract base class for handlers that process PurchasedPaidMediaHandler updates.
/// </summary>
public abstract class PurchasedPaidMediaHandler() : AbstractUpdateHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia)
{
}


/// <summary>
/// Abstract base class for branching handlers that process PurchasedPaidMediaHandler updates.
/// </summary>
public abstract class BranchingPurchasedPaidMediaHandler() : BranchingUpdateHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia)
{
}

