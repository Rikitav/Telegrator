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
using Telegrator.Handlers.Diagnostics;
using Telegrator.Logging;

namespace Telegrator.Core.Descriptors;

/// <summary>
/// Represents a set of filters for a handler descriptor, including update and state keeper validators.
/// </summary>
public sealed class DescriptorFiltersSet
{
    /// <summary>
    /// Validator for the update object.
    /// </summary>
    public IFilter<Update>? UpdateValidator { get; set; }

    /// <summary>
    /// Validator for the state keeper.
    /// </summary>
    public IFilter<Update>? StateKeeperValidator { get; set; }

    /// <summary>
    /// Array of update filters.
    /// </summary>
    public IFilter<Update>[]? UpdateFilters { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DescriptorFiltersSet"/> class.
    /// </summary>
    /// <param name="updateValidator">Validator for the update object.</param>
    /// <param name="stateKeeperValidator">Validator for the state keeper.</param>
    /// <param name="updateFilters">Array of update filters.</param>
    public DescriptorFiltersSet(IFilter<Update>? updateValidator, IFilter<Update>? stateKeeperValidator, IFilter<Update>[]? updateFilters)
    {
        UpdateValidator = updateValidator;
        StateKeeperValidator = stateKeeperValidator;
        UpdateFilters = updateFilters;
    }

    /// <summary>
    /// Validates the filter context using all filters in the set.
    /// </summary>
    /// <param name="filterContext">The filter execution context.</param>
    /// <param name="formReport"></param>
    /// <param name="report"></param>
    /// <returns>True if all filters pass; otherwise, false.</returns>
    public Result Validate(FilterExecutionContext<Update> filterContext, bool formReport, ref FiltersFallbackReport report)
    {
        bool anyErrors = false;

        if (UpdateValidator != null)
        {
            bool result = ExecuteFilter(UpdateValidator, filterContext, out Exception? exc);

            if (formReport)
            {
                report.UpdateValidator = new FilterFallbackInfo("Validator", UpdateValidator, !result, exc);
            }

            if (!result)
            {
                anyErrors = true;
                TelegratorLogging.LogTrace("(E) UpdateValidator filter of '{0}' for Update ({1}) didnt pass!", filterContext.Data.TryGetValue("handler_name", out object? handlerName) ? handlerName : "Unknown", filterContext.Update.Id);

                if (!formReport)
                    return Result.Fault();
            }
            else
            {
                filterContext.CompletedFilters.Add(UpdateValidator);
            }
        }

        if (StateKeeperValidator != null)
        {
            bool result = ExecuteFilter(StateKeeperValidator, filterContext, out Exception? exc);

            if (formReport)
            {
                report.StateKeeperValidator = new FilterFallbackInfo("State", StateKeeperValidator, !result, exc);
            }

            if (!result)
            {
                anyErrors = true;
                TelegratorLogging.LogTrace("(E) StateKeeperValidator filter of '{0}' for Update ({1}) didnt pass!", filterContext.Data.TryGetValue("handler_name", out object? handlerName) ? handlerName : "Unknown", filterContext.Update.Id);

                if (!formReport)
                    return Result.Fault();
            }
            else
            {
                filterContext.CompletedFilters.Add(StateKeeperValidator);
            }
        }

        if (UpdateFilters != null)
        {
            foreach (IFilter<Update> filter in UpdateFilters)
            {
                bool result = ExecuteFilter(filter, filterContext, out Exception? exc);
                string filterName = filter is INamedFilter named ? named.Name : filter.GetType().Name;

                if (formReport)
                {
                    report.UpdateFilters.Add(new FilterFallbackInfo(filterName, filter, !result, exc));
                }

                if (!result)
                {
                    anyErrors = true;
                    TelegratorLogging.LogTrace("(E) '{0}' filter of '{1}' for Update ({2}) didnt pass!", filterName, filterContext.Data.TryGetValue("handler_name", out object? handlerName) ? handlerName : "Unknown", filterContext.Update.Id);

                    if (!formReport)
                        return Result.Fault();
                }
                else
                {
                    filterContext.CompletedFilters.Add(filter);
                }
            }
        }

        if (!anyErrors)
            return Result.Ok();

        return formReport
            ? Result.Next() : Result.Fault();
    }

    private static bool ExecuteFilter<T>(IFilter<T> filter, FilterExecutionContext<T> context, out Exception? exception) where T : class
    {
        try
        {
            exception = null;
            return filter.CanPass(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }
}
