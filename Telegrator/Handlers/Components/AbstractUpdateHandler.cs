using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers.Components
{
    /// <summary>
    /// Abstract handler for Telegram updates of type <typeparamref name="TUpdate"/>.
    /// </summary>
    public abstract class AbstractUpdateHandler<TUpdate> : UpdateHandlerBase, IHandlerContainerFactory, IAbstractUpdateHandler<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Handler container for the current update.
        /// </summary>
        public IHandlerContainer<TUpdate> Container { get; private set; } = default!;

        /// <summary>
        /// Telegram Bot client associated with the current container.
        /// </summary>
        protected ITelegramBotClient Client => Container.Client;

        /// <summary>
        /// Incoming update of type <typeparamref name="TUpdate"/>.
        /// </summary>
        protected TUpdate Input => Container.ActualUpdate;

        /// <summary>
        /// The Telegram update being handled.
        /// </summary>
        protected Update HandlingUpdate => Container.HandlingUpdate;

        /// <summary>
        /// Additional data associated with the handler execution.
        /// </summary>
        protected Dictionary<string, object> ExtraData => Container.ExtraData;

        /// <summary>
        /// List of successfully passed filters.
        /// </summary>
        protected CompletedFiltersList CompletedFilters => Container.CompletedFilters;

        /// <summary>
        /// Provider for awaiting asynchronous operations.
        /// </summary>
        protected IAwaitingProvider AwaitingProvider => Container.AwaitingProvider;

        /// <summary>
        /// Initializes a new instance and checks that the update type matches <typeparamref name="TUpdate"/>.
        /// </summary>
        /// <param name="handlingUpdateType">The type of update to handle.</param>
        protected AbstractUpdateHandler(UpdateType handlingUpdateType) : base(handlingUpdateType)
        {
            if (!HandlingUpdateType.IsValidUpdateObject<TUpdate>())
                throw new Exception();
        }

        /// <summary>
        /// Creates a handler container for the specified awaiting provider and handler info.
        /// </summary>
        /// <param name="handlerInfo">The handler descriptor info.</param>
        /// <returns>The created handler container.</returns>
        public virtual IHandlerContainer CreateContainer(DescribedHandlerInfo handlerInfo)
        {
            return new HandlerContainer<TUpdate>(handlerInfo);
        }

        /// <summary>
        /// Executes the handler logic using the specified container.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected override sealed async Task<Result> ExecuteInternal(IHandlerContainer container, CancellationToken cancellationToken)
        {
            Container = (IHandlerContainer<TUpdate>)container;
            return await Execute(Container, cancellationToken);
        }

        /// <summary>
        /// Abstract method to execute the update handling logic.
        /// </summary>
        /// <param name="container">The handler container.</param>
        /// <param name="cancellation">Cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public abstract Task<Result> Execute(IHandlerContainer<TUpdate> container, CancellationToken cancellation);
    }
}
