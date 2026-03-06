namespace Telegrator.Handlers.Diagnostics
{
    /// <summary>
    /// A class builder for pattern checking of <see cref="FiltersFallbackReport"/>
    /// </summary>
    /// <param name="report"></param>
    /// <param name="defaulState"></param>
    public sealed class ReportInspector(FiltersFallbackReport report, bool defaulState)
    {
        private readonly FiltersFallbackReport _report = report;
        private readonly bool _defaulState = defaulState;

        private readonly List<string> _ignore = [];
        private readonly List<string> _excepts = [];

        /// <summary>
        /// Adds a filter to the exclusion list.
        /// Excluded filters are compared oppositely with the default state.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ReportInspector Except(string name)
        {
            _excepts.Add(name);
            return this;
        }

        /// <summary>
        /// Adds a filter to the ignore list.
        /// Ignored filters are not checked and do not affect the final result.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ReportInspector Whenever(string name)
        {
            _ignore.Add(name);
            return this;
        }

        /// <summary>
        /// It goes through the report and compares it with the specified filter pattern.
        /// </summary>
        /// <returns></returns>
        public bool Match()
        {
            foreach (FilterFallbackInfo info in _report.UpdateFilters)
            {
                if (_ignore.Contains(info.Name))
                    continue;

                if (_excepts.Contains(info.Name))
                {
                    if (_defaulState == true && !info.Failed)
                        return false;

                    if (_defaulState == false && info.Failed)
                        return false;
                }

                if (!info.Failed != _defaulState)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Casts inspector by executing <see cref="Match"/>
        /// </summary>
        /// <param name="inspector"></param>
        public static implicit operator bool(ReportInspector inspector) => inspector.Match();
    }
}
