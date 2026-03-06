using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process any type of update.
    /// This handler will be triggered for all incoming updates regardless of their type.
    /// </summary>
    /// <param name="importance"></param>
    public class AnyUpdateHandlerAttribute(int importance = -1) : UpdateHandlerAttribute<AnyUpdateHandler>(UpdateType.Unknown, importance)
    {
        /// <summary>
        /// Always returns true, allowing any update to pass through this filter.
        /// </summary>
        /// <param name="context">The filter execution context (unused).</param>
        /// <returns>Always returns true to allow any update.</returns>
        public override bool CanPass(FilterExecutionContext<Update> context) => true;
    }

    /// <summary>
    /// Abstract base class for handlers that can process any type of update.
    /// Provides a foundation for creating handlers that respond to all incoming updates.
    /// </summary>
    public abstract class AnyUpdateHandler() : AbstractUpdateHandler<Update>(UpdateType.Unknown)
    {

    }
}
