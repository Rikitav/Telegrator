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

using Telegrator.Filters;
using Telegrator.Logging;

namespace Telegrator.Core.Filters;

/// <summary>
/// Represents a filter that composes multiple filters and passes only if all of them pass.
/// </summary>
/// <typeparam name="T">The type of the input for the filter.</typeparam>
public class CompiledFilter<T> : Filter<T>, INamedFilter where T : class
{
    private readonly IFilter<T>[] Filters;
    private readonly string _name;

    /// <summary>
    /// Gets the name of this compiled filter.
    /// </summary>
    public virtual string Name => _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledFilter{T}"/> class.
    /// </summary>
    /// <param name="filters">The filters to compose.</param>
    public CompiledFilter(params IFilter<T>[] filters)
    {
        _name = string.Join("+", filters.Select(fltr => fltr.GetType().Name));
        Filters = filters;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledFilter{T}"/> class with a custom name.
    /// </summary>
    /// <param name="name">The custom name for the compiled filter.</param>
    /// <param name="filters">The filters to compose.</param>
    public CompiledFilter(string name, params IFilter<T>[] filters)
    {
        _name = name;
        Filters = filters;
    }

    /// <summary>
    /// Determines whether all composed filters pass for the given context.
    /// </summary>
    /// <param name="context">The filter execution context.</param>
    /// <returns>True if all filters pass; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<T> context)
    {
        foreach (IFilter<T> filter in Filters)
        {
            if (!filter.CanPass(context))
            {
                if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                {
                    string handlerName = context.Data.TryGetValue("handler_name", out object? nameObj) ? nameObj?.ToString() ?? "Unknown" : "Unknown";
                    TelegratorLogging.LogTrace("{0} filter of {1} didnt pass! (Compiled)", filter.GetType().Name, handlerName);
                }

                return false;
            }

            context.CompletedFilters.Add(filter);
        }

        return true;
    }
}
