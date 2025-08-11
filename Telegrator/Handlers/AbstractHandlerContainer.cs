﻿using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Container class that holds the context and data for handler execution.
    /// Provides access to the update, client, filters, and other execution context.
    /// </summary>
    /// <typeparam name="TUpdate">The type of update being handled.</typeparam>
    public class AbstractHandlerContainer<TUpdate> : IAbstractHandlerContainer<TUpdate> where TUpdate : class
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

        /// <summary>
        /// Initializes new instance of <see cref="AbstractHandlerContainer{TUpdate}"/>
        /// </summary>
        /// <param name="handlerInfo"></param>
        public AbstractHandlerContainer(DescribedHandlerInfo handlerInfo)
        {
            ActualUpdate = handlerInfo.HandlingUpdate.GetActualUpdateObject<TUpdate>();
            HandlingUpdate = handlerInfo.HandlingUpdate;
            Client = handlerInfo.Client;
            ExtraData = handlerInfo.ExtraData;
            CompletedFilters = handlerInfo.CompletedFilters;
            AwaitingProvider = handlerInfo.AwaitingProvider;
        }

        /// <summary>
        /// Initializes new instance of <see cref="AbstractHandlerContainer{TUpdate}"/>
        /// </summary>
        /// <param name="actualUpdate"></param>
        /// <param name="handlingUpdate"></param>
        /// <param name="client"></param>
        /// <param name="extraData"></param>
        /// <param name="filters"></param>
        /// <param name="awaitingProvider"></param>
        public AbstractHandlerContainer(TUpdate actualUpdate, Update handlingUpdate, ITelegramBotClient client, Dictionary<string, object> extraData, CompletedFiltersList filters, IAwaitingProvider awaitingProvider)
        {
            ActualUpdate = actualUpdate;
            HandlingUpdate = handlingUpdate;
            Client = client;
            ExtraData = extraData;
            CompletedFilters = filters;
            AwaitingProvider = awaitingProvider;
        }

        /// <summary>
        /// Creates new container of specific update type from thos contatiner
        /// </summary>
        /// <typeparam name="QUpdate"></typeparam>
        /// <returns></returns>
        public AbstractHandlerContainer<QUpdate> CreateChild<QUpdate>() where QUpdate : class
        {
            return new AbstractHandlerContainer<QUpdate>(
                HandlingUpdate.GetActualUpdateObject<QUpdate>(),
                HandlingUpdate, Client, ExtraData,
                CompletedFilters, AwaitingProvider);
        }

        /// <summary>
        /// Creates new container of specific update type from existing container
        /// </summary>
        /// <typeparam name="QUpdate"></typeparam>
        /// <param name="other"></param>
        /// <returns></returns>
        public static AbstractHandlerContainer<TUpdate> From<QUpdate>(IAbstractHandlerContainer<QUpdate> other) where QUpdate : class
        {
            return new AbstractHandlerContainer<TUpdate>(
                other.HandlingUpdate.GetActualUpdateObject<TUpdate>(),
                other.HandlingUpdate, other.Client, other.ExtraData,
                other.CompletedFilters, other.AwaitingProvider);
        }
    }
}
