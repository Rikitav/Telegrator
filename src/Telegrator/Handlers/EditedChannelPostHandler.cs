using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process EditedChannelPostHandler updates.
/// </summary>
public class EditedChannelPostHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<EditedChannelPostHandler>(UpdateType.EditedChannelPost, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { EditedChannelPost: { } };
}

/// <summary>
/// Abstract base class for handlers that process EditedChannelPostHandler updates.
/// </summary>
public abstract class EditedChannelPostHandler() : AbstractUpdateHandler<Message>(UpdateType.EditedChannelPost)
{
}


/// <summary>
/// Abstract base class for branching handlers that process EditedChannelPostHandler updates.
/// </summary>
public abstract class BranchingEditedChannelPostHandler() : BranchingUpdateHandler<Message>(UpdateType.EditedChannelPost)
{
}

