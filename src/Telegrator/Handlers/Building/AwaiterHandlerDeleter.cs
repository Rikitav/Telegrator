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
    public Task<TUpdate> Await(IStateKeyResolver keyResolver, CancellationToken cancellationToken = default)
    {
        string? handlingKey = keyResolver.ResolveKey(handlingUpdate);
        if (handlingKey is null)
            throw new InvalidOperationException("Cannot await update with resolved key as NULL");

        if (!handlerProvider.TryGetDescriptorList(updateType, out HandlerDescriptorList? list) || list == null)
            return Task.FromResult<TUpdate>(null!);

        foreach (DescriptorIndexer handler in list.Where(x => x.UpdateType == updateType && typeof(IAwaiterHandlerBuilder<>).IsAssignableFrom(x.HandlerType)).Select(x => x.Indexer).ToArray())
            list.Remove(handler);

        return Task.FromResult<TUpdate>(null!);
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
