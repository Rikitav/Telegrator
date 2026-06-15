/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Core;
using Telegrator.Core.Descriptors;

namespace Telegrator.Providers;

/// <summary>
/// Collection class for managing handler descriptors organized by update type.
/// Provides functionality for collecting, adding, and organizing handlers.
/// </summary>
/// <param name="options">Optional configuration options for handler collecting.</param>
public class HandlersCollection(TelegratorOptions? options) : IHandlersCollection
{
    /// <summary>
    /// Gets the collection of <see cref="UpdateType"/>'s allowed by registered handlers
    /// </summary>
    protected readonly List<UpdateType> _allowedTypes = [];

    /// <summary>
    /// Dictionary that organizes handler descriptors by update type.
    /// </summary>
    protected readonly Dictionary<UpdateType, HandlerDescriptorList> InnerDictionary = [];

    /// <summary>
    /// Configuration options for handler collecting.
    /// </summary>
    protected readonly TelegratorOptions? Options = options;

    /// <inheritdoc/>
    public virtual HandlerDescriptor CreateClassDescriptor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType, Attribute[]? precompiledAttributes = null)
    {
        return new ClassHandlerDescriptor(DescriptorType.General, handlerType, dontInspect: false, precompiledAttributes);
    }

    /// <summary>
    /// List of command aliases that have been registered.
    /// </summary>
    public readonly List<string> CommandAliasses = [];

    /// <inheritdoc/>
    public IEnumerable<UpdateType> AllowedTypes => _allowedTypes;

    /// <inheritdoc/>
    public IEnumerable<UpdateType> Keys
    {
        get => InnerDictionary.Keys;
    }

    /// <inheritdoc/>
    public IEnumerable<HandlerDescriptorList> Values
    {
        get => InnerDictionary.Values;
    }

    /// <inheritdoc/>
    public HandlerDescriptorList this[UpdateType updateType]
    {
        get => InnerDictionary[updateType];
    }

    /// <summary>
    /// Adds a handler descriptor to the collection.
    /// </summary>
    /// <param name="descriptor">The handler descriptor to add.</param>
    /// <returns>This collection instance for method chaining.</returns>
    /// <exception cref="Exception">Thrown when the handler type doesn't have a parameterless constructor and MustHaveParameterlessCtor is true.</exception>
    public virtual IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
    {
        _allowedTypes.UnionAdd(descriptor.UpdateType);
        MightAwaitAttribute? mightAwait = descriptor.HandlerType.GetCustomAttribute<MightAwaitAttribute>();
        if (mightAwait != null)
            _allowedTypes.UnionAdd(mightAwait.UpdateTypes);

        IntersectCommands(descriptor);
        HandlerDescriptorList list = GetDescriptorList(descriptor);

        if (descriptor.UpdateType == UpdateType.InlineQuery || descriptor.UpdateType == UpdateType.ChosenInlineResult)
        {
            if (list.Count > 0)
                throw new Exception("Bot cannot have more than one InlineQuery handler");
        }

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

        if (!InnerDictionary.TryGetValue(updateType, out HandlerDescriptorList? list))
        {
            list = new HandlerDescriptorList(updateType, Options);
            InnerDictionary.Add(updateType, list);
        }

        return list;
    }

    /// <summary>
    /// Checks for intersecting command aliases and handles them according to configuration.
    /// </summary>
    /// <param name="descriptor">The handler descriptor to check for command aliases.</param>
    /// <exception cref="Exception">Thrown when intersecting command aliases are found and ExceptIntersectingCommandAliases is enabled.</exception>
    protected void IntersectCommands(HandlerDescriptor descriptor)
    {
        if (Options == null)
            return;

        CommandAliasAttribute? alliasAttribute = descriptor.HandlerType.GetCustomAttribute<CommandAliasAttribute>();
        if (alliasAttribute == null)
            return;

        if (Options.ExceptIntersectingCommandAliases && CommandAliasses.Intersect(alliasAttribute.Alliases, StringComparer.InvariantCultureIgnoreCase).Any())
            throw new Exception(descriptor.HandlerType.FullName);

        CommandAliasses.AddRange(alliasAttribute.Alliases);
    }

    /// <inheritdoc/>
    public bool TryGetDescriptorList(UpdateType updateType, [NotNullWhen(true)] out HandlerDescriptorList? list)
    {
        return InnerDictionary.TryGetValue(updateType, out list);
    }
}
