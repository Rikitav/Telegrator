using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Filters;
using Telegram.Bot.Types.Payments;

namespace Telegrator.Annotations;

/// <summary>
/// Abstract base attribute for filtering PreCheckoutQuery updates.
/// </summary>
public abstract class PreCheckoutQueryFilterAttribute(params IFilter<PreCheckoutQuery>[] filters) : UpdateFilterAttribute<PreCheckoutQuery>(filters)
{
    /// <inheritdoc/>
    public override UpdateType[] AllowedTypes => [UpdateType.PreCheckoutQuery];

    /// <inheritdoc/>
    public override PreCheckoutQuery? GetFilterringTarget(Update update)
        => update.PreCheckoutQuery;
}

