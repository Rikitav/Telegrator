/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegram.Bot.Types;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;

namespace Telegrator.Handlers.Diagnostics;

/// <summary>
/// Represents a report of filter fallback information for debugging and error handling.
/// Contains detailed information about which filters failed and why during handler execution.
/// </summary>
/// <param name="descriptor">The handler descriptor that generated this report.</param>
/// <param name="context">The filter execution context.</param>
public class FiltersFallbackReport(HandlerDescriptor descriptor, FilterExecutionContext<Update> context)
{
    /// <summary>
    /// Gets the handler descriptor associated with this fallback report.
    /// </summary>
    public HandlerDescriptor Descriptor { get; } = descriptor;

    /// <summary>
    /// Gets the filter execution context that generated this report.
    /// </summary>
    public FilterExecutionContext<Update> Context { get; } = context;

    /// <summary>
    /// Gets or sets the fallback information for the update validator filter.
    /// </summary>
    public FilterFallbackInfo? UpdateValidator { get; set; }

    /// <summary>
    /// Gets or sets the fallback information for the state keeper validator filter.
    /// </summary>
    public FilterFallbackInfo? StateKeeperValidator { get; set; }

    /// <summary>
    /// Gets the list of fallback information for update filters that failed.
    /// </summary>
    public List<FilterFallbackInfo> UpdateFilters { get; } = [];

    /// <summary>
    /// Checks filter fail status by name
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool this[string name] => UpdateFilters.FirstOrDefault(f => f.Name == name)?.Failed ?? false;

    /// <summary>
    /// Creates new instance of <see cref="ReportInspector"/> with default filter state as FAILED.
    /// </summary>
    /// <returns></returns>
    public ReportInspector AllFailed()
    {
        return new ReportInspector(this, false);
    }

    /// <summary>
    /// Creates new instance of <see cref="ReportInspector"/> with default filter state as PASSED.
    /// </summary>
    /// <returns></returns>
    public ReportInspector AllPassed()
    {
        return new ReportInspector(this, true);
    }
}
