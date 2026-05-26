using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks if <see cref="ChatJoinRequest"/> was created using specific invite link
/// </summary>
public class ChatJoinRequestInviteLinkFilter(string inviteLink) : Filter<ChatJoinRequest>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<ChatJoinRequest> context)
    {
        return context.Input.InviteLink?.InviteLink == inviteLink;
    }
}
