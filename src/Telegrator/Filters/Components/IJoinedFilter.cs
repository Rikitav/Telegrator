namespace Telegrator.Filters.Components
{
    /// <summary>
    /// Represents a filter that joins multiple filters together.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public interface IJoinedFilter<T> : IFilter<T> where T : class
    {
        /// <summary>
        /// Gets the array of joined filters.
        /// </summary>
        public IFilter<T>[] Filters { get; }
    }
}
