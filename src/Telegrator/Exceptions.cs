using Telegrator.Core.Descriptors;

namespace Telegrator
{
    /// <summary>
    /// Exception thrown when attempting to modify a frozen collection.
    /// </summary>
    public class CollectionFrozenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionFrozenException"/> class.
        /// </summary>
        public CollectionFrozenException()
            : base("Can't change a frozen collection.") { }
    }

    /// <summary>
    /// Exception thrown when a type is not a valid filter type.
    /// </summary>
    public class NotFilterTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotFilterTypeException"/> class.
        /// </summary>
        /// <param name="type">The type that is not a filter type.</param>
        public NotFilterTypeException(Type type)
            : base(string.Format("\"{0}\" is not a filter type", type.Name)) { }
    }

    /// <summary>
    /// Exception thrown when a handler execution fails.
    /// Contains information about the handler and the inner exception.
    /// </summary>
    public class HandlerFaultedException : Exception
    {
        /// <summary>
        /// The handler info associated with the faulted handler.
        /// </summary>
        public readonly DescribedHandlerDescriptor HandlerInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandlerFaultedException"/> class.
        /// </summary>
        /// <param name="handlerInfo">The handler info.</param>
        /// <param name="inner">The inner exception.</param>
        public HandlerFaultedException(DescribedHandlerDescriptor handlerInfo, Exception inner)
            : base(string.Format("Handler's \"{0}\" execution was faulted", handlerInfo.DisplayString), inner)
        {
            HandlerInfo = handlerInfo;
        }
    }
}
