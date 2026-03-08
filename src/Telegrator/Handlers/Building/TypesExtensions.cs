using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers.Building;
using Telegrator.Core.States;

namespace Telegrator.Handlers.Building
{
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

        /// <inheritdoc cref="HandlerBuilderBase.SetState{TKey, TValue}(TValue?)"/>
        public static TBuilder SetState<TBuilder, TKey, TValue>(this TBuilder handlerBuilder, TValue? myState)
            where TBuilder : HandlerBuilderBase
            where TKey : IStateKeyResolver, new()
            where TValue : IEquatable<TValue>
        {
            handlerBuilder.SetState<TKey, TValue>(myState);
            return handlerBuilder;
        }

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
}
