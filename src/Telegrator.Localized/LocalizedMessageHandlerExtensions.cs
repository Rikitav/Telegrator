using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegrator.Handlers;

namespace Telegrator;

/// <summary>
/// Provides extension methods for localized message handlers.
/// </summary>
public static class LocalizedMessageHandlerExtensions
{
    /// <summary>
    /// Sends a localized response message using the handler's localization provider.
    /// </summary>
    public static async Task<Message> ResponseLocalized(this ILocalizedHandler<Message> localizedHandler, string localizedReplyIdentifier, params object[] formatArgs)
    {
        // 1. Resolve culture (this assumes we have access to it, but typically resolving the culture
        //    would be done by a middleware/aspect and set on CultureInfo.CurrentCulture.
        //    For simplicity here, we assume the environment/thread already has the resolved culture
        //    or we could explicitly use the string localizer.
        LocalizedString localizedString = localizedHandler.LocalizationProvider[localizedReplyIdentifier, formatArgs];
        return await localizedHandler.Container.Responce(localizedString.Value);
    }

    /// <summary>
    /// Sends a localized reply message using the handler's localization provider.
    /// </summary>
    public static async Task<Message> ReplyLocalized(this ILocalizedHandler<Message> localizedHandler, string localizedReplyIdentifier, params object[] formatArgs)
    {
        LocalizedString localizedString = localizedHandler.LocalizationProvider[localizedReplyIdentifier, formatArgs];
        return await localizedHandler.Container.Reply(localizedString.Value);
    }

    /// <summary>
    /// Gets the localized string for a specific key.
    /// </summary>
    public static string Localize<T>(this ILocalizedHandler<T> localizedHandler, string key, params object[] formatArgs) where T : class
    {
        return localizedHandler.LocalizationProvider[key, formatArgs].Value;
    }
}
