using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process BusinessConnectionHandler updates.
/// </summary>
public class BusinessConnectionHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<BusinessConnectionHandler>(UpdateType.BusinessConnection, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { BusinessConnection: { } };
}

/// <summary>
/// Abstract base class for handlers that process BusinessConnectionHandler updates.
/// </summary>
public abstract class BusinessConnectionHandler() : AbstractUpdateHandler<BusinessConnection>(UpdateType.BusinessConnection)
{
}


/// <summary>
/// Abstract base class for branching handlers that process BusinessConnectionHandler updates.
/// </summary>
public abstract class BranchingBusinessConnectionHandler() : BranchingUpdateHandler<BusinessConnection>(UpdateType.BusinessConnection)
{
}

