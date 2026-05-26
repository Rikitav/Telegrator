using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process document messages only.
/// </summary>
public class DocumentMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<DocumentMessageHandler>(UpdateType.Message, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.Message is { Type: MessageType.Document, Document: not null };
}

/// <summary>
/// Abstract base class for handlers that process document messages only.
/// </summary>
public abstract class DocumentMessageHandler : MessageHandler
{
    /// <summary>
    /// Gets the document attached to the message.
    /// </summary>
    protected Document Document => Input.Document!;
}

/// <summary>
/// Abstract base class for branching handlers that process document messages only.
/// </summary>
public abstract class BranchingDocumentMessageHandler : BranchingMessageHandler
{
    /// <summary>
    /// Gets the document attached to the message.
    /// </summary>
    protected Document Document => Input.Document!;
}
