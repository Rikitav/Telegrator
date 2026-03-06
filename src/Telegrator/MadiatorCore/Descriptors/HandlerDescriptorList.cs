using System.Collections;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// The collection containing the <see cref="HandlerDescriptor"/>'s. Used to route <see cref="Update"/>'s in <see cref="IHandlersProvider"/>
    /// </summary>
    public sealed class HandlerDescriptorList : IEnumerable<HandlerDescriptor>
    {
        private readonly object _lock = new object();
        private readonly SortedList<DescriptorIndexer, HandlerDescriptor> _innerCollection;
        private readonly ITelegratorOptions? _options;
        private readonly UpdateType _handlingType;

        private int count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly { get; private set; } = false;

        /// <summary>
        /// Gets the <see cref="UpdateType"/> of handlers in this collection.
        /// </summary>
        public UpdateType HandlingType => _handlingType;

        /// <summary>
        /// Gets count of registered handlers in list
        /// </summary>
        public int Count => _innerCollection.Count;

        /// <summary>
        /// Gets or sets the <see cref="HandlerDescriptor"/> at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public HandlerDescriptor this[int index]
        {
            get => _innerCollection.Values[index];
            set => _innerCollection.Values[index] = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptorList"/> class without a specific <see cref="UpdateType"/>.
        /// </summary>
        public HandlerDescriptorList()
            : this(UpdateType.Unknown, default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerDescriptorList"/> class.
        /// </summary>
        /// <param name="updateType">The update type for the handlers.</param>
        /// <param name="options">The collecting options.</param>
        public HandlerDescriptorList(UpdateType updateType, ITelegratorOptions? options)
        {
            _innerCollection = [];
            _handlingType = updateType;
            _options = options;
        }

        /// <summary>
        /// Adds a new <see cref="HandlerDescriptor"/> to the collection.
        /// </summary>
        /// <param name="descriptor">The handler descriptor to add.</param>
        /// <exception cref="CollectionFrozenException">Thrown if the collection is frozen.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the update type does not match.</exception>
        public void Add(HandlerDescriptor descriptor)
        {
            lock (_lock)
            {
                if (IsReadOnly)
                    throw new CollectionFrozenException();

                if (_handlingType != UpdateType.Unknown && descriptor.UpdateType != _handlingType)
                    throw new InvalidOperationException();

                descriptor.Indexer = descriptor.Indexer.UpdateIndex(count++);
                _innerCollection.Add(descriptor.Indexer, descriptor);

                if (_handlingType == UpdateType.InlineQuery)
                    IsReadOnly = true;
            }
        }

        /// <summary>
        /// Checks if the collection contains a <see cref="HandlerDescriptor"/> with the specified <see cref="DescriptorIndexer"/>.
        /// </summary>
        /// <param name="indexer">The descriptor indexer.</param>
        /// <returns>True if the descriptor exists; otherwise, false.</returns>
        public bool ContainsKey(DescriptorIndexer indexer)
        {
            return _innerCollection.ContainsKey(indexer);
        }

        /// <summary>
        /// Removes the <see cref="HandlerDescriptor"/> with the specified <see cref="DescriptorIndexer"/> from the collection.
        /// </summary>
        /// <param name="indexer">The descriptor indexer.</param>
        /// <returns>True if the descriptor was removed; otherwise, false.</returns>
        public bool Remove(DescriptorIndexer indexer)
        {
            lock (_lock)
            {
                return _innerCollection.Remove(indexer);
            }
        }

        /// <summary>
        /// Removes the <see cref="HandlerDescriptor"/> from the collection.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        public bool Remove(HandlerDescriptor descriptor)
        {
            lock (_lock)
            {
                int index = _innerCollection.IndexOfValue(descriptor);
                if (index == -1)
                    return false;

                _innerCollection.RemoveAt(index);
                return true;
            }
        }

        /// <summary>
        /// Removes all descriptos from the <see cref="HandlerDescriptorList"/>
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _innerCollection.Clear();
            }
        }

        /// <summary>
        /// Freezes the <see cref="HandlerDescriptorList"/> and prohibits adding new elements to it.
        /// </summary>
        public void Freeze()
        {
            IsReadOnly = true;
        }

        /// <inheritdoc/>
        public IEnumerator<HandlerDescriptor> GetEnumerator()
        {
            return _innerCollection.Values.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerCollection.Values.GetEnumerator();
        }
    }
}
