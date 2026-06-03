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
using Telegrator.Core.Handlers.Building;
using Telegrator.Core.States;

namespace Telegrator.Handlers.Building;

/// <summary>
/// Extension methods for handler builders.
/// Provides convenient methods for creating handlers and setting state keepers.
/// </summary>
public static partial class HandlerBuilderExtensions
{
    /// <inheritdoc cref="HandlerBuilderBase.SetUpdateValidating(UpdateValidateAction)"/>
    public static TBuilder SetUpdateValidating<TBuilder>(this TBuilder handlerBuilder, UpdateValidateAction updateValidateAction)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.SetUpdateValidating(updateValidateAction);
        return handlerBuilder;
    }

    /// <inheritdoc cref="HandlerBuilderBase.SetConcurreny(int)"/>
    public static TBuilder SetConcurreny<TBuilder>(this TBuilder handlerBuilder, int concurrency)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.SetConcurreny(concurrency);
        return handlerBuilder;
    }

    /// <inheritdoc cref="HandlerBuilderBase.SetPriority(int)"/>
    public static TBuilder SetPriority<TBuilder>(this TBuilder handlerBuilder, int priority)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.SetPriority(priority);
        return handlerBuilder;
    }

    /// <inheritdoc cref="HandlerBuilderBase.SetIndexer(int, int)"/>
    public static TBuilder SetIndexer<TBuilder>(this TBuilder handlerBuilder, int concurrency, int priority)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.SetIndexer(concurrency, priority);
        return handlerBuilder;
    }

    /// <inheritdoc cref="HandlerBuilderBase.AddFilter(IFilter{Update})"/>
    public static TBuilder AddFilter<TBuilder>(this TBuilder handlerBuilder, IFilter<Update> filter)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.AddFilter(filter);
        return handlerBuilder;
    }

    /// <inheritdoc cref="HandlerBuilderBase.AddFilters(IFilter{Update}[])"/>
    public static TBuilder AddFilters<TBuilder>(this TBuilder handlerBuilder, params IFilter<Update>[] filters)
        where TBuilder : HandlerBuilderBase
    {
        handlerBuilder.AddFilters(filters);
        return handlerBuilder;
    }

#pragma warning disable CS1574
    /// <inheritdoc cref="HandlerBuilderBase.SetState{TKey, TValue}(TValue?)"/>
    public static TBuilder SetState<TBuilder, TKey, TValue>(this TBuilder handlerBuilder, TValue? myState)
        where TBuilder : HandlerBuilderBase
        where TKey : IStateKeyResolver, new()
        where TValue : IEquatable<TValue>
    {
        handlerBuilder.SetState<TKey, TValue>(myState);
        return handlerBuilder;
    }
#pragma warning restore CS1574

    /// <summary>
    /// Adds a targeted filter for a specific filter target type.
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
    /// <param name="handlerBuilder"></param>
    /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
    /// <param name="filter">The filter to add.</param>
    /// <returns>The builder instance.</returns>
    public static TBuilder AddTargetedFilter<TBuilder, TFilterTarget>(this TBuilder handlerBuilder, Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter)
        where TBuilder : HandlerBuilderBase
        where TFilterTarget : class
    {
        handlerBuilder.AddTargetedFilter(getFilterringTarget, filter);
        return handlerBuilder;
    }

    /// <summary>
    /// Adds multiple targeted filters for a specific filter target type.
    /// </summary>
    /// <typeparam name="TBuilder"></typeparam>
    /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
    /// <param name="handlerBuilder"></param>
    /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
    /// <param name="filters">The filters to add.</param>
    /// <returns>The builder instance.</returns>
    public static TBuilder AddTargetedFilters<TBuilder, TFilterTarget>(this TBuilder handlerBuilder, Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters)
        where TBuilder : HandlerBuilderBase
        where TFilterTarget : class
    {
        handlerBuilder.AddTargetedFilters(getFilterringTarget, filters);
        return handlerBuilder;
    }
}
