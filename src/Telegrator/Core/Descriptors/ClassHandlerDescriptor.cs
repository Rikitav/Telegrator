using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Core.Descriptors;

/// <summary>
/// Descriptor for creating handlers from classes
/// </summary>
public class ClassHandlerDescriptor : HandlerDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType descriptorType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, bool dontInspect = false, Attribute[]? precompiledAttributes = null)
        : base(descriptorType, handlerType, dontInspect, precompiledAttributes)
    {
        if (!dontInspect)
        {
            ValidateConstructor();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a service key.
    /// </summary>
    public ClassHandlerDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, object serviceKey, Attribute[]? precompiledAttributes = null)
        : base(handlerType, serviceKey, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, singletonInstance, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with an instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a service key and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class using a polling attribute.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute and singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, singletonInstance, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute, service key, and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with an update validation filter.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter and singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, singletonInstance, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter, service key, and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, instanceFactory, precompiledAttributes)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Validates the handler's constructor matches the requirements.
    /// </summary>
    protected virtual void ValidateConstructor()
    {
        if (!HandlerType.HasParameterlessCtor())
            throw new Exception("This handler (" + HandlerType.FullName + "), must contain constructor without parameters.");
    }
}
