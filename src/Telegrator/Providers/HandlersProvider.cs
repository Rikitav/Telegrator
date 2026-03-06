using System.Collections.ObjectModel;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Handlers.Components;
using Telegrator.Logging;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Provides handler resolution and instantiation logic for Telegram bot updates.
    /// Responsible for mapping update types to handler descriptors, filtering handlers based on update context,
    /// and creating handler instances with appropriate lifecycle management.
    /// </summary>
    public class HandlersProvider : IHandlersProvider
    {
        /// <inheritdoc/>
        public IEnumerable<UpdateType> AllowedTypes { get; }

        /// <summary>
        /// Read-only dictionary mapping <see cref="UpdateType"/> to lists of handler descriptors.
        /// Each descriptor list is frozen to prevent modification after initialization.
        /// </summary>
        protected readonly ReadOnlyDictionary<UpdateType, HandlerDescriptorList> HandlersDictionary;

        /// <summary>
        /// Configuration options for the bot and handler execution behavior.
        /// </summary>
        protected readonly TelegratorOptions Options;

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
        /// </summary>
        /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
        /// <param name="options">Configuration options for the bot and handler execution</param>
        /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
        public HandlersProvider(IHandlersCollection handlers, TelegratorOptions options)
        {
            AllowedTypes = handlers.AllowedTypes;
            HandlersDictionary = handlers.Values.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Alligator.LogTrace("{0} created!", GetType().Name);
        }

        /// <summary>
        /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
        /// </summary>
        /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
        /// <param name="options">Configuration options for the bot and handler execution</param>
        /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
        public HandlersProvider(IEnumerable<HandlerDescriptorList> handlers, TelegratorOptions options)
        {
            AllowedTypes = Update.AllTypes;
            HandlersDictionary = handlers.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Alligator.LogTrace("{0} created!", GetType().Name);
        }

        /// <inheritdoc/>
        /// <exception cref="Exception">Thrown when the descriptor type is not recognized</exception>
        public virtual UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            try
            {
                // Checking handler instance status
                cancellationToken.ThrowIfCancellationRequested();
                bool useSingleton = UseSingleton(descriptor);

                // Returning singleton instance
                if (useSingleton && descriptor.SingletonInstance != null)
                    return descriptor.SingletonInstance;

                // Creating instance
                UpdateHandlerBase instance = GetHandlerInstanceInternal(descriptor);
                if (useSingleton)
                    descriptor.TrySetInstance(instance);

                // Lazy initialization execution
                descriptor.LazyInitialization?.Invoke(instance);
                return instance;
            }
            catch (Exception ex)
            {
                Alligator.LogError("Failed to create instance of '{0}'", exception: ex, descriptor.ToString());
                throw;
            }
        }

        private static UpdateHandlerBase GetHandlerInstanceInternal(HandlerDescriptor descriptor)
        {
            if (descriptor.InstanceFactory != null)
                return descriptor.InstanceFactory.Invoke();

            return (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType);
        }

        private static bool UseSingleton(HandlerDescriptor descriptor) => descriptor.Type switch
        {
            DescriptorType.General or DescriptorType.Keyed => false,
            DescriptorType.Implicit or DescriptorType.Singleton => true,
            _ => throw new Exception("Unknown decriptor type")
        };

        /// <inheritdoc/>
        public virtual bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
        {
            if (UpdateTypeExtensions.SuppressTypes.TryGetValue(updateType, out UpdateType suppressType))
                updateType = suppressType;

            return HandlersDictionary.TryGetValue(updateType, out list);
        }

        /// <inheritdoc/>
        public virtual bool IsEmpty()
        {
            return HandlersDictionary.Count == 0;
        }
    }
}
