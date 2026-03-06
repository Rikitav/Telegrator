using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Diagnostics;
using Telegrator.Logging;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Represents a set of filters for a handler descriptor, including update and state keeper validators.
    /// </summary>
    public sealed class DescriptorFiltersSet
    {
        /// <summary>
        /// Validator for the update object.
        /// </summary>
        public IFilter<Update>? UpdateValidator { get; set; }

        /// <summary>
        /// Validator for the state keeper.
        /// </summary>
        public IFilter<Update>? StateKeeperValidator { get; set; }

        /// <summary>
        /// Array of update filters.
        /// </summary>
        public IFilter<Update>[]? UpdateFilters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorFiltersSet"/> class.
        /// </summary>
        /// <param name="updateValidator">Validator for the update object.</param>
        /// <param name="stateKeeperValidator">Validator for the state keeper.</param>
        /// <param name="updateFilters">Array of update filters.</param>
        public DescriptorFiltersSet(IFilter<Update>? updateValidator, IFilter<Update>? stateKeeperValidator, IFilter<Update>[]? updateFilters)
        {
            UpdateValidator = updateValidator;
            StateKeeperValidator = stateKeeperValidator;
            UpdateFilters = updateFilters;
        }

        /// <summary>
        /// Validates the filter context using all filters in the set.
        /// </summary>
        /// <param name="filterContext">The filter execution context.</param>
        /// <param name="formReport"></param>
        /// <param name="report"></param>
        /// <returns>True if all filters pass; otherwise, false.</returns>
        public Result Validate(FilterExecutionContext<Update> filterContext, bool formReport, ref FiltersFallbackReport report)
        {
            bool anyErrors = false;

            if (UpdateValidator != null)
            {
                bool result = ExecuteFilter(UpdateValidator, filterContext, out Exception? exc);

                if (formReport)
                {
                    report.UpdateValidator = new FilterFallbackInfo("Validator", UpdateValidator, !result, exc);
                }

                if (!result)
                {
                    anyErrors = true;
                    Alligator.LogTrace("(E) UpdateValidator filter of '{0}' for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);

                    if (!formReport)
                        return Result.Fault();
                }
                else
                {
                    filterContext.CompletedFilters.Add(UpdateValidator);
                }
            }

            if (StateKeeperValidator != null)
            {
                bool result = ExecuteFilter(StateKeeperValidator, filterContext, out Exception? exc);

                if (formReport)
                {
                    report.StateKeeperValidator = new FilterFallbackInfo("State", StateKeeperValidator, !result, exc);
                }

                if (!result)
                {
                    anyErrors = true;
                    Alligator.LogTrace("(E) StateKeeperValidator filter of '{0}' for Update ({1}) didnt pass!", filterContext.Data["handler_name"], filterContext.Update.Id);

                    if (!formReport)
                        return Result.Fault();
                }
                else
                {
                    filterContext.CompletedFilters.Add(StateKeeperValidator);
                }
            }

            if (UpdateFilters != null)
            {
                foreach (IFilter<Update> filter in UpdateFilters)
                {
                    bool result = ExecuteFilter(filter, filterContext, out Exception? exc);
                    string filterName = filter is INamedFilter named ? named.Name : filter.GetType().Name;

                    if (formReport)
                    {
                        report.UpdateFilters.Add(new FilterFallbackInfo(filterName, filter, !result, exc));
                    }

                    if (!result)
                    {
                        anyErrors = true;
                        Alligator.LogTrace("(E) '{0}' filter of '{1}' for Update ({2}) didnt pass!", filterName, filterContext.Data["handler_name"], filterContext.Update.Id);

                        if (!formReport)
                            return Result.Fault();
                    }
                    else
                    {
                        filterContext.CompletedFilters.Add(filter);
                    }
                }
            }

            if (!anyErrors)
                return Result.Ok();

            return formReport ? Result.Next() : Result.Fault();
        }

        private static bool ExecuteFilter<T>(IFilter<T> filter, FilterExecutionContext<T> context, out Exception? exception) where T : class
        {
            try
            {
                exception = null;
                return filter.CanPass(context);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
    }
}
