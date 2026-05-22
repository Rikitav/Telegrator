using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process BusinessMessageHandler updates.
/// </summary>
public class BusinessMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<BusinessMessageHandler>(UpdateType.BusinessMessage, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { BusinessMessage: { } };
}

/// <summary>
/// Abstract base class for handlers that process BusinessMessageHandler updates.
/// </summary>
public abstract class BusinessMessageHandler() : AbstractUpdateHandler<Message>(UpdateType.BusinessMessage)
{
}

