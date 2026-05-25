using System.Globalization;
using Telegrator.Core.Handlers;

namespace Telegrator.Localized;

/// <summary>
/// A default implementation of <see cref="ICultureResolver"/> that uses the standard language code from the user info.
/// </summary>
public class DefaultCultureResolver : ICultureResolver
{
    /// <inheritdoc/>
    public Task<CultureInfo> ResolveAsync(IHandlerContainer container)
    {
        string? languageCode = container.HandlingUpdate.GetSenderId() > 0
            ? container.HandlingUpdate.Message?.From?.LanguageCode ??
              container.HandlingUpdate.CallbackQuery?.From?.LanguageCode ??
              container.HandlingUpdate.InlineQuery?.From?.LanguageCode ??
              container.HandlingUpdate.ChosenInlineResult?.From?.LanguageCode
            : null;

        CultureInfo cultureInfo = !string.IsNullOrEmpty(languageCode)
            ? new CultureInfo(languageCode)
            : CultureInfo.CurrentCulture;

        return Task.FromResult(cultureInfo);
    }
}
