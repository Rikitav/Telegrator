using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers.Building
{
    /// <summary>
    /// Internal handler class that wraps a delegate action for execution.
    /// Used for dynamically created handlers that execute custom actions.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    internal class BuildedAbstractHandler<TUpdate> : AbstractUpdateHandler<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// The delegate action to execute when the handler is invoked.
        /// </summary>
        private readonly AbstractHandlerAction<TUpdate> HandlerAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildedAbstractHandler{TUpdate}"/> class.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update this handler processes.</param>
        /// <param name="handlerAction">The delegate action to execute.</param>
        public BuildedAbstractHandler(UpdateType handlingUpdateType, AbstractHandlerAction<TUpdate> handlerAction) : base(handlingUpdateType)
        {
            HandlerAction = handlerAction;
        }

        /// <summary>
        /// Executes the wrapped handler action.
        /// </summary>
        /// <param name="container">The handler container with execution context.</param>
        /// <param name="cancellation">The cancellation token.</param>
        /// <returns>A task representing the asynchronous execution.</returns>
        public override Task<Result> Execute(IHandlerContainer<TUpdate> container, CancellationToken cancellation)
            => HandlerAction.Invoke(container, cancellation);
    }
}
