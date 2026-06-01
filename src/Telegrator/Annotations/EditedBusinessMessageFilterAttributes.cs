using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering EditedBusinessMessage updates.
/// </summary>
public abstract class EditedBusinessMessageFilterAttribute(params IFilter<Message>[] filters) : UpdateFilterAttribute<Message>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.EditedBusinessMessage];

    /// <inheritdoc/>
    public override Message? GetFilterringTarget(Update update)
        => update.EditedBusinessMessage;
}

