using System;
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
    public HostClassHandlerDescriptor(DescriptorType descriptorType, Type handlerType, bool dontInspect = false)
        : base(descriptorType, handlerType, dontInspect)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a service key.
    /// </summary>
    public HostClassHandlerDescriptor(Type handlerType, object serviceKey)
        : base(handlerType, serviceKey)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters)
        : base(type, handlerType, updateType, indexer, filters)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, singletonInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with an instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, filters, instanceFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a service key and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, DescriptorFiltersSet filters, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, filters, serviceKey, instanceFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class using a polling attribute.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute and singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, singletonInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, instanceFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a polling attribute, service key, and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateHandlerAttributeBase pollingHandlerAttribute, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, pollingHandlerAttribute, filters, stateKeepFilter, serviceKey, instanceFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with an update validation filter.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter and singleton instance.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, singletonInstance)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, instanceFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HostClassHandlerDescriptor"/> class with a validation filter, service key, and instance factory.
    /// </summary>
    public HostClassHandlerDescriptor(DescriptorType type, Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, Func<UpdateHandlerBase> instanceFactory)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, instanceFactory)
    {
    }

    /// <inheritdoc/>
    protected override void ValidateConstructor()
    {
        // Hosting allows dependency injection, so no parameterless constructor is required.
    }
}
