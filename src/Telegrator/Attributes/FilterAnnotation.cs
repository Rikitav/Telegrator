using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.Filters.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Reactive way to implement a new <see cref="UpdateFilterAttribute{T}"/> of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FilterAnnotation<T> : UpdateFilterAttribute<T>, IFilter<T>, INamedFilter where T : class
    {
        /// <inheritdoc/>
        public virtual bool IsCollectible { get; } = false;

        /// <inheritdoc/>
        public override UpdateType[] AllowedTypes { get; } = typeof(T).GetAllowedUpdateTypes();

        /// <inheritdoc/>
        public string Name => GetType().Name;

        /// <summary>
        /// Initializes new instance of <see cref="FilterAnnotation{T}"/>
        /// </summary>
        public FilterAnnotation() : base()
        {
            UpdateFilter = Filter<T>.If(CanPass);
            AnonymousFilter = AnonymousTypeFilter.Compile(UpdateFilter, GetFilterringTarget);
        }

        /// <inheritdoc/>
        public override T? GetFilterringTarget(Update update)
            => update.GetActualUpdateObject<T>();

        /// <inheritdoc/>
        public abstract bool CanPass(FilterExecutionContext<T> context);
    }
}
