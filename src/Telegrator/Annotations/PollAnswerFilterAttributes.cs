using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering PollAnswer updates.
/// </summary>
public abstract class PollAnswerFilterAttribute(params IFilter<PollAnswer>[] filters) : UpdateFilterAttribute<PollAnswer>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.PollAnswer];

    /// <inheritdoc/>
    public override PollAnswer? GetFilterringTarget(Update update)
        => update.PollAnswer;
}

