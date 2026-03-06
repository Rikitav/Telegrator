using System.Reflection;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Configuration;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Provides functionality of incrementally collecting, organizing and resolving handlers instances.
    /// Minimum implementation of <see cref="IHandlersManager"/>. Abstract class, still requires handler instance resolving.
    /// </summary>
    /// <param name="options"></param>
    public abstract class HandlersManagerBase(ITelegratorOptions options) : IHandlersManager
    {
        /// <summary>
        /// Dictionary that organizes handler descriptors by update type.
        /// </summary>
        protected readonly Dictionary<UpdateType, HandlerDescriptorList> _handlersDictionary = [];

        /// <summary>
        /// Gets the collection of Telegram.Bot.Types.Enums.UpdateType's allowed by registered handlers
        /// </summary>
        protected readonly List<UpdateType> _allowedTypes = [];

        /// <summary>
        /// Configuration options for handler collecting.
        /// </summary>
        protected readonly ITelegratorOptions? Options = options;

        /// <summary>
        /// Gets whether handlers must have a parameterless constructor.
        /// </summary>
        protected virtual bool MustHaveParameterlessCtor => true;

        /// <summary>
        /// List of command aliases that have been registered.
        /// </summary>
        public readonly List<string> CommandAliasses = [];

        /// <inheritdoc/>
        public HandlerDescriptorList this[UpdateType updateType] => _handlersDictionary[updateType];

        /// <inheritdoc/>
        public IEnumerable<UpdateType> AllowedTypes => _allowedTypes;

        /// <inheritdoc/>
        public IEnumerable<UpdateType> Keys => _handlersDictionary.Keys;

        /// <inheritdoc/>
        public IEnumerable<HandlerDescriptorList> Values => _handlersDictionary.Values;

        /// <inheritdoc/>
        public virtual IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
        {
            if (MustHaveParameterlessCtor && !descriptor.HandlerType.HasParameterlessCtor())
                throw new Exception("This handler (" + descriptor.HandlerType.FullName + "), must contain constructor without parameters.");

            _allowedTypes.UnionAdd([descriptor.UpdateType]);
            MightAwaitAttribute? mightAwait = descriptor.HandlerType.GetCustomAttribute<MightAwaitAttribute>();
            if (mightAwait != null)
                _allowedTypes.UnionAdd(mightAwait.UpdateTypes);

            IntersectCommands(descriptor);
            HandlerDescriptorList list = GetDescriptorList(descriptor);

            list.Add(descriptor);
            return this;
        }

        /// <summary>
        /// Gets the <see cref="HandlerDescriptorList"/> for the specified <see cref="HandlerDescriptor"/>.
        /// </summary>
        /// <param name="descriptor">The handler descriptor.</param>
        /// <returns>The handler descriptor list containing the descriptor.</returns>
        protected virtual HandlerDescriptorList GetDescriptorList(HandlerDescriptor descriptor)
        {
            UpdateType updateType = UpdateTypeExtensions.SuppressTypes.TryGetValue(descriptor.UpdateType, out UpdateType suppressType)
                ? suppressType : descriptor.UpdateType;

            if (!_handlersDictionary.TryGetValue(updateType, out HandlerDescriptorList? list))
            {
                list = new HandlerDescriptorList(updateType, Options);
                _handlersDictionary.Add(updateType, list);
            }

            return list;
        }

        /// <summary>
        /// Checks for intersecting command aliases and handles them according to configuration.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to check for command aliases.</param>
        /// <exception cref="Exception">Thrown when intersecting command aliases are found and ExceptIntersectingCommandAliases is enabled.</exception>
        protected virtual void IntersectCommands(HandlerDescriptor descriptor)
        {
            if (Options == null || !Options.ExceptIntersectingCommandAliases)
                return;

            CommandAlliasAttribute? alliasAttribute = descriptor.HandlerType.GetCustomAttribute<CommandAlliasAttribute>();
            if (alliasAttribute == null)
                return;

            if (CommandAliasses.Intersect(alliasAttribute.Alliases, StringComparer.InvariantCultureIgnoreCase).Any())
                throw new Exception(descriptor.HandlerType.FullName);

            CommandAliasses.AddRange(alliasAttribute.Alliases);
        }

        /// <inheritdoc/>
        public bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
        {
            return _handlersDictionary.TryGetValue(updateType, out list);
        }

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            return _handlersDictionary.Any(pair => pair.Value.Count != 0);
        }

        /// <inheritdoc/>
        public abstract UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default);
    }
}
