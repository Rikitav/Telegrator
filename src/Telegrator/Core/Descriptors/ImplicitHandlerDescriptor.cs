using System.Diagnostics.CodeAnalysis;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Core.Descriptors;

/// <summary>
/// Descriptor for creating implicit handlers (e.g. from handler builder)
/// </summary>
public class ImplicitHandlerDescriptor : HandlerDescriptor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImplicitHandlerDescriptor"/> class.
    /// </summary>
    public ImplicitHandlerDescriptor(DescriptorType type, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, UpdateType updateType, DescriptorIndexer indexer, IFilter<Update>? validateFilter, IFilter<Update>[]? filters, IFilter<Update>? stateKeepFilter, object serviceKey, UpdateHandlerBase singletonInstance, Attribute[]? precompiledAttributes = null)
        : base(type, handlerType, updateType, indexer, validateFilter, filters, stateKeepFilter, serviceKey, singletonInstance, precompiledAttributes)
    {
    }
}
