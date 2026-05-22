using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process ChannelPostHandler updates.
/// </summary>
public class ChannelPostHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<ChannelPostHandler>(UpdateType.ChannelPost, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { ChannelPost: { } };
}

/// <summary>
/// Abstract base class for handlers that process ChannelPostHandler updates.
/// </summary>
public abstract class ChannelPostHandler() : AbstractUpdateHandler<Message>(UpdateType.ChannelPost)
{
}

