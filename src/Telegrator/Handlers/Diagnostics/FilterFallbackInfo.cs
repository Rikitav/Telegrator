using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Handlers.Diagnostics
{
    /// <summary>
    /// Contains information about a filter that failed during execution.
    /// Provides details about the filter, its failure status, and any associated exception.
    /// </summary>
    /// <param name="name">The name of the filter.</param>
    /// <param name="filter">The filter instance that failed.</param>
    /// <param name="failed">Whether the filter failed.</param>
    /// <param name="exception">The exception that occurred during filter execution, if any.</param>
    public class FilterFallbackInfo(string name, IFilter<Update> filter, bool failed, Exception? exception)
    {
        /// <summary>
        /// Gets the name of the filter.
        /// </summary>
        public string Name { get; } = name;

        /// <summary>
        /// Gets the filter instance that failed.
        /// </summary>
        public IFilter<Update> Filter { get; } = filter;

        /// <summary>
        /// Gets a value indicating whether the filter failed.
        /// </summary>
        public bool Failed { get; } = failed;

        /// <summary>
        /// Gets the exception that occurred during filter execution, if any.
        /// </summary>
        public Exception? Exception { get; } = exception;
    }
}
