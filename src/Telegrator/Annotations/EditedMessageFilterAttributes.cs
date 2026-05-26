using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering EditedMessage updates.
/// </summary>
public abstract class EditedMessageFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.EditedMessage];

    /// <inheritdoc/>
    public override Message? GetFilterringTarget(Update update)
        => update.EditedMessage;
}

