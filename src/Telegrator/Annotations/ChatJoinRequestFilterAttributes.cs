using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ChatJoinRequest updates.
/// </summary>
public abstract class ChatJoinRequestFilterAttribute(params IFilter<ChatJoinRequest>[] filters) : UpdateFilterAttribute<ChatJoinRequest>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ChatJoinRequest];

    /// <inheritdoc/>
    public override ChatJoinRequest? GetFilterringTarget(Update update)
        => update.ChatJoinRequest;
}


/// <summary>
/// Attribute for filtering <see cref="ChatJoinRequest"/> by invite link used
/// </summary>
public class ChatJoinRequestInviteLinkAttribute(string inviteLink)
    : ChatJoinRequestFilterAttribute(new ChatJoinRequestInviteLinkFilter(inviteLink))
{ }
