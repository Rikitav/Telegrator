using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Base class for building handler descriptors and managing handler filters.
    /// </summary>
    public abstract class HandlerBuilderBase(Type buildingHandlerType, UpdateType updateType, IHandlersCollection? handlerCollection) : IHandlerBuilder
    {
        private static int HandlerServiceKeyIndex = 0;

        /// <summary>
        /// <see cref="IHandlersCollection"/> to ehich new builded handlers is adding
        /// </summary>
        protected readonly IHandlersCollection? HandlerCollection = handlerCollection;

        /// <summary>
        /// <see cref="UpdateType"/> of building handler
        /// </summary>
        protected readonly UpdateType UpdateType = updateType;
        
        /// <summary>
        /// Type of handler to build
        /// </summary>
        protected readonly Type BuildingHandlerType = buildingHandlerType;
        
        /// <summary>
        /// Filters applied to handler
        /// </summary>
        protected readonly List<IFilter<Update>> Filters = [];

        /// <summary>
        /// <see cref="DescriptorIndexer"/> of building handler
        /// </summary>
        protected DescriptorIndexer Indexer = new DescriptorIndexer(0, 0, 0);
        
        /// <summary>
        /// Update validation filter of building handler
        /// </summary>
        protected IFilter<Update>? ValidateFilter;
        
        /// <summary>
        /// State keeper of building handler
        /// </summary>
        protected IFilter<Update>? StateKeeper;

        /// <summary>
        /// Builds an implicit <see cref="HandlerDescriptor"/> for the specified handler instance.
        /// </summary>
        /// <param name="instance">The <see cref="UpdateHandlerBase"/> instance.</param>
        /// <returns>The created <see cref="HandlerDescriptor"/>.</returns>
        protected HandlerDescriptor BuildImplicitDescriptor(UpdateHandlerBase instance)
        {
            object handlerServiceKey = GetImplicitHandlerServiceKey(BuildingHandlerType);

            HandlerDescriptor descriptor = new HandlerDescriptor(
                DescriptorType.Implicit, BuildingHandlerType,
                UpdateType, Indexer, ValidateFilter,
                Filters.ToArray(), StateKeeper,
                handlerServiceKey, instance);

            HandlerCollection?.AddDescriptor(descriptor);
            return descriptor;
        }

        /// <summary>
        /// Gets a unique service key for an implicit handler type.
        /// </summary>
        /// <param name="BuildingHandlerType">The handler type.</param>
        /// <returns>A unique service key string.</returns>
        public static object GetImplicitHandlerServiceKey(Type BuildingHandlerType)
            => string.Format("ImplicitHandler_{0}+{1}", HandlerServiceKeyIndex++, BuildingHandlerType.Name);

        /// <summary>
        /// Sets the update validating action for the handler.
        /// </summary>
        /// <param name="validateAction">The <see cref="UpdateValidateAction"/> to use.</param>
        /// <returns>The builder instance.</returns>
        public void SetUpdateValidating(UpdateValidateAction validateAction)
        {
            ValidateFilter = new UpdateValidateFilter(validateAction);
        }

        /// <summary>
        /// Sets the concurrency level for the handler.
        /// </summary>
        /// <param name="concurrency">The concurrency value.</param>
        /// <returns>The builder instance.</returns>
        public void SetConcurreny(int concurrency)
        {
            Indexer = Indexer.UpdateImportance(concurrency);
        }

        /// <summary>
        /// Sets the priority for the handler.
        /// </summary>
        /// <param name="priority">The priority value.</param>
        /// <returns>The builder instance.</returns>
        public void SetPriority(int priority)
        {
            Indexer = Indexer.UpdatePriority(priority);
        }

        /// <summary>
        /// Sets both concurrency and priority for the handler.
        /// </summary>
        /// <param name="concurrency">The concurrency value.</param>
        /// <param name="priority">The priority value.</param>
        /// <returns>The builder instance.</returns>
        public void SetIndexer(int concurrency, int priority)
        {
            Indexer = new DescriptorIndexer(0, concurrency, priority);
        }

        /// <summary>
        /// Adds a filter to the handler.
        /// </summary>
        /// <param name="filter">The <see cref="IFilter{Update}"/> to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddFilter(IFilter<Update> filter)
        {
            Filters.Add(filter);
        }

        /// <summary>
        /// Adds multiple filters to the handler.
        /// </summary>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddFilters(params IFilter<Update>[] filters)
        {
            Filters.AddRange(filters);
        }

        /// <summary>
        /// Sets a state keeper for the handler using a specific state and key resolver.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="myState">The state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <returns>The builder instance.</returns>
        public void SetStateKeeper<TKey, TState, TKeeper>(TState myState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(myState, keyResolver);
        }

        /// <summary>
        /// Sets a state keeper for the handler using a special state and key resolver.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TState">The type of the state.</typeparam>
        /// <typeparam name="TKeeper">The type of the state keeper.</typeparam>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <returns>The builder instance.</returns>
        public void SetStateKeeper<TKey, TState, TKeeper>(SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            StateKeeper = new StateKeepFilter<TKey, TState, TKeeper>(specialState, keyResolver);
        }

        /// <summary>
        /// Adds a targeted filter for a specific filter target type.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filter">The filter to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddTargetedFilter<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, IFilter<TFilterTarget> filter) where TFilterTarget : class
        {
            AnonymousTypeFilter anonymousTypeFilter = AnonymousTypeFilter.Compile(filter, getFilterringTarget);
            Filters.Add(anonymousTypeFilter);
        }

        /// <summary>
        /// Adds multiple targeted filters for a specific filter target type.
        /// </summary>
        /// <typeparam name="TFilterTarget">The type of the filter target.</typeparam>
        /// <param name="getFilterringTarget">Function to get the filter target from an update.</param>
        /// <param name="filters">The filters to add.</param>
        /// <returns>The builder instance.</returns>
        public void AddTargetedFilters<TFilterTarget>(Func<Update, TFilterTarget?> getFilterringTarget, params IFilter<TFilterTarget>[] filters) where TFilterTarget : class
        {
            AnonymousCompiledFilter compiledPollingFilter = AnonymousCompiledFilter.Compile(filters, getFilterringTarget);
            Filters.Add(compiledPollingFilter);
        }
    }
}
