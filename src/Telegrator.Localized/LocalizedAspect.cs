using System.Globalization;
using Telegrator.Aspects;
using Telegrator.Core.Handlers;

namespace Telegrator;

/// <summary>
/// A handler pre-processor that intercepts execution and sets the current culture context
/// according to the <see cref="ICultureResolver"/>.
/// </summary>
public class LocalizedAspect : IPreProcessor
{
    private readonly ICultureResolver _cultureResolver;

    /// <summary>
    /// Initializes a new instance of <see cref="LocalizedAspect"/>.
    /// </summary>
    /// <param name="cultureResolver">The culture resolver used to get user language.</param>
    public LocalizedAspect(ICultureResolver cultureResolver)
    {
        _cultureResolver = cultureResolver;
    }

    /// <inheritdoc/>
    public async Task<Result> BeforeExecution(IHandlerContainer container, System.Threading.CancellationToken cancellationToken)
    {
        // Skip for non-localized handlers
        // To accurately skip, you'd typically need the handler instance from the container or rely on some other marker.
        // If we inject this processor only on ILocalizedHandler, we might not need this check.
        // Assuming we always resolve culture if the aspect is applied:

        CultureInfo resolvedCulture = await _cultureResolver.ResolveAsync(container);

        CultureInfo.CurrentCulture = resolvedCulture;
        CultureInfo.CurrentUICulture = resolvedCulture;

        return Result.Ok();
    }
}
