﻿using Telegrator.Attributes.Components;

namespace Telegrator.MadiatorCore.Descriptors
{
    /// <summary>
    /// Represents an indexer for handler descriptors, containing concurrency and priority information.
    /// </summary>
    public readonly struct DescriptorIndexer(int routerIndex, int concurrency, int priority) : IComparable<DescriptorIndexer>
    {
        /// <summary>
        /// Index of this descriptor when it was added to router
        /// </summary>
        public readonly int RouterIndex = routerIndex;

        /// <summary>
        /// Of this handlert type
        /// </summary>
        public readonly int Importance = concurrency;

        /// <summary>
        /// The priority of the handler.
        /// </summary>
        public readonly int Priority = priority;

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptorIndexer"/> struct from a handler attribute.
        /// </summary>
        /// <param name="routerIndex"></param>
        /// <param name="pollingHandler">The handler attribute.</param>
        public DescriptorIndexer(int routerIndex, UpdateHandlerAttributeBase pollingHandler)
            : this(routerIndex, pollingHandler.Concurrency, pollingHandler.Priority) { }

        /// <summary>
        /// Returns a new <see cref="DescriptorIndexer"/> with updated priority.
        /// </summary>
        /// <param name="priority">The new priority value.</param>
        /// <returns>A new <see cref="DescriptorIndexer"/> instance.</returns>
        public DescriptorIndexer UpdatePriority(int priority)
            => new DescriptorIndexer(RouterIndex, Importance, priority);

        /// <summary>
        /// Returns a new <see cref="DescriptorIndexer"/> with updated concurrency.
        /// </summary>
        /// <param name="concurrency">The new concurrency value.</param>
        /// <returns>A new <see cref="DescriptorIndexer"/> instance.</returns>
        public DescriptorIndexer UpdateConcurrency(int concurrency)
            => new DescriptorIndexer(RouterIndex, concurrency, Priority);

        /// <summary>
        /// Returns a new <see cref="DescriptorIndexer"/> with updated RouterIndex.
        /// </summary>
        /// <param name="routerIndex"></param>
        /// <returns>A new <see cref="DescriptorIndexer"/> instance.</returns>
        public DescriptorIndexer UpdateIndex(int routerIndex)
            => new DescriptorIndexer(routerIndex, Importance, Priority);

        /// <summary>
        /// Compares this instance to another <see cref="DescriptorIndexer"/>.
        /// </summary>
        /// <param name="other">The other indexer to compare to.</param>
        /// <returns>An integer indicating the relative order.</returns>
        public int CompareTo(DescriptorIndexer other)
        {
            int importanceCmp = Importance.CompareTo(other.Importance);
            if (importanceCmp != 0)
                return importanceCmp;

            int priorityCmp = Priority.CompareTo(other.Priority);
            if (priorityCmp != 0)
                return priorityCmp;

            int routerIndexCmp = RouterIndex.CompareTo(other.RouterIndex);
            if (routerIndexCmp != 0)
                return routerIndexCmp;

            return 0;
        }

        /// <summary>
        /// Returns a string representation of the indexer.
        /// </summary>
        /// <returns>A string in the format (C:concurrency, P:priority).</returns>
        public override string ToString()
        {
            return string.Format("(I:{0}, C:{1}, P:{2})", RouterIndex, Importance, Priority);
        }
    }
}
