using System.Globalization;
using Telegrator.Aspects;
using Telegrator.Core.Handlers;

namespace Telegrator;

/// <summary>
/// A handler aspect that sets and restores the current culture context
/// according to the <see cref="ICultureResolver"/>.
/// </summary>
public class LocalizedAspect : IPreProcessor, IPostProcessor
{
    private readonly ICultureResolver _cultureResolver;
    private CultureInfo? _previousCulture;
    private CultureInfo? _previousUICulture;

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
        _previousCulture = CultureInfo.CurrentCulture;
        _previousUICulture = CultureInfo.CurrentUICulture;

        CultureInfo resolvedCulture = await _cultureResolver.ResolveAsync(container);

        CultureInfo.CurrentCulture = resolvedCulture;
        CultureInfo.CurrentUICulture = resolvedCulture;

        return Result.Ok();
    }

    /// <inheritdoc/>
    public Task<Result> AfterExecution(IHandlerContainer container, System.Threading.CancellationToken cancellationToken)
    {
        CultureInfo.CurrentCulture = _previousCulture ?? CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = _previousUICulture ?? CultureInfo.InvariantCulture;
        return Task.FromResult(Result.Ok());
    }
}
