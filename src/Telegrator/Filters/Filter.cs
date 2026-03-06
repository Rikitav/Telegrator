using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters, providing logical operations and collectability.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public abstract class Filter<T> : IFilter<T> where T : class
    {
        /// <summary>
        /// Creates a filter from a function.
        /// </summary>
        /// <param name="filter">The filter function.</param>
        /// <returns>A <see cref="Filter{T}"/> instance.</returns>
        public static Filter<T> If(Func<FilterExecutionContext<T>, bool> filter)
            => new FunctionFilter<T>(filter);

        /// <summary>
        /// Creates a filter that always passes.
        /// </summary>
        /// <returns>An <see cref="AnyFilter{T}"/> instance.</returns>
        public static AnyFilter<T> Any()
            => new AnyFilter<T>();

        /// <summary>
        /// Creates a filter that inverts the result of this filter.
        /// </summary>
        /// <returns>A <see cref="ReverseFilter{T}"/> instance.</returns>
        public Filter<T> Not()
            => new ReverseFilter<T>(this);

        /// <summary>
        /// Creates a filter that inverts the result of this filter.
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <returns>A <see cref="ReverseFilter{T}"/> instance.</returns>
        public static Filter<Q> Not<Q>(IFilter<Q> filter) where Q : class
            => new ReverseFilter<Q>(filter);

        /// <summary>
        /// Creates a filter that passes only if both this and the specified filter pass.
        /// </summary>
        /// <param name="filter">The filter to combine with.</param>
        /// <returns>An <see cref="AndFilter{T}"/> instance.</returns>
        public AndFilter<T> And(IFilter<T> filter)
            => new AndFilter<T>(this, filter);

        /// <summary>
        /// Creates a filter that passes if either this or the specified filter pass.
        /// </summary>
        /// <param name="filter">The filter to combine with.</param>
        /// <returns>An <see cref="OrFilter{T}"/> instance.</returns>
        public OrFilter<T> Or(IFilter<T> filter)
            => new OrFilter<T>(this, filter);

        /// <summary>
        /// Creates a filter that passes if either this or the specified filter pass.
        /// </summary>
        /// <typeparam name="Q"></typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns>An <see cref="OrFilter{Q}"/> instance.</returns>
        public static OrFilter<Q> Or<Q>(IFilter<Q> left, IFilter<Q> right) where Q : class
            => new OrFilter<Q>(left, right);

        /// <summary>
        /// Gets a value indicating whether this filter is collectible.
        /// </summary>
        public bool IsCollectible => GetType().HasPublicProperties();

        /// <summary>
        /// Determines whether the filter can pass for the given context.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        public abstract bool CanPass(FilterExecutionContext<T> context);

        /// <summary>
        /// Implicitly creates <see cref="IFilter{T}"/> from function
        /// </summary>
        /// <param name="filter"></param>
        public static implicit operator Filter<T>(Func<FilterExecutionContext<T>, bool> filter) => Filter<T>.If(filter);
    }

    /// <summary>
    /// A filter that always passes.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class AnyFilter<T> : Filter<T> where T : class
    {
        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<T> context)
            => true;
    }

    /// <summary>
    /// A filter that inverts the result of another filter.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class ReverseFilter<T> : Filter<T> where T : class
    {
        private readonly IFilter<T> filter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseFilter{T}"/> class.
        /// </summary>
        /// <param name="filter">The filter to invert.</param>
        public ReverseFilter(IFilter<T> filter)
            => this.filter = filter;

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<T> context)
            => !filter.CanPass(context);
    }

    /// <summary>
    /// A filter that uses a function to determine if it passes.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class FunctionFilter<T> : Filter<T> where T : class
    {
        private readonly Func<FilterExecutionContext<T>, bool>? FilterFunc;
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionFilter{T}"/> class.
        /// </summary>
        /// <param name="funcFilter">The filter function.</param>
        public FunctionFilter(Func<FilterExecutionContext<T>, bool> funcFilter)
            => FilterFunc = funcFilter;

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<T> context)
            => context.Input != null && FilterFunc != null && FilterFunc(context);
    }
}
