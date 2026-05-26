using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering EditedChannelPost updates.
/// </summary>
public abstract class EditedChannelPostFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.EditedChannelPost];

    /// <inheritdoc/>
    public override Message? GetFilterringTarget(Update update)
        => update.EditedChannelPost;
}

