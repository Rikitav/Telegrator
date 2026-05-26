using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ChatMember updates.
/// </summary>
public abstract class ChatMemberFilterAttribute(params IFilter<ChatMemberUpdated>[] filters) : UpdateFilterAttribute<ChatMemberUpdated>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ChatMember];

    /// <inheritdoc/>
    public override ChatMemberUpdated? GetFilterringTarget(Update update)
        => update.ChatMember;
}

