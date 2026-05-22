using Microsoft.Extensions.Localization;
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;

namespace Telegrator.Localized;

/// <summary>
/// Indicates that handler utilizes localization
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ILocalizedHandler<T> : IAbstractUpdateHandler<Message> where T : class
{
    /// <summary>
    /// Gets the localization provider associated with this handler.
    /// </summary>
    public IStringLocalizer LocalizationProvider { get; }
}
