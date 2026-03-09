using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegrator.Handlers;

namespace Telegrator.Localized;

public static class LocalizedMessageHandlerExtensions
{
    public static async Task<Message> ResponseLocalized(this ILocalizedHandler<Message> localizedHandler, string localizedReplyIdentifier, params IEnumerable<string> formatArgs)
    {
        LocalizedString localizedString = localizedHandler.LocalizationProvider[localizedReplyIdentifier, formatArgs];
        return await localizedHandler.Container.Responce(localizedString.Value);
    }
}
