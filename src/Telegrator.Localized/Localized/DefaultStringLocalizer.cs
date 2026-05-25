using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Telegrator.Core.Handlers;

namespace Telegrator.Localized;

public class DefaultStringLocalizer : IStringLocalizer
{
    public LocalizedString this[string name] => throw new NotImplementedException();

    public LocalizedString this[string name, params object[] arguments] => throw new NotImplementedException();

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => throw new NotImplementedException();
}
