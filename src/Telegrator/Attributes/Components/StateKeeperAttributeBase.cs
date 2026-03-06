using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Attributes.Components
{
    /// <summary>
    /// Sets the state in which the <see cref="UpdateHandlerBase"/> can be executed
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class StateKeeperAttributeBase : Attribute, IFilter<Update>
    {
        /// <inheritdoc/>
        public bool IsCollectible => GetType().HasPublicProperties();

        /// <summary>
        /// Creates a new instance <see cref="StateKeeperBase{TKey, TState}"/>
        /// </summary>
        /// <param name="stateKeeperType"></param>
        /// <exception cref="ArgumentException"></exception>
        protected StateKeeperAttributeBase(Type stateKeeperType)
        {
            if (!stateKeeperType.IsAssignableToGenericType(typeof(StateKeeperBase<,>)))
                throw new ArgumentException(stateKeeperType + " is not a StateKeeperBase", nameof(stateKeeperType));
        }

        /// <summary>
        /// Realizes a <see cref="IFilter{T}"/> for validation of the current <see cref="StateKeeperBase{TKey, TState}"/> in the polling routing
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract bool CanPass(FilterExecutionContext<Update> context);
    }
}
