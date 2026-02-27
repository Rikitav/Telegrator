using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Building.Components;
using Telegrator.MadiatorCore;

namespace Telegrator.Handlers.Building
{
    /// <summary>
    /// Delegate for handler execution actions that take a container and cancellation token.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    /// <param name="container">The handler container with execution context.</param>
    /// <param name="cancellation">The cancellation token.</param>
    /// <returns>A task representing the asynchronous execution.</returns>
    public delegate Task<Result> AbstractHandlerAction<TUpdate>(IHandlerContainer<TUpdate> container, CancellationToken cancellation) where TUpdate : class;

    /// <summary>
    /// Builder class for creating regular handlers that can process updates.
    /// Provides fluent API for configuring filters, state keepers, and other handler properties.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update to handle.</typeparam>
    public class HandlerBuilder<TUpdate> : HandlerBuilderBase, IRegularHandlerBuilder<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerBuilder{TUpdate}"/> class.
        /// </summary>
        /// <param name="updateType">The type of update this handler will process.</param>
        /// <param name="handlerCollection">The collection to register the built handler with.</param>
        /// <exception cref="ArgumentException">Thrown when the update type is not valid for TUpdate.</exception>
        public HandlerBuilder(UpdateType updateType, IHandlersCollection handlerCollection) : base(typeof(BuildedAbstractHandler<TUpdate>), updateType, handlerCollection)
        {
            if (!updateType.IsValidUpdateObject<TUpdate>())
                throw new ArgumentException("\"UpdateType." + updateType + "\" is not valid type for \"" + nameof(TUpdate) + "\" update object", nameof(updateType));
        }

        /// <summary>
        /// Builds an abstract handler with the specified execution action.
        /// </summary>
        /// <param name="executeHandler">The delegate action to execute when the handler is invoked.</param>
        /// <exception cref="ArgumentNullException">Thrown when executeHandler is null.</exception>
        public IHandlersCollection Build(AbstractHandlerAction<TUpdate> executeHandler)
        {
            if (executeHandler == null)
                throw new ArgumentNullException(nameof(executeHandler));

            BuildedAbstractHandler<TUpdate> instance = new BuildedAbstractHandler<TUpdate>(UpdateType, executeHandler);
            BuildImplicitDescriptor(instance);
            return HandlerCollection!;
        }
    }
}
