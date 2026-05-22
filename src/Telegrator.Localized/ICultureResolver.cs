using System.Globalization;
using Telegrator.Core.Handlers;

namespace Telegrator;

/// <summary>
/// Service responsible for resolving the culture to use for localization based on the context of an incoming update.
/// </summary>
public interface ICultureResolver
{
    /// <summary>
    /// Resolves the <see cref="CultureInfo"/> asynchronously from the current container context.
    /// </summary>
    /// <param name="container">The current handler container for the executing update.</param>
    /// <returns>The resolved <see cref="CultureInfo"/>.</returns>
    Task<CultureInfo> ResolveAsync(IHandlerContainer container);
}
