using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Provides methods to retrieve and describe handler information for updates.
    /// </summary>
    public interface IHandlersProvider
    {
        /// <summary>
        /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
        /// </summary>
        public IEnumerable<UpdateType> AllowedTypes { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateType"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list);

        /// <summary>
        /// Instantiates a handler for the given descriptor, using the appropriate creation strategy based on descriptor type.
        /// Supports singleton, implicit, keyed, and general descriptor types with different instantiation patterns.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>An instance of <see cref="UpdateHandlerBase"/> for the descriptor</returns>
        public UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines whether the provider contains any handlers.
        /// </summary>
        /// <returns>True if the provider is empty; otherwise, false.</returns>
        public bool IsEmpty();
    }
}
