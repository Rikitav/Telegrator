using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering MyChatMember updates.
/// </summary>
public abstract class MyChatMemberFilterAttribute(params IFilter<ChatMemberUpdated>[] filters) : UpdateFilterAttribute<ChatMemberUpdated>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.MyChatMember];

    /// <inheritdoc/>
    public override ChatMemberUpdated? GetFilterringTarget(Update update)
        => update.MyChatMember;
}

