using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Attributes;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Providers;

/// <summary>
/// Descriptor for creating handlers from classes in a host environment
/// </summary>
public class HostClassHandlerDescriptor : ClassHandlerDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType descriptorType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, bool dontInspect = false, Attribute[]? precompiledAttributes = null)
        : base(descriptorType, handlerType, dontInspect, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a service key.
    /// </summary>
    public HostClassHandlerDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, object serviceKey, Attribute[]? precompiledAttributes = null)
        : base(handlerType, serviceKey, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, singletonInstance, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with an instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, instanceFactory, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a service key and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, instanceFactory, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class using a polling attribute.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute and singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, singletonInstance, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, instanceFactory, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute, service key, and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, instanceFactory, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with an update validation filter.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter and singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, singletonInstance, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, instanceFactory, precompiledAttributes)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter, service key, and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, instanceFactory, precompiledAttributes)
    {
    }

    /// <inheritdoc/>
    protected override void ValidateConstructor()
    {
        // Hosting allows dependency injection, so no parameterless constructor is required.
    }
}
