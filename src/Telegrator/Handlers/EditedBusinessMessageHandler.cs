using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process EditedBusinessMessageHandler updates.
/// </summary>
public class EditedBusinessMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<EditedBusinessMessageHandler>(UpdateType.EditedBusinessMessage, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { EditedBusinessMessage: { } };
}

/// <summary>
/// Abstract base class for handlers that process EditedBusinessMessageHandler updates.
/// </summary>
public abstract class EditedBusinessMessageHandler() : AbstractUpdateHandler<Message>(UpdateType.EditedBusinessMessage)
{
}


/// <summary>
/// Abstract base class for branching handlers that process EditedBusinessMessageHandler updates.
/// </summary>
public abstract class BranchingEditedBusinessMessageHandler() : BranchingUpdateHandler<Message>(UpdateType.EditedBusinessMessage)
{
}

