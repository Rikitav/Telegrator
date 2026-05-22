using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process ChatMemberHandler updates.
/// </summary>
public class ChatMemberHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<ChatMemberHandler>(UpdateType.ChatMember, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ChatMember: { } };
}

/// <summary>
/// Abstract base class for handlers that process ChatMemberHandler updates.
/// </summary>
public abstract class ChatMemberHandler() : AbstractUpdateHandler<ChatMemberUpdated>(UpdateType.ChatMember)
{
}

