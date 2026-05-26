using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering BusinessMessage updates.
/// </summary>
public abstract class BusinessMessageFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.BusinessMessage];

    /// <inheritdoc/>
    public override Message? GetFilterringTarget(Update update)
        => update.BusinessMessage;
}

