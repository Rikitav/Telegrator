using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process photo messages only.
/// </summary>
public class PhotoMessageHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PhotoMessageHandler>(UpdateType.Message, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.Message is { Type: MessageType.Photo, Photo.Length: > 0 };
}

/// <summary>
/// Abstract base class for handlers that process photo messages only.
/// </summary>
public abstract class PhotoMessageHandler : MessageHandler
{
    /// <summary>
    /// Gets the photos attached to the message.
    /// </summary>
    protected PhotoSize[] Photos => Input.Photo!;

    /// <summary>
    /// Gets the largest photo (best quality) attached to the message.
    /// </summary>
    protected PhotoSize LargestPhoto => Photos.OrderByDescending(p => p.FileSize).First();
}

/// <summary>
/// Abstract base class for branching handlers that process photo messages only.
/// </summary>
public abstract class BranchingPhotoMessageHandler : BranchingMessageHandler
{
    /// <summary>
    /// Gets the photos attached to the message.
    /// </summary>
    protected PhotoSize[] Photos => Input.Photo!;

    /// <summary>
    /// Gets the largest photo (best quality) attached to the message.
    /// </summary>
    protected PhotoSize LargestPhoto => Photos.OrderByDescending(p => p.FileSize).First();
}
