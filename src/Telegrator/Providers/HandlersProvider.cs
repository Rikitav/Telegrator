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

using System.Collections.ObjectModel;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Handlers;
using Telegrator.Logging;

namespace Telegrator.Providers;

/// <summary>
/// Provides handler resolution and instantiation logic for Telegram bot updates.
/// Responsible for mapping update types to handler descriptors, filtering handlers based on update context,
/// and creating handler instances with appropriate lifecycle management.
/// </summary>
public class HandlersProvider : IHandlersProvider
{
    /// <inheritdoc/>
    public IEnumerable<UpdateType> AllowedTypes { get; }

    /// <summary>
    /// Read-only dictionary mapping <see cref="UpdateType"/> to lists of handler descriptors.
    /// Each descriptor list is frozen to prevent modification after initialization.
    /// </summary>
    protected readonly ReadOnlyDictionary<UpdateType, HandlerDescriptorList> HandlersDictionary;

    /// <summary>
    /// Configuration options for the bot and handler execution behavior.
    /// </summary>
    protected readonly TelegratorOptions Options;

    /// <summary>
    /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
    /// </summary>
    /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
    /// <param name="options">Configuration options for the bot and handler execution</param>
    /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
    public HandlersProvider(IHandlersCollection handlers, TelegratorOptions options)
    {
        AllowedTypes = handlers.AllowedTypes;
        HandlersDictionary = handlers.Values.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
        Options = options ?? throw new ArgumentNullException(nameof(options));
        TelegratorLogging.LogTrace("{0} created!", GetType().Name);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="HandlersProvider"/> with the specified handler collections and configuration.
    /// </summary>
    /// <param name="handlers">Collection of handler descriptor lists organized by update type</param>
    /// <param name="options">Configuration options for the bot and handler execution</param>
    /// <exception cref="ArgumentNullException">Thrown when options or botInfo is null</exception>
    public HandlersProvider(IEnumerable<HandlerDescriptorList> handlers, TelegratorOptions options)
    {
        AllowedTypes = Update.AllTypes;
        HandlersDictionary = handlers.ForEach(list => list.Freeze()).ToReadOnlyDictionary(list => list.HandlingType);
        Options = options ?? throw new ArgumentNullException(nameof(options));
        TelegratorLogging.LogTrace("{0} created!", GetType().Name);
    }

    /// <inheritdoc/>
    /// <exception cref="Exception">Thrown when the descriptor type is not recognized</exception>
    public virtual UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
    {
        try
        {
            // Checking handler instance status
            cancellationToken.ThrowIfCancellationRequested();
            bool useSingleton = UseSingleton(descriptor);

            // Returning singleton instance
            if (useSingleton && descriptor.SingletonInstance != null)
                return descriptor.SingletonInstance;

            // Creating instance
            UpdateHandlerBase instance = GetHandlerInstanceInternal(descriptor);
            if (useSingleton)
                descriptor.TrySetInstance(instance);

            // Lazy initialization execution
            descriptor.LazyInitialization?.Invoke(instance);
            return instance;
        }
        catch (Exception ex)
        {
            TelegratorLogging.LogError("Failed to create instance of '{0}'", exception: ex, descriptor.ToString());
            throw;
        }
    }

    private static UpdateHandlerBase GetHandlerInstanceInternal(HandlerDescriptor descriptor)
    {
        if (descriptor.InstanceFactory != null)
            return descriptor.InstanceFactory.Invoke();

        return (UpdateHandlerBase)Activator.CreateInstance(descriptor.HandlerType);
    }

    private static bool UseSingleton(HandlerDescriptor descriptor) => descriptor.Type switch
    {
        DescriptorType.General or DescriptorType.Keyed => false,
        DescriptorType.Implicit or DescriptorType.Singleton => true,
        _ => throw new Exception("Unknown decriptor type")
    };

    /// <inheritdoc/>
    public virtual bool TryGetDescriptorList(UpdateType updateType, out HandlerDescriptorList? list)
    {
        if (UpdateTypeExtensions.SuppressTypes.TryGetValue(updateType, out UpdateType suppressType))
            updateType = suppressType;

        return HandlersDictionary.TryGetValue(updateType, out list);
    }

    /// <inheritdoc/>
    public virtual bool IsEmpty()
    {
        return HandlersDictionary.Count == 0;
    }
}
