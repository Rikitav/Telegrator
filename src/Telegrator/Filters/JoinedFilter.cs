using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters that join multiple filters together.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public abstract class JoinedFilter<T>(params IFilter<T>[] filters) : Filter<T>, IJoinedFilter<T> where T : class
    {
        /// <summary>
        /// Gets the array of joined filters.
        /// </summary>
        public IFilter<T>[] Filters { get; } = filters;
    }

    /// <summary>
    /// A filter that passes only if both joined filters pass.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class AndFilter<T> : JoinedFilter<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndFilter{T}"/> class.
        /// </summary>
        /// <param name="leftFilter">The left filter.</param>
        /// <param name="rightFilter">The right filter.</param>
        public AndFilter(IFilter<T> leftFilter, IFilter<T> rightFilter)
            : base(leftFilter, rightFilter) { }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<T> context)
            => Filters[0].CanPass(context) && Filters[1].CanPass(context);
    }

    /// <summary>
    /// A filter that passes if at least one of the joined filters passes.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class OrFilter<T> : JoinedFilter<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrFilter{T}"/> class.
        /// </summary>
        /// <param name="leftFilter">The left filter.</param>
        /// <param name="rightFilter">The right filter.</param>
        public OrFilter(IFilter<T> leftFilter, IFilter<T> rightFilter)
            : base(leftFilter, rightFilter) { }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<T> context)
            => Filters[0].CanPass(context) || Filters[1].CanPass(context);
    }
}
