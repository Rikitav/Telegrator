using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Attributes.Components
{
    /// <summary>
    /// Defines the <see cref="IFilter{T}"/> to <see cref="Update"/> validation for entry into execution of the <see cref="UpdateHandlerBase"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class UpdateFilterAttributeBase : Attribute
    {
        /// <summary>
        /// Gets the <see cref="UpdateType"/>'s that <see cref="UpdateHandlerBase"/> processing
        /// </summary>
        public abstract UpdateType[] AllowedTypes { get; }

        /// <summary>
        /// Gets the <see cref="IFilter{T}"/> that <see cref="UpdateHandlerBase"/> processing
        /// </summary>
        public abstract IFilter<Update> AnonymousFilter { get; protected set; }

        /// <summary>
        /// Gets or sets the filter modifiers that affect how this filter is combined with others.
        /// </summary>
        public FilterModifier Modifiers { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateHandlerAttributeBase"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        protected internal UpdateFilterAttributeBase()
        {
            if (AllowedTypes.Length == 0)
                throw new ArgumentException();
        }

        /// <summary>
        /// Determines the logic of filter modifiers. Exceptionally internal implementation</summary>
        /// <param name="previous"></param>
        /// <returns></returns>
        public abstract bool ProcessModifiers(UpdateFilterAttributeBase? previous);
    }
}
