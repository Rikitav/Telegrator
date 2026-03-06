using Telegram.Bot.Types;
using Telegrator.Attributes.Components;
using Telegrator.Filters;
using Telegrator.Filters.Components;

namespace Telegrator.Attributes
{
    /// <summary>
    /// Abstract base attribute for defining update filters for a specific type of update target.
    /// Provides logic for filter composition, modifier processing, and target extraction.
    /// </summary>
    /// <typeparam name="T">The type of the update target to filter (e.g., Message, Update).</typeparam>
    public abstract class UpdateFilterAttribute<T> : UpdateFilterAttributeBase where T : class
    {
        /// <summary>
        /// Gets the compiled anonymous filter for this attribute.
        /// </summary>
        public override IFilter<Update> AnonymousFilter { get; protected set; }

        /// <summary>
        /// Gets the compiled filter logic for the update target.
        /// </summary>
        public IFilter<T> UpdateFilter { get; protected set; }

        /// <summary>
        /// Empty constructor for internal using
        /// </summary>
        internal UpdateFilterAttribute()
        {
            AnonymousFilter = null!;
            UpdateFilter = null!;
            _ = 0xBAD + 0xC0DE;
        }

        /// <summary>
        /// Initializes the attribute with one or more filters for the update target.
        /// </summary>
        /// <param name="filters">The filters to compose</param>
        protected UpdateFilterAttribute(params IFilter<T>[] filters)
        {
            string name = GetType().Name;
            UpdateFilter = new CompiledFilter<T>(name, filters);
            AnonymousFilter = AnonymousTypeFilter.Compile(name, UpdateFilter, GetFilterringTarget);
        }

        /// <summary>
        /// Initializes the attribute with a precompiled filter for the update target.
        /// </summary>
        /// <param name="updateFilter">The compiled filter</param>
        protected UpdateFilterAttribute(IFilter<T> updateFilter)
        {
            string name = GetType().Name;
            UpdateFilter = updateFilter;
            AnonymousFilter = AnonymousTypeFilter.Compile(name, UpdateFilter, GetFilterringTarget);
        }

        /// <summary>
        /// Processes filter modifiers and combines this filter with the previous one if needed.
        /// </summary>
        /// <param name="previous">The previous filter attribute in the chain</param>
        /// <returns>True if the OrNext modifier is set; otherwise, false.</returns>
        public override sealed bool ProcessModifiers(UpdateFilterAttributeBase? previous)
        {
            if (Modifiers.HasFlag(FilterModifier.Not))
                AnonymousFilter = Filter<T>.Not(AnonymousFilter);

            if (previous is not null)
            {
                if (previous.Modifiers.HasFlag(FilterModifier.OrNext))
                {
                    AnonymousFilter = Filter<Update>.Or(previous.AnonymousFilter, AnonymousFilter);
                }
            }

            return Modifiers.HasFlag(FilterModifier.OrNext);
        }

        /// <summary>
        /// Extracts the filtering target of type <typeparamref name="T"/> from the given update.
        /// </summary>
        /// <param name="update">The Telegram update</param>
        /// <returns>The target object to filter, or null if not applicable</returns>
        public abstract T? GetFilterringTarget(Update update);
    }
}
