using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process BusinessMessagesDeletedHandler updates.
/// </summary>
public class BusinessMessagesDeletedHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<BusinessMessagesDeletedHandler>(UpdateType.DeletedBusinessMessages, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { DeletedBusinessMessages: { } };
}

/// <summary>
/// Abstract base class for handlers that process BusinessMessagesDeletedHandler updates.
/// </summary>
public abstract class BusinessMessagesDeletedHandler() : AbstractUpdateHandler<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages)
{
}

/// <summary>
/// Abstract base class for branching handlers that process BusinessMessagesDeletedHandler updates.
/// </summary>
public abstract class BranchingBusinessMessagesDeletedHandler() : BranchingUpdateHandler<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages)
{
}

