namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Interface for providers that collect and manage handler collections.
    /// Provides access to a collection of handlers for various processing operations.
    /// </summary>
    public interface ICollectingProvider
    {
        /// <summary>
        /// Gets the collection of handlers managed by this provider.
        /// </summary>
        public IHandlersCollection Handlers { get; }
    }
}
