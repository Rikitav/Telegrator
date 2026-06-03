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
using Telegrator.Core.States;

namespace Telegrator.Core.Handlers.Building;

/// <summary>
/// Defines builder actions for configuring handler builders.
/// </summary>
public interface IHandlerBuilder
{
    /// <summary>
    /// Sets the update validating action for the handler.
    /// </summary>
    /// <param name="validateAction">The <see cref="UpdateValidateAction"/> to use.</param>
    /// <returns>The builder instance.</returns>
    public void SetUpdateValidating(UpdateValidateAction validateAction);

    /// <summary>
    /// Sets the concurrency level for the handler.
    /// </summary>
    /// <param name="concurrency">The concurrency value.</param>
    /// <returns>The builder instance.</returns>
    public void SetConcurreny(int concurrency);

    /// <summary>
    /// Sets the priority for the handler.
    /// </summary>
    /// <param name="priority">The priority value.</param>
    /// <returns>The builder instance.</returns>
    public void SetPriority(int priority);

    /// <summary>
    /// Sets both concurrency and priority for the handler.
    /// </summary>
    /// <param name="concurrency">The concurrency value.</param>
    /// <param name="priority">The priority value.</param>
    /// <returns>The builder instance.</returns>
    public void SetIndexer(int concurrency, int priority);

    /// <summary>
    /// Adds a filter to the handler.
    /// </summary>
    /// <param name="filter">The <see cref="IFilter{Update}"/> to add.</param>
    /// <returns>The builder instance.</returns>
    public void AddFilter(IFilter<Update> filter);

    /// <summary>
    /// Adds multiple filters to the handler.
    /// </summary>
    /// <param name="filters">The filters to add.</param>
    /// <returns>The builder instance.</returns>
    public void AddFilters(params IFilter<Update>[] filters);

    /// <summary>
    /// Sets a state keeper for the handler using a specific state and key resolver.
    /// </summary>
    /// <typeparam name="TKey">The key resolver.</typeparam>
    /// <typeparam name="TValue">The state value.</typeparam>
    /// <param name="state">The state value.</param>
    /// <returns>The builder instance.</returns>
    public void SetState<TKey, TValue>(TValue? state)
        where TKey : IStateKeyResolver, new()
        where TValue : IEquatable<TValue>;

    /// <summary>
    /// Adds a targeted filter for a specific filter target type.
    /// </summary>
    /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
    /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
    /// <param name="filter">The filter to add.</param>
    /// <returns>The builder instance.</returns>
    public void AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter)
        where TFilterTarget : class;

    /// <summary>
    /// Adds multiple targeted filters for a specific filter target type.
    /// </summary>
    /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
    /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
    /// <param name="filters">The filters to add.</param>
    /// <returns>The builder instance.</returns>
    public void AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters)
        where TFilterTarget : class;
}
