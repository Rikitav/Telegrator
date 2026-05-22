using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegrator.Core.Attributes;

namespace Telegrator.Core.Descriptors;

/// <summary>
/// Descriptor for creating handlers from classes
/// </summary>
public class ClassHandlerDescriptor : HandlerDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType descriptorType, Type handlerType, bool dontInspect = false)
        : base(descriptorType, handlerType, dontInspect)
    {
        if (!dontInspect)
        {
            ValidateConstructor();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a service key.
    /// </summary>
    public ClassHandlerDescriptor(Type handlerType, object serviceKey)
        : base(handlerType, serviceKey)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
        : base(type, handlerType, updateType, indexer, filters)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, singletonInstance)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with an instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, filters, instanceFactory)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a service key and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, instanceFactory)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class using a polling attribute.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute and singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, singletonInstance)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, instanceFactory)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a polling attribute, service key, and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, instanceFactory)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with an update validation filter.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter and singleton instance.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, singletonInstance)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, instanceFactory)
    {
        ValidateConstructor();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassHandlerDescriptor"/> class with a validation filter, service key, and instance factory.
    /// </summary>
    public ClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, instanceFactory)
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
