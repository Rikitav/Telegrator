using Telegrator.Logging;

namespace Telegrator.Filters.Components
{
    /// <summary>
    /// Represents a filter that composes multiple filters and passes only if all of them pass.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class CompiledFilter<T> : Filter<T>, INamedFilter where T : class
    {
        private readonly IFilter<T>[] Filters;
        private readonly string _name;

        /// <summary>
        /// Gets the name of this compiled filter.
        /// </summary>
        public virtual string Name => _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledFilter{T}"/> class.
        /// </summary>
        /// <param name="filters">The filters to compose.</param>
        public CompiledFilter(params IFilter<T>[] filters)
        {
            _name = string.Join("+", filters.Select(fltr => fltr.GetType().Name));
            Filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledFilter{T}"/> class with a custom name.
        /// </summary>
        /// <param name="name">The custom name for the compiled filter.</param>
        /// <param name="filters">The filters to compose.</param>
        public CompiledFilter(string name, params IFilter<T>[] filters)
        {
            _name = name;
            Filters = filters;
        }

        /// <summary>
        /// Determines whether all composed filters pass for the given context.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<T> context)
        {
            foreach (IFilter<T> filter in Filters)
            {
                if (!filter.CanPass(context))
                {
                    if (filter is not AnonymousCompiledFilter && filter is not AnonymousTypeFilter)
                        Alligator.LogTrace("{0} filter of {1} didnt pass! (Compiled)", filter.GetType().Name, context.Data["handler_name"]);

                    return false;
                }

                context.CompletedFilters.Add(filter);
            }
    
            return true;
        }
    }
}
