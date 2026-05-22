using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegram.Bot.Types.Payments;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process ShippingQueryHandler updates.
/// </summary>
public class ShippingQueryHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<ShippingQueryHandler>(UpdateType.ShippingQuery, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ShippingQuery: { } };
}

/// <summary>
/// Abstract base class for handlers that process ShippingQueryHandler updates.
/// </summary>
public abstract class ShippingQueryHandler() : AbstractUpdateHandler<ShippingQuery>(UpdateType.ShippingQuery)
{
}

