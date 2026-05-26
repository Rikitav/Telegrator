using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process MessageReactionHandler updates.
/// </summary>
public class MessageReactionHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<MessageReactionHandler>(UpdateType.MessageReaction, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { MessageReaction: { } };
}

/// <summary>
/// Abstract base class for handlers that process MessageReactionHandler updates.
/// </summary>
public abstract class MessageReactionHandler() : AbstractUpdateHandler<MessageReactionUpdated>(UpdateType.MessageReaction)
{
}


/// <summary>
/// Abstract base class for branching handlers that process MessageReactionHandler updates.
/// </summary>
public abstract class BranchingMessageReactionHandler() : BranchingUpdateHandler<MessageReactionUpdated>(UpdateType.MessageReaction)
{
}

