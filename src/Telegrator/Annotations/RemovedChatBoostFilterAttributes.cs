using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering RemovedChatBoost updates.
/// </summary>
public abstract class RemovedChatBoostFilterAttribute(params IFilter<ChatBoostRemoved>[] filters) : UpdateFilterAttribute<ChatBoostRemoved>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.RemovedChatBoost];

    /// <inheritdoc/>
    public override ChatBoostRemoved? GetFilterringTarget(Update update)
        => update.RemovedChatBoost;
}

