using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process text messages only.
/// </summary>
public class TextMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<TextMessageHandler>(UpdateType.Message, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.Message is { Type: MessageType.Text, Text.Length: > 0 };
}

/// <summary>
/// Abstract base class for handlers that process text messages only.
/// </summary>
public abstract class TextMessageHandler : MessageHandler
{
    /// <summary>
    /// Gets the text of the message.
    /// </summary>
    protected string Text => Input.Text!;
}

/// <summary>
/// Abstract base class for branching handlers that process text messages only.
/// </summary>
public abstract class BranchingTextMessageHandler : BranchingMessageHandler
{
    /// <summary>
    /// Gets the text of the message.
    /// </summary>
    protected string Text => Input.Text!;
}
