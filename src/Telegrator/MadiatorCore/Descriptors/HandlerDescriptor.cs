using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes.Components;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Specifies the type of handler descriptor.
    /// </summary>
    public enum DescriptorType
    {
        /// <summary>
        /// General handler descriptor.
        /// </summary>
        General,

        /// <summary>
        /// Keyed handler descriptor (uses a service key).
        /// </summary>
        Keyed,

        /// <summary>
        /// Implicit handler descriptor.
        /// </summary>
        Implicit,

        /// <summary>
        /// Singleton handler descriptor (single instance).
        /// </summary>
        Singleton
    }

    /// <summary>
    /// Describes a handler, its type, filters, and instantiation logic.
    /// </summary>
    public class HandlerDescriptor
    {
        /// <summary>
        /// The type of the descriptor.
        /// </summary>
        public DescriptorType Type
        {
            get;
            private set;
        }

        /// <summary>
        /// The type of the handler.
        /// </summary>
        public Type HandlerType
        {
            get;
            private set;
        }

        /// <summary>
        /// The update type handled by this handler.
        /// </summary>
        public UpdateType UpdateType
        {
            get;
            protected set;
        }

        /// <summary>
        /// The indexer for handler concurrency and priority.
        /// </summary>
        public DescriptorIndexer Indexer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to form a fallback report.
        /// </summary>
        public bool FormReport
        {
            get;
            set;
        }

        /// <summary>
        /// The set of filters associated with this handler.
        /// </summary>
        public DescriptorFiltersSet? Filters
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the aspects configuration for this handler.
        /// Contains pre and post-execution processors if the handler uses the aspect system.
        /// </summary>
        public DescriptorAspectsSet? Aspects
        {
            get;
            protected set;
        }

        /// <summary>
        /// The service key for keyed handlers.
        /// </summary>
        public object? ServiceKey
        {
            get;
            protected set;
        }

        /// <summary>
        /// Factory for creating handler instances.
        /// </summary>
        public Func<UpdateHandlerBase>? InstanceFactory
        {
            get;
            protected set;
        }

        /// <summary>
        /// Singleton instance of the handler, if applicable.
        /// </summary>
        public UpdateHandlerBase? SingletonInstance
        {
            get;
            protected set;
        }

        /// <summary>
        /// Display string for the handler (for debugging or logging).
        /// </summary>
        public string? DisplayString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a function for 'lazy' handlers initialization
        /// </summary>
        public Action<UpdateHandlerBase>? LazyInitialization
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with the specified descriptor type and handler type.
        /// Automatically inspects the handler type to extract attributes, filters, and configuration.
        /// </summary>
        /// <param name="descriptorType">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler to describe</param>
        /// <param name="dontInspect"></param>
        /// <exception cref="ArgumentException">Thrown when the handler type is not compatible with the expected handler type</exception>
        public HandlerDescriptor(DescriptorType descriptorType, Type handlerType, bool dontInspect = false)
        {
            Type = descriptorType;
            HandlerType = handlerType;
            Filters = new DescriptorFiltersSet(null, null, null);

            if (dontInspect)
                return;

            UpdateHandlerAttributeBase handlerAttribute = HandlerInspector.GetHandlerAttribute(handlerType);
            if (handlerAttribute.ExpectingHandlerType != null && !handlerAttribute.ExpectingHandlerType.Contains(handlerType.BaseType))
                throw new ArgumentException(string.Format("This handler attribute cannot be attached to this class. Attribute can be attached on next handlers : {0}", string.Join(", ", handlerAttribute.ExpectingHandlerType.AsEnumerable())));

            StateKeeperAttributeBase? stateKeeperAttribute = HandlerInspector.GetStateKeeperAttribute(handlerType);
            IFilter<Update>[] filters = HandlerInspector.GetFilterAttributes(handlerType, handlerAttribute.Type).ToArray();

            UpdateType = handlerAttribute.Type;
            Indexer = handlerAttribute.GetIndexer();
            FormReport = handlerAttribute.FormReport;
            Filters = new DescriptorFiltersSet(handlerAttribute, stateKeeperAttribute, filters);
            Aspects = HandlerInspector.GetAspects(handlerType);
            DisplayString = HandlerInspector.GetDisplayName(handlerType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class as a keyed handler with the specified service key.
        /// </summary>
        /// <param name="handlerType">The type of the handler to describe</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> is null</exception>
        public HandlerDescriptor(Type handlerType, object serviceKey) : this(DescriptorType.Keyed, handlerType)
        {
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with all basic properties.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="filters">The set of filters associated with this handler</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with singleton instance support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="filters">The set of filters associated with this handler</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="singletonInstance">The singleton instance of the handler</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="singletonInstance"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with instance factory support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="filters">The set of filters associated with this handler</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with service key and instance factory support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="filters">The set of filters associated with this handler</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = filters;
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with polling handler attribute and filters.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute containing configuration</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with polling handler attribute, filters, and singleton instance.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute containing configuration</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="singletonInstance">The singleton instance of the handler</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="singletonInstance"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with polling handler attribute, filters, and instance factory.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute containing configuration</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with polling handler attribute, filters, service key, and instance factory.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="pollingHandlerAttribute">The polling handler attribute containing configuration</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = pollingHandlerAttribute.Type;
            Indexer = pollingHandlerAttribute.GetIndexer();
            Filters = new DescriptorFiltersSet(pollingHandlerAttribute, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with validation filter support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="validateFilter">Optional validation filter</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with validation filter and singleton instance support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="validateFilter">Optional validation filter</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="singletonInstance">The singleton instance of the handler</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="singletonInstance"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            SingletonInstance = singletonInstance ?? throw new ArgumentNullException(nameof(singletonInstance));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with validation filter and instance factory support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="validateFilter">Optional validation filter</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptor"/> class with validation filter, service key, and instance factory support.
        /// </summary>
        /// <param name="type">The type of the descriptor</param>
        /// <param name="handlerType">The type of the handler</param>
        /// <param name="updateType">The type of update this handler processes</param>
        /// <param name="indexer">The indexer for handler concurrency and priority</param>
        /// <param name="validateFilter">Optional validation filter</param>
        /// <param name="filters">Optional array of filters to apply</param>
        /// <param name="stateKeepFilter">Optional state keeping filter</param>
        /// <param name="serviceKey">The service key for dependency injection</param>
        /// <param name="instanceFactory">Factory for creating handler instances</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceKey"/> or <paramref name="instanceFactory"/> is null</exception>
        public HandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        {
            Type = type;
            HandlerType = handlerType;
            UpdateType = updateType;
            Indexer = indexer;
            Filters = new DescriptorFiltersSet(validateFilter, stateKeepFilter, filters);
            ServiceKey = serviceKey ?? throw new ArgumentNullException(nameof(serviceKey));
            InstanceFactory = instanceFactory ?? throw new ArgumentNullException(nameof(instanceFactory));
        }

        /// <summary>
        /// Sets singleton instance of this descriptor
        /// Throws exception if instance already set
        /// </summary>
        /// <param name="instance"></param>
        /// <exception cref="Exception"></exception>
        public void SetInstance(UpdateHandlerBase instance)
        {
            if (SingletonInstance != null)
                throw new Exception();

            SingletonInstance = instance;
        }

        /// <summary>
        /// Tries to set singleton instance of this descriptor
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public bool TrySetInstance(UpdateHandlerBase instance)
        {
            if (SingletonInstance != null)
                return false;

            SingletonInstance = instance;
            return true;
        }

        /// <inheritdoc/>
        public override string ToString()
            => DisplayString ?? HandlerType.Name;
    }
}