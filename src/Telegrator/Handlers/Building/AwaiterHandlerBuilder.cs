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

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers.Building;
using Telegrator.Core.States;
using Telegrator.Filters;
using Telegrator.States;

namespace Telegrator.Handlers.Building;

/// <summary>
/// Builder class for creating awaiter handlers that can wait for specific update types.
/// Provides fluent API for configuring filters, state keepers, and other handler properties.
/// </summary>
/// <typeparam name="TUpdate">The type of update to await.</typeparam>
public class AwaiterHandlerBuilder<TUpdate> : HandlerBuilderBase, IAwaiterHandlerBuilder<TUpdate> where TUpdate : class
{
    /// <summary>
    /// The awaiting provider for managing handler registration.
    /// </summary>
    private readonly IAwaitingProvider HandlerProvider;

    /// <summary>
    /// The update that triggered the awaiter creation.
    /// </summary>
    private readonly Update HandlingUpdate;

    /// <summary>
    /// Initializes a new instance of the <see cref="AwaiterHandlerBuilder{TUpdate}"/> class.
    /// </summary>
    /// <param name="updateType">The type of update to await.</param>
    /// <param name="handlingUpdate">The update that triggered the awaiter creation.</param>
    /// <param name="handlerProvider">The awaiting provider for managing handler registration.</param>
    /// <exception cref="Exception">Thrown when the update type is not valid for TUpdate.</exception>
    public AwaiterHandlerBuilder(UpdateType updateType, Update handlingUpdate, IAwaitingProvider handlerProvider) : base(typeof(AwaiterHandler), updateType, null)
    {
        if (!updateType.IsValidUpdateObject<TUpdate>())
            throw new ArgumentException($"UpdateType {updateType} is not valid for {typeof(TUpdate).Name}.");

        HandlerProvider = handlerProvider;
        HandlingUpdate = handlingUpdate;
    }

    /// <summary>
    /// Awaits for an update of the specified type using the default sender ID resolver.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    /// <returns>The awaited update of type TUpdate.</returns>
    public async Task<TUpdate?> Await(CancellationToken cancellationToken = default)
        => await Await(new SenderIdResolver(), cancellationToken);

    /// <summary>
    /// Awaits for an update of the specified type using a custom state key resolver.
    /// </summary>
    /// <param name="keyResolver">The state key resolver to use for filtering updates.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    /// <returns>The awaited update of type TUpdate.</returns>
    public async Task<TUpdate?> Await(IStateKeyResolver keyResolver, CancellationToken cancellationToken = default)
    {
        string? handlingKey = keyResolver.ResolveKey(HandlingUpdate);
        if (handlingKey is null)
            throw new InvalidOperationException("Cannot await update with resolved key as NULL");

        Filters.Clear();
        Filters.Add(Filter<Update>.If(ctx =>
        {
            string? key = keyResolver.ResolveKey(ctx.Update);
            if (key is null)
                return false;

            return key == handlingKey;
        }));

        AwaiterHandler handlerInstance = new AwaiterHandler(UpdateType);
        HandlerDescriptor descriptor = BuildImplicitDescriptor(handlerInstance);

        using (HandlerProvider.UseHandler(descriptor))
        {
            await handlerInstance.Await(cancellationToken);
            return handlerInstance.HandlingUpdate.GetActualUpdateObject<TUpdate>();
        }
    }
}
