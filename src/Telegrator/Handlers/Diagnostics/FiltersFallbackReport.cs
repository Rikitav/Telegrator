using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Diagnostics
{
    /// <summary>
    /// Represents a report of filter fallback information for debugging and error handling.
    /// Contains detailed information about which filters failed and why during handler execution.
    /// </summary>
    /// <param name="descriptor">The handler descriptor that generated this report.</param>
    /// <param name="context">The filter execution context.</param>
    public class FiltersFallbackReport(HandlerDescriptor descriptor, FilterExecutionContext<Update> context)
    {
        /// <summary>
        /// Gets the handler descriptor associated with this fallback report.
        /// </summary>
        public HandlerDescriptor Descriptor { get; } = descriptor;

        /// <summary>
        /// Gets the filter execution context that generated this report.
        /// </summary>
        public FilterExecutionContext<Update> Context { get; } = context;

        /// <summary>
        /// Gets or sets the fallback information for the update validator filter.
        /// </summary>
        public FilterFallbackInfo? UpdateValidator { get; set; }

        /// <summary>
        /// Gets or sets the fallback information for the state keeper validator filter.
        /// </summary>
        public FilterFallbackInfo? StateKeeperValidator { get; set; }

        /// <summary>
        /// Gets the list of fallback information for update filters that failed.
        /// </summary>
        public List<FilterFallbackInfo> UpdateFilters { get; } = [];

        /// <summary>
        /// Checks filter fail status by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool this[string name] => UpdateFilters.FirstOrDefault(f => f.Name == name)?.Failed ?? false;

        /// <summary>
        /// Creates new instance of <see cref="ReportInspector"/> with default filter state as FAILED.
        /// </summary>
        /// <returns></returns>
        public ReportInspector AllFailed()
        {
            return new ReportInspector(this, false);
        }

        /// <summary>
        /// Creates new instance of <see cref="ReportInspector"/> with default filter state as PASSED.
        /// </summary>
        /// <returns></returns>
        public ReportInspector AllPassed()
        {
            return new ReportInspector(this, true);
        }
    }
}
