using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering MessageReaction updates.
/// </summary>
public abstract class MessageReactionFilterAttribute(params IFilter<MessageReactionUpdated>[] filters) : UpdateFilterAttribute<MessageReactionUpdated>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.MessageReaction];

    /// <inheritdoc/>
    public override MessageReactionUpdated? GetFilterringTarget(Update update)
        => update.MessageReaction;
}

