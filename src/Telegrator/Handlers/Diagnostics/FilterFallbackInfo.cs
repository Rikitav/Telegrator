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
using Telegrator.Core.Filters;

namespace Telegrator.Handlers.Diagnostics;

/// <summary>
/// Contains information about a filter that failed during execution.
/// Provides details about the filter, its failure status, and any associated exception.
/// </summary>
/// <param name="name">The name of the filter.</param>
/// <param name="filter">The filter instance that failed.</param>
/// <param name="failed">Whether the filter failed.</param>
/// <param name="exception">The exception that occurred during filter execution, if any.</param>
public class FilterFallbackInfo(string name, IFilter<Update> filter, bool failed, Exception? exception)
{
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the filter instance that failed.
    /// </summary>
    public IFilter<Update> Filter { get; } = filter;

    /// <summary>
    /// Gets a value indicating whether the filter failed.
    /// </summary>
    public bool Failed { get; } = failed;

    /// <summary>
    /// Gets the exception that occurred during filter execution, if any.
    /// </summary>
    public Exception? Exception { get; } = exception;
}
