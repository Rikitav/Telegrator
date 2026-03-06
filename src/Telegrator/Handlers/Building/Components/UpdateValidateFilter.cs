using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Handlers.Building.Components
{
    /// <summary>
    /// Delegate for validating an update in a filter context.
    /// </summary>
    /// <param name="context">The filter execution context.</param>
    /// <returns>True if the update is valid; otherwise, false.</returns>
    public delegate bool UpdateValidateAction(FilterExecutionContext<Update> context);

    /// <summary>
    /// Filter that uses a delegate to validate updates.
    /// </summary>
    public class UpdateValidateFilter : IFilter<Update>
    {
        /// <summary>
        /// Gets a value indicating whether this filter is collectable. Always false for this filter.
        /// </summary>
        public bool IsCollectible => false;
        private readonly UpdateValidateAction UpdateValidateAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateValidateFilter"/> class.
        /// </summary>
        /// <param name="updateValidateAction">The validation delegate to use.</param>
        public UpdateValidateFilter(UpdateValidateAction updateValidateAction)
        {
            UpdateValidateAction = updateValidateAction;
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context using the validation delegate.
        /// </summary>
        /// <param name="info">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        public bool CanPass(FilterExecutionContext<Update> info)
            => UpdateValidateAction.Invoke(info);
    }
}
