using Telegrator.Core.Descriptors;

namespace Telegrator.Core.Handlers
{
    /// <summary>
    /// Factory interface for creating handler containers.
    /// Provides a way to create handler containers with specific providers and handler information.
    /// </summary>
    public interface IHandlerContainerFactory
    {
        /// <summary>
        /// Creates a new <see cref="IHandlerContainer"/> for the specified awaiting provider and handler info.
        /// </summary>
        /// <param name="handlerInfo">The <see cref="DescribedHandlerDescriptor"/> for the handler.</param>
        /// <returns>A new <see cref="IHandlerContainer"/> instance.</returns>
        public IHandlerContainer CreateContainer(DescribedHandlerDescriptor handlerInfo);
    }
}
