using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;

namespace Telegrator.Localized;

public interface ILocalizedHandler<T> : IAbstractUpdateHandler<Message> where T : class
{
    public IStringLocalizer LocalizationProvider { get; }
}
