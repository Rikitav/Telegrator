using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters;
using Telegrator.StateKeeping;
using Telegrator.Handlers.Building.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.StateKeeping.Components;

namespace Telegrator.Handlers.Building
{
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
        public async Task<TUpdate> Await(IStateKeyResolver<long> keyResolver, CancellationToken cancellationToken = default)
        {
            Filters.Add(new StateKeyFilter<long>(keyResolver, keyResolver.ResolveKey(HandlingUpdate)));
            AwaiterHandler handlerInstance = new AwaiterHandler(UpdateType);
            HandlerDescriptor descriptor = BuildImplicitDescriptor(handlerInstance);
            
            using (HandlerProvider.UseHandler(descriptor))
            {
                handlerInstance.Wait(cancellationToken);
            }

            await Task.CompletedTask;
            return handlerInstance.HandlingUpdate.GetActualUpdateObject<TUpdate>();
        }
    }
}
