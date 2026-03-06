using Telegram.Bot.Types;
using Telegrator.Annotations.StateKeeping;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Building.Components;
using Telegrator.StateKeeping;
using Telegrator.StateKeeping.Components;

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

        /// <inheritdoc cref="HandlerBuilderBase.SetStateKeeper{TKey, TState, TKeeper}(TState, IStateKeyResolver{TKey})"/>
        public static TBuilder SetStateKeeper<TBuilder, TKey, TState, TKeeper>(this TBuilder handlerBuilder, TState myState, IStateKeyResolver<TKey> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            handlerBuilder.SetStateKeeper<TKey, TState, TKeeper>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <inheritdoc cref="HandlerBuilderBase.SetStateKeeper{TKey, TState, TKeeper}(SpecialState, IStateKeyResolver{TKey})"/>
        public static TBuilder SetStateKeeper<TBuilder, TKey, TState, TKeeper>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<TKey> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TKey : notnull
            where TState : IEquatable<TState>
            where TKeeper : StateKeeperBase<TKey, TState>, new()
        {
            handlerBuilder.SetStateKeeper<TKey, TState, TKeeper>(specialState, keyResolver);
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

        /// <summary>
        /// Sets a numeric state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, int myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The numeric state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, int myState)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(myState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets a numeric state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetNumericState<TBuilder>(this TBuilder handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
        {
            handlerBuilder.SetStateKeeper<long, int, NumericStateKeeper>(specialState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, TEnum myState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a special state and custom key resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <param name="keyResolver">The key resolver for the state.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, SpecialState specialState, IStateKeyResolver<long> keyResolver)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, keyResolver);
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="myState">The enum state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, TEnum myState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(myState, new SenderIdResolver());
            return handlerBuilder;
        }

        /// <summary>
        /// Sets an enum state keeper with a special state and the default sender ID resolver.
        /// </summary>
        /// <typeparam name="TBuilder">The type of the handler builder.</typeparam>
        /// <typeparam name="TEnum">The type of the enum state.</typeparam>
        /// <param name="handlerBuilder">The handler builder.</param>
        /// <param name="specialState">The special state value.</param>
        /// <returns>The handler builder for method chaining.</returns>
        public static TBuilder SetEnumState<TBuilder, TEnum>(this TBuilder handlerBuilder, SpecialState specialState)
            where TBuilder : HandlerBuilderBase
            where TEnum : Enum, IEquatable<TEnum>
        {
            handlerBuilder.SetStateKeeper<long, TEnum, EnumStateKeeper<TEnum>>(specialState, new SenderIdResolver());
            return handlerBuilder;
        }
    }
}
