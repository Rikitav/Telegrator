using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering BusinessMessagesDeleted updates.
/// </summary>
public abstract class BusinessMessagesDeletedFilterAttribute(params IFilter<BusinessMessagesDeleted>[] filters) : UpdateFilterAttribute<BusinessMessagesDeleted>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.DeletedBusinessMessages];

    /// <inheritdoc/>
    public override BusinessMessagesDeleted? GetFilterringTarget(Update update)
        => update.DeletedBusinessMessages;
}
