/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Extensions.Localization;
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
