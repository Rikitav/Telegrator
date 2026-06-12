/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.States;
using Telegrator.Handlers;

namespace Telegrator.Core.Handlers;

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
    /// Storage of bot states.
    /// </summary>
    protected IStateStorage StateStorage => Container.StateStorage;

    /// <summary>
    /// Initializes a new instance and checks that the update type matches <typeparamref name="TUpdate"/>.
    /// </summary>
    /// <param name="handlingUpdateType">The type of update to handle.</param>
    protected AbstractUpdateHandler(UpdateType handlingUpdateType) : base(handlingUpdateType)
    {
        if (!HandlingUpdateType.IsValidUpdateObject<TUpdate>())
            throw new InvalidOperationException($"HandlingUpdateType {HandlingUpdateType} is not valid for {typeof(TUpdate).Name}.");
    }

    /// <summary>
    /// Creates a handler container for the specified awaiting provider and handler info.
    /// </summary>
    /// <param name="handlerInfo">The handler descriptor info.</param>
    /// <returns>The created handler container.</returns>
    public virtual IHandlerContainer CreateContainer(DescribedHandlerDescriptor handlerInfo)
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
