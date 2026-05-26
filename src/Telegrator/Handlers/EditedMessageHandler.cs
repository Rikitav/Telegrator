using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process EditedMessageHandler updates.
/// </summary>
public class EditedMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<EditedMessageHandler>(UpdateType.EditedMessage, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { EditedMessage: { } };
}

/// <summary>
/// Abstract base class for handlers that process EditedMessageHandler updates.
/// </summary>
public abstract class EditedMessageHandler() : AbstractUpdateHandler<Message>(UpdateType.EditedMessage)
{
}


/// <summary>
/// Abstract base class for branching handlers that process EditedMessageHandler updates.
/// </summary>
public abstract class BranchingEditedMessageHandler() : BranchingUpdateHandler<Message>(UpdateType.EditedMessage)
{
}

