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

using Telegram.Bot.Types;
using Telegrator.Filters;
using Telegrator.Logging;

namespace Telegrator.Core.Filters;

/// <summary>
/// Represents a compiled filter that applies a set of filters to an anonymous target type.
/// </summary>
public class AnonymousCompiledFilter : Filter<Update>, INamedFilter
{
    private readonly Func<FilterExecutionContext<Update>, object, bool> FilterAction;
    private readonly Func<Update, object?> GetFilterringTarget;
    private readonly string _name;

    /// <summary>
    /// Gets the name of this compiled filter.
    /// </summary>
    public virtual string Name => _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnonymousCompiledFilter"/> class.
    /// </summary>
    /// <param name="name">The name of the compiled filter.</param>
    /// <param name="filterAction">The filter action delegate.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    private AnonymousCompiledFilter(string name, Func<Update, object?> getFilterringTarget, Func<FilterExecutionContext<Update>, object, bool> filterAction)
    {
        FilterAction = filterAction;
        GetFilterringTarget = getFilterringTarget;
        _name = name;
    }

    /// <summary>
    /// Compiles a set of filters into an <see cref="AnonymousCompiledFilter"/> for a specific target type.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="filters">The list of filters to compile.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    /// <returns>The compiled filter.</returns>
    public static AnonymousCompiledFilter Compile<T>(IEnumerable<IFilter<T>> filters, Func<Update, object?> getFilterringTarget) where T : class
    {
        return new AnonymousCompiledFilter(
            string.Join("+", filters.Select(fltr => fltr.GetType().Name)),
            getFilterringTarget,
            (context, filterringTarget) => CanPassInternal(context, filters, filterringTarget));
    }

    /// <summary>
    /// Compiles a set of filters into an <see cref="AnonymousCompiledFilter"/> for a specific target type with a custom name.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="name">The custom name for the compiled filter.</param>
    /// <param name="filters">The list of filters to compile.</param>
    /// <param name="getFilterringTarget">The function to get the filtering target from an update.</param>
    /// <returns>The compiled filter.</returns>
    public static AnonymousCompiledFilter Compile<T>(string name, IEnumerable<IFilter<T>> filters, Func<Update, object?> getFilterringTarget) where T : class
    {
        return new AnonymousCompiledFilter(
            name,
            getFilterringTarget,
            (context, filterringTarget) => CanPassInternal(context, filters, filterringTarget));
    }

    /// <summary>
    /// Determines whether all filters can pass for the given context and filtering target.
    /// </summary>
    /// <typeparam name="T">The type of the filtering target.</typeparam>
    /// <param name="filters">The list of filters.</param>
    /// <param name="updateContext">The filter execution context.</param>
    /// <param name="filterringTarget">The filtering target.</param>
    /// <returns>True if all filters pass; otherwise, false.</returns>
    private static bool CanPassInternal<T>(FilterExecutionContext<Update> updateContext, IEnumerable<IFilter<T>> filters, object filterringTarget) where T : class
    {
        FilterExecutionContext<T> context = updateContext.CreateChild((T)filterringTarget);
        foreach (IFilter<T> filter in filters)
        {
            if (!filter.CanPass(context))
            {
                if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                    TelegratorLogging.LogDebug("{0} filter of {1} didnt pass! (Compiled anonymous)", filter.GetType().Name, context.Data["handler_name"]);

                return false;
            }

            context.CompletedFilters.Add(filter);
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context)
    {
        try
        {
            object? filterringTarget = GetFilterringTarget.Invoke(context.Input);
            if (filterringTarget == null)
                return false;

            return FilterAction.Invoke(context, filterringTarget);
        }
        catch
        {
            return false;
        }
    }
}
