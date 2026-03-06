using Telegrator.MadiatorCore.Descriptors;

namespace Telegrator.Providers
{
    /// <summary>
    /// Interface for classes that can provide custom handler descriptors.
    /// Allows classes to define their own handler description logic beyond the standard reflection-based approach.
    /// </summary>
    public interface ICustomDescriptorsProvider
    {
        /// <summary>
        /// Describes the handlers provided by this class.
        /// </summary>
        /// <returns>A collection of handler descriptors.</returns>
        public IEnumerable<HandlerDescriptor> DescribeHandlers();
    }
}
