using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process MessageReactionCountHandler updates.
/// </summary>
public class MessageReactionCountHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<MessageReactionCountHandler>(UpdateType.MessageReactionCount, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { MessageReactionCount: { } };
}

/// <summary>
/// Abstract base class for handlers that process MessageReactionCountHandler updates.
/// </summary>
public abstract class MessageReactionCountHandler() : AbstractUpdateHandler<MessageReactionCountUpdated>(UpdateType.MessageReactionCount)
{
}


/// <summary>
/// Abstract base class for branching handlers that process MessageReactionCountHandler updates.
/// </summary>
public abstract class BranchingMessageReactionCountHandler() : BranchingUpdateHandler<MessageReactionCountUpdated>(UpdateType.MessageReactionCount)
{
}

