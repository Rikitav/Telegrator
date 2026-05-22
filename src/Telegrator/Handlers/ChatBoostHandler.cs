using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process ChatBoostHandler updates.
/// </summary>
public class ChatBoostHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<ChatBoostHandler>(UpdateType.ChatBoost, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ChatBoost: { } };
}

/// <summary>
/// Abstract base class for handlers that process ChatBoostHandler updates.
/// </summary>
public abstract class ChatBoostHandler() : AbstractUpdateHandler<ChatBoostUpdated>(UpdateType.ChatBoost)
{
}

