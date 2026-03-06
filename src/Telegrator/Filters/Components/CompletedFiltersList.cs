using System.Collections;

namespace Telegrator.Filters.Components
{
    /// <summary>
    /// The list containing filters worked out during Polling to further obtain additional filtering information
    /// </summary>
    public class CompletedFiltersList : IEnumerable<IFilterCollectable>
    {
        private readonly List<IFilterCollectable> CompletedFilters = [];

        /// <summary>
        /// Adds the completed filter to the list.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update.</typeparam>
        /// <param name="filter">The filter to add.</param>
        public void Add<TUpdate>(IFilter<TUpdate> filter) where TUpdate : class
        {
            if (filter is AnonymousTypeFilter | filter is AnonymousCompiledFilter)
                return;

            if (!filter.IsCollectible)
                return;

            CompletedFilters.Add(filter);
        }

        /// <summary>
        /// Adds many completed filters to the list.
        /// </summary>
        /// <typeparam name="TUpdate">The type of update.</typeparam>
        /// <param name="filters">The filters to add.</param>
        public void AddRange<TUpdate>(IEnumerable<IFilter<TUpdate>> filters) where TUpdate : class
        {
            foreach (IFilter<TUpdate> filter in filters)
                Add(filter);
        }

        /// <summary>
        /// Looks for filters of a given type in the list.
        /// </summary>
        /// <typeparam name="TFilter">The filter type to search for.</typeparam>
        /// <returns>The enumerable containing filters of the given type.</returns>
        /// <exception cref="NotFilterTypeException">Thrown if the type is not a filter type.</exception>
        public IEnumerable<TFilter> Get<TFilter>() where TFilter : notnull, IFilterCollectable
        {
            if (!typeof(TFilter).IsFilterType())
                throw new NotFilterTypeException(typeof(TFilter));

            return CompletedFilters.OfType<TFilter>();
        }

        /// <summary>
        /// Looks for a filter of a given type at the specified index in the list.
        /// </summary>
        /// <typeparam name="TFilter">The filter type to search for.</typeparam>
        /// <param name="index">The index of the filter.</param>
        /// <returns>The filter of the given type at the specified index.</returns>
        /// <exception cref="NotFilterTypeException">Thrown if the type is not a filter type.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if no filter is found at the index.</exception>
        public TFilter Get<TFilter>(int index) where TFilter : notnull, IFilterCollectable
        {
            IEnumerable<TFilter> filters = Get<TFilter>();
            return filters.Any() ? filters.ElementAt(index) : throw new KeyNotFoundException();
        }

        /// <summary>
        /// Returns a filter of a given type at the specified index, or null if it does not exist.
        /// </summary>
        /// <typeparam name="TFilter">The filter type to search for.</typeparam>
        /// <param name="index">The index of the filter.</param>
        /// <returns>The filter at the specified index, or null if it does not exist.</returns>
        /// <exception cref="NotFilterTypeException">Thrown if the type is not a filter type.</exception>
        public TFilter? GetOrDefault<TFilter>(int index) where TFilter : IFilterCollectable
        {
            IEnumerable<TFilter> filters = Get<TFilter>();
            return filters.Any() ? filters.ElementAt(index) : default;
        }

        /// <inheritdoc/>
        public IEnumerator<IFilterCollectable> GetEnumerator() => CompletedFilters.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => CompletedFilters.GetEnumerator();
    }
}
