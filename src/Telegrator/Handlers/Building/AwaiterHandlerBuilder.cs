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
            throw new Exception();

        HandlerProvider = handlerProvider;
        HandlingUpdate = handlingUpdate;
    }

    /// <summary>
    /// Awaits for an update of the specified type using the default sender ID resolver.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    /// <returns>The awaited update of type TUpdate.</returns>
    public async Task<TUpdate> Await(CancellationToken cancellationToken = default)
        => await Await(new SenderIdResolver(), cancellationToken);

    /// <summary>
    /// Awaits for an update of the specified type using a custom state key resolver.
    /// </summary>
    /// <param name="keyResolver">The state key resolver to use for filtering updates.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the wait operation.</param>
    /// <returns>The awaited update of type TUpdate.</returns>
    public async Task<TUpdate> Await(IStateKeyResolver keyResolver, CancellationToken cancellationToken = default)
    {
        string? handlingKey = keyResolver.ResolveKey(HandlingUpdate);
        if (handlingKey is null)
            throw new InvalidOperationException("Cannot await update with resolved key as NULL");

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
