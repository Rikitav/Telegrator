using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process MyChatMemberHandler updates.
/// </summary>
public class MyChatMemberHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<MyChatMemberHandler>(UpdateType.MyChatMember, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { MyChatMember: { } };
}

/// <summary>
/// Abstract base class for handlers that process MyChatMemberHandler updates.
/// </summary>
public abstract class MyChatMemberHandler() : AbstractUpdateHandler<ChatMemberUpdated>(UpdateType.MyChatMember)
{
}


/// <summary>
/// Abstract base class for branching handlers that process MyChatMemberHandler updates.
/// </summary>
public abstract class BranchingMyChatMemberHandler() : BranchingUpdateHandler<ChatMemberUpdated>(UpdateType.MyChatMember)
{
}

