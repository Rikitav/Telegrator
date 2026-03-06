using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Attributes.Components
{
    /// <summary>
    /// Defines the <see cref="UpdateType"/>'s and validator (<see cref="IFilter{T}"/>) of the <see cref="Update"/> that <see cref="UpdateHandlerBase"/> will process
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class UpdateHandlerAttributeBase : Attribute, IFilter<Update>
    {
        /// <inheritdoc/>
        public bool IsCollectible => GetType().HasPublicProperties();

        /// <summary>
        /// Gets an array of <see cref="UpdateHandlerBase"/> that this attribute can be attached to
        /// </summary>
        public Type[] ExpectingHandlerType { get; private set; }
        
        /// <summary>
        /// Gets an <see cref="UpdateType"/> that handlers processes
        /// </summary>
        public UpdateType Type { get; private set; }
        
        /// <summary>
        /// Gets or sets importance of this <see cref="UpdateHandlerBase"/> in same <see cref="UpdateType"/> pool
        /// </summary>
        public int Importance { get; set; }

        /// <summary>
        /// Gets or sets priority of this <see cref="UpdateHandlerBase"/> in same type handlers pool
        /// </summary>
        public int Priority { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether to form a fallback report.
        /// </summary>
        public bool FormReport { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="UpdateHandlerAttributeBase"/>
        /// </summary>
        /// <param name="expectingHandlerType">The types of handlers that this attribute can be applied to.</param>
        /// <param name="updateType">The type of update that this handler processes.</param>
        /// <param name="importance">The importance level of this handler (default: 0).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expectingHandlerType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when one of the handler types is not a valid handler type.</exception>
        /// <exception cref="Exception">Thrown when <paramref name="updateType"/> is <see cref="UpdateType.Unknown"/>.</exception>
        protected internal UpdateHandlerAttributeBase(Type[] expectingHandlerType, UpdateType updateType, int importance = 0)
        {
            if (expectingHandlerType == null)
                throw new ArgumentNullException(nameof(expectingHandlerType));
            
            if (expectingHandlerType.Any(type => !type.IsHandlerAbstract()))
                throw new ArgumentException("One of expectingHandlerType is not a handler type", nameof(expectingHandlerType));

            if (updateType == UpdateType.Unknown)
                throw new Exception();

            ExpectingHandlerType = expectingHandlerType;
            Type = updateType;
            Importance = importance;
        }

        /// <summary>
        /// Gets an <see cref="DescriptorIndexer"/> of this <see cref="UpdateHandlerAttributeBase"/> from <see cref="Importance"/> and <see cref="Priority"/>
        /// </summary>
        /// <returns>A descriptor indexer for this handler attribute.</returns>
        public DescriptorIndexer GetIndexer()
            => new DescriptorIndexer(0, this);

        /// <summary>
        /// Validator (<see cref="IFilter{T}"/>) of the <see cref="Update"/> that <see cref="UpdateHandlerBase"/> will process
        /// </summary>
        /// <param name="context">The filter execution context containing the update to validate.</param>
        /// <returns>True if the update passes validation; otherwise, false.</returns>
        public abstract bool CanPass(FilterExecutionContext<Update> context);
    }
}
