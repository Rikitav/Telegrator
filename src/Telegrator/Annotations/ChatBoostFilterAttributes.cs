using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering ChatBoost updates.
/// </summary>
public abstract class ChatBoostFilterAttribute(params IFilter<ChatBoostUpdated>[] filters) : UpdateFilterAttribute<ChatBoostUpdated>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.ChatBoost];

    /// <inheritdoc/>
    public override ChatBoostUpdated? GetFilterringTarget(Update update)
        => update.ChatBoost;
}

