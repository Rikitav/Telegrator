using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ChannelPost updates.
/// </summary>
public abstract class ChannelPostFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ChannelPost];

    /// <inheritdoc/>
    public override Message? GetFilterringTarget(Update update)
        => update.ChannelPost;
}

