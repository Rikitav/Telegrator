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

namespace Telegrator;

/*
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
*/
