using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process DeletedBusinessMessagesHandler updates.
/// </summary>
public class DeletedBusinessMessagesHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<DeletedBusinessMessagesHandler>(UpdateType.DeletedBusinessMessages, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { DeletedBusinessMessages: { } };
}

/// <summary>
/// Abstract base class for handlers that process DeletedBusinessMessagesHandler updates.
/// </summary>
public abstract class DeletedBusinessMessagesHandler() : AbstractUpdateHandler<BusinessMessagesDeleted>(UpdateType.DeletedBusinessMessages)
{
}

