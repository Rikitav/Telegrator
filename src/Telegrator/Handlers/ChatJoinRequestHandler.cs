using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process ChatJoinRequestHandler updates.
/// </summary>
public class ChatJoinRequestHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<ChatJoinRequestHandler>(UpdateType.ChatJoinRequest, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ChatJoinRequest: { } };
}

/// <summary>
/// Abstract base class for handlers that process ChatJoinRequestHandler updates.
/// </summary>
public abstract class ChatJoinRequestHandler() : AbstractUpdateHandler<ChatJoinRequest>(UpdateType.ChatJoinRequest)
{
}


/// <summary>
/// Abstract base class for branching handlers that process ChatJoinRequestHandler updates.
/// </summary>
public abstract class BranchingChatJoinRequestHandler() : BranchingUpdateHandler<ChatJoinRequest>(UpdateType.ChatJoinRequest)
{
}

