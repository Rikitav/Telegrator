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
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers.Building;
using Telegrator.Core.States;

namespace Telegrator.Handlers.Building;

internal class AwaiterHandlerDeleter<TUpdate>(UpdateType updateType, Update handlingUpdate, IAwaitingProvider handlerProvider) : IAwaiterHandlerBuilder<TUpdate> where TUpdate : class
{
    public Task<TUpdate?> Await(IStateKeyResolver keyResolver, CancellationToken cancellationToken = default)
    {
        string? handlingKey = keyResolver.ResolveKey(handlingUpdate);
        if (handlingKey is null)
            throw new InvalidOperationException("Cannot await update with resolved key as NULL");

        if (!handlerProvider.TryGetDescriptorList(updateType, out HandlerDescriptorList? list) || list == null)
            return Task.FromResult<TUpdate?>(null);

        foreach (DescriptorIndexer handler in list.Where(x => x.UpdateType == updateType && typeof(IAwaiterHandlerBuilder<>).IsAssignableFrom(x.HandlerType)).Select(x => x.Indexer).ToArray())
            list.Remove(handler);

        return Task.FromResult<TUpdate?>(null);
    }

    public void AddFilter(IFilter<Update> filter) => throw new NotImplementedException();
    public void AddFilters(params IFilter<Update>[] filters) => throw new NotImplementedException();
    public void AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter) where TFilterTarget : class => throw new NotImplementedException();
    public void AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters) where TFilterTarget : class => throw new NotImplementedException();
    public void SetConcurreny(int concurrency) => throw new NotImplementedException();
    public void SetIndexer(int concurrency, int priority) => throw new NotImplementedException();
    public void SetPriority(int priority) => throw new NotImplementedException();
    public void SetState<TKey, TValue>(TValue? state) where TKey : IStateKeyResolver, new() where TValue : IEquatable<TValue> => throw new NotImplementedException();
    public void SetUpdateValidating(UpdateValidateAction validateAction) => throw new NotImplementedException();
}
