using Telegram.Bot.Types.Enums;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Provider for managing awaiting handlers that can wait for specific update types.
    /// Extends HandlersProvider to provide functionality for creating and managing awaiter handlers.
    /// </summary>
    /// <param name="options">The bot configuration options.</param>
    public class AwaitingProvider(TelegratorOptions options) : HandlersProvider([], options), IAwaitingProvider
    {
        /// <summary>
        /// List of handler descriptors for awaiting handlers.
        /// </summary>
        protected readonly HandlerDescriptorList HandlersList = [];

        /// <inheritdoc/>
        public override bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
        {
            list = HandlersList;
            return true;
        }

        /// <inheritdoc/>
        public IDisposable UseHandler(HandlerDescriptor handlerDescriptor)
        {
            HandlerToken handlerToken = new HandlerToken(HandlersList, handlerDescriptor);
            handlerToken.Register();
            return handlerToken;
        }

        /// <summary>
        /// Token for managing the lifetime of a handler in the awaiting provider.
        /// Implements IDisposable to automatically remove the handler when disposed.
        /// </summary>
        /// <param name="handlersList">The list of handler descriptors.</param>
        /// <param name="handlerDescriptor">The handler descriptor to manage.</param>
        private readonly struct HandlerToken(HandlerDescriptorList handlersList, HandlerDescriptor handlerDescriptor) : IDisposable
        {
            /// <summary>
            /// Registers the handler descriptor in the handlers list.
            /// </summary>
            /// <exception cref="Exception">Thrown when the handler descriptor has no singleton instance.</exception>
            public readonly void Register()
            {
                if (handlerDescriptor.SingletonInstance == null)
                    throw new Exception();

                handlersList.Add(handlerDescriptor);
            }

            /// <summary>
            /// Disposes of the handler token by removing the handler descriptor from the list.
            /// </summary>
            public readonly void Dispose()
            {
                handlersList.Remove(handlerDescriptor.Indexer);
            }
        }
    }
}
