using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegram.Bot.Types.Payments;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process PreCheckoutQueryHandler updates.
/// </summary>
public class PreCheckoutQueryHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PreCheckoutQueryHandler>(UpdateType.PreCheckoutQuery, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { PreCheckoutQuery: { } };
}

/// <summary>
/// Abstract base class for handlers that process PreCheckoutQueryHandler updates.
/// </summary>
public abstract class PreCheckoutQueryHandler() : AbstractUpdateHandler<PreCheckoutQuery>(UpdateType.PreCheckoutQuery)
{
}


/// <summary>
/// Abstract base class for branching handlers that process PreCheckoutQueryHandler updates.
/// </summary>
public abstract class BranchingPreCheckoutQueryHandler() : BranchingUpdateHandler<PreCheckoutQuery>(UpdateType.PreCheckoutQuery)
{
}

