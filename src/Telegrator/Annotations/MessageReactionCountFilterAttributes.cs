using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering MessageReactionCount updates.
/// </summary>
public abstract class MessageReactionCountFilterAttribute(params IFilter<MessageReactionCountUpdated>[] filters) : UpdateFilterAttribute<MessageReactionCountUpdated>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.MessageReactionCount];

    /// <inheritdoc/>
    public override MessageReactionCountUpdated? GetFilterringTarget(Update update)
        => update.MessageReactionCount;
}

