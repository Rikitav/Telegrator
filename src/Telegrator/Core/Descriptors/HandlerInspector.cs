using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Aspects;
using Telegrator.Core.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Core.Descriptors;

/// <summary>
/// Provides methods for inspecting handler types and retrieving their attributes and filters.
/// </summary>
public static class HandlerInspector
{
    /// <summary>
    /// Gets handler's display name
    /// </summary>
    /// <param name="handlerType"></param>
    /// <param name="precompiledAttributes">Precompiled attributes if available.</param>
    /// <returns></returns>
    public static string? GetDisplayName(MemberInfo handlerType, Attribute[]? precompiledAttributes = null)
    {
        if (handlerType == null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        if (precompiledAttributes != null)
        {
            return precompiledAttributes.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName;
        }

        return handlerType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
    }

    /// <summary>
    /// Gets the handler attribute from the specified member info.
    /// </summary>
    /// <param name="handlerType">The member info representing the handler type.</param>
    /// <param name="precompiledAttributes">Precompiled attributes if available</param>
    /// <returns>The handler attribute.</returns>
    public static UpdateHandlerAttributeBase GetHandlerAttribute(MemberInfo handlerType, Attribute[]? precompiledAttributes = null)
    {
        if (handlerType == null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        List<UpdateHandlerAttributeBase> handlerAttrs = precompiledAttributes != null
            ? precompiledAttributes.OfType<UpdateHandlerAttributeBase>().ToList()
            : handlerType.GetCustomAttributes<UpdateHandlerAttributeBase>().ToList();

        if (handlerAttrs.Count == 0)
        {
            throw new InvalidOperationException(
                $"Failed to register handler '{handlerType.Name}': Missing required attribute derived from '{nameof(UpdateHandlerAttributeBase)}'.");
        }

        if (handlerAttrs.Count > 1)
        {
            throw new InvalidOperationException(
                $"Failed to register handler '{handlerType.Name}': Multiple handler attributes found. A handler must have exactly one attribute derived from '{nameof(UpdateHandlerAttributeBase)}'.");
        }

        return handlerAttrs[0];
    }

    /// <summary>
    /// Gets the state keeper attribute from the specified member info, if present.
    /// </summary>
    /// <param name="handlerType">The member info representing the handler type.</param>
    /// <param name="precompiledAttributes">Precompiled attributes if available</param>
    /// <returns>The state keeper attribute, or null if not present.</returns>
    public static IFilter<Update>? GetStateKeeperAttribute(MemberInfo handlerType, Attribute[]? precompiledAttributes = null)
    {
        if (handlerType == null)
            throw new ArgumentNullException(nameof(handlerType));

        IEnumerable<Attribute> attrs = precompiledAttributes != null ? precompiledAttributes : handlerType.GetCustomAttributes();

        Attribute? stateAttr = attrs.FirstOrDefault(attr =>
        {
            Type type = attr.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(StateAttribute<,>);
        });

        return stateAttr as IFilter<Update>;
    }

    /// <summary>
    /// Gets all filter attributes for the specified handler type and update type.
    /// </summary>
    /// <param name="handlerType">The member info representing the handler type.</param>
    /// <param name="validUpdType">The valid update type.</param>
    /// <param name="precompiledAttributes">Precompiled attributes if available</param>
    /// <returns>An enumerable of filter attributes.</returns>
    public static IEnumerable<IFilter<Update>> GetFilterAttributes(MemberInfo handlerType, UpdateType validUpdType, Attribute[]? precompiledAttributes = null)
    {
        if (handlerType == null)
            throw new ArgumentNullException(nameof(handlerType));

        List<UpdateFilterAttributeBase> filters = precompiledAttributes != null
            ? precompiledAttributes.OfType<UpdateFilterAttributeBase>().ToList()
            : handlerType.GetCustomAttributes<UpdateFilterAttributeBase>().ToList();

        UpdateFilterAttributeBase? invalidFilter = filters.FirstOrDefault(f => !f.AllowedTypes.Contains(validUpdType));
        if (invalidFilter != null)
        {
            string allowedTypesStr = string.Join(", ", invalidFilter.AllowedTypes);
            throw new InvalidOperationException(
                $"Filter conflict on handler '{handlerType.Name}': The filter '{invalidFilter.GetType().Name}' " +
                $"does not support update type '{validUpdType}'. Allowed types: [{allowedTypesStr}].");
        }

        UpdateFilterAttributeBase? lastFilterAttribute = null;
        foreach (UpdateFilterAttributeBase filterAttribute in filters)
        {
            if (!filterAttribute.ProcessModifiers(lastFilterAttribute))
            {
                lastFilterAttribute = null;
                yield return filterAttribute.AnonymousFilter;
            }
            else
            {
                lastFilterAttribute = filterAttribute;
            }
        }
    }

    /// <summary>
    /// Gets the aspects configuration for the specified handler type.
    /// Inspects the handler for both self-processing (implements interfaces) and typed processing (uses attributes).
    /// </summary>
    /// <param name="handlerType">The type of the handler to inspect.</param>
    /// <param name="precompiledAttributes">Precompiled attributes if available</param>
    /// <returns>A <see cref="DescriptorAspectsSet"/> containing the aspects configuration.</returns>
    public static DescriptorAspectsSet GetAspects([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, Attribute[]? precompiledAttributes = null)
    {
        if (handlerType == null)
            throw new ArgumentNullException(nameof(handlerType));

        Type? typedPre = GetGenericArgumentFromOpenGenericAttribute(handlerType, typeof(BeforeExecutionAttribute<>), precompiledAttributes);
        Type? typedPost = GetGenericArgumentFromOpenGenericAttribute(handlerType, typeof(AfterExecutionAttribute<>), precompiledAttributes);

        return new DescriptorAspectsSet(typedPre, typedPost);
    }

    private static Type? GetGenericArgumentFromOpenGenericAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, Type openGenericAttributeType, Attribute[]? precompiledAttributes = null)
    {
        IEnumerable<Attribute> attrs = precompiledAttributes != null ? precompiledAttributes : handlerType.GetCustomAttributes();

        Attribute? attribute = attrs.FirstOrDefault(attr =>
        {
            Type type = attr.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == openGenericAttributeType;
        });

        return attribute?.GetType().GetGenericArguments().FirstOrDefault();
    }
}
