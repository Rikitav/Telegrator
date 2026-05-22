using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process RemovedChatBoostHandler updates.
/// </summary>
public class RemovedChatBoostHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<RemovedChatBoostHandler>(UpdateType.RemovedChatBoost, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { RemovedChatBoost: { } };
}

/// <summary>
/// Abstract base class for handlers that process RemovedChatBoostHandler updates.
/// </summary>
public abstract class RemovedChatBoostHandler() : AbstractUpdateHandler<ChatBoostRemoved>(UpdateType.RemovedChatBoost)
{
}

