using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.States;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Container class that holds the context and data for handler execution.
    /// Provides access to the update, client, filters, and other execution context.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public class HandlerContainer<TUpdate> : IHandlerContainer<TUpdate> where TUpdate : class
    {
        /// <summary>
        /// Gets the actual update object of type TUpdate.
        /// </summary>
        public TUpdate ActualUpdate { get; }

        /// <inheritdoc/>
        public Update HandlingUpdate { get; }

        /// <inheritdoc/>
        public ITelegramBotClient Client { get; }

        /// <inheritdoc/>
        public Dictionary<string, object> ExtraData { get; }

        /// <inheritdoc/>
        public CompletedFiltersList CompletedFilters { get; }

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider { get; }

        /// <inheritdoc/>
        public IStateStorage StateStorage { get; }

        /// <summary>
        /// Initializes new instance of <see cref="HandlerContainer{TUpdate}"/>
        /// </summary>
        /// <param name="handlerInfo"></param>
        public HandlerContainer(DescribedHandlerDescriptor handlerInfo)
        {
            ActualUpdate = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();
            HandlingUpdate = handlerInfo.HandlingUpdate;
            Client = handlerInfo.Client;
            ExtraData = handlerInfo.ExtraData;
            CompletedFilters = handlerInfo.CompletedFilters;
            AwaitingProvider = handlerInfo.AwaitingProvider;
            StateStorage = handlerInfo.StateStorage;
        }

        /// <summary>
        /// Initializes new instance of <see cref="HandlerContainer{TUpdate}"/>
        /// </summary>
        /// <param name="actualUpdate"></param>
        /// <param name="handlingUpdate"></param>
        /// <param name="client"></param>
        /// <param name="extraData"></param>
        /// <param name="filters"></param>
        /// <param name="awaitingProvider"></param>
        /// <param name="stateStorage"></param>
        public HandlerContainer(TUpdate actualUpdate, Update handlingUpdate, ITelegramBotClient client, Dictionary<string, object> extraData, CompletedFiltersList filters, IAwaitingProvider awaitingProvider, IStateStorage stateStorage)
        {
            ActualUpdate = actualUpdate;
            HandlingUpdate = handlingUpdate;
            Client = client;
            ExtraData = extraData;
            CompletedFilters = filters;
            AwaitingProvider = awaitingProvider;
            StateStorage = stateStorage;
        }

        /// <summary>
        /// Creates new container of specific update type from thos contatiner
        /// </summary>
        /// <typeparam name="QUpdate"></typeparam>
        /// <returns></returns>
        public HandlerContainer<QUpdate> CreateChild<QUpdate>() where QUpdate : class
        {
            return new HandlerContainer<QUpdate>(
                HandlingUpdate.GetActualUpdateObject<QUpdate>(),
                HandlingUpdate, Client, ExtraData, CompletedFilters,
                AwaitingProvider, StateStorage);
        }

        /// <summary>
        /// Creates new container of specific update type from existing container
        /// </summary>
        /// <typeparam name="QUpdate"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static HandlerContainer<TUpdate> From<QUpdate>(IHandlerContainer<QUpdate> other) where QUpdate : class
        {
            return new HandlerContainer<TUpdate>(
                other.HandlingUpdate.GetActualUpdateObject<TUpdate>(),
                other.HandlingUpdate, other.Client, other.ExtraData, other.CompletedFilters,
                other.AwaitingProvider, other.StateStorage);
        }
    }
}
