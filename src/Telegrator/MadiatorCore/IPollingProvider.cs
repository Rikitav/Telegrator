namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Interface for polling providers that manage both regular and awaiting handlers.
    /// Provides access to handlers for different types of update processing during polling operations.
    /// </summary>
    public interface IPollingProvider
    {
        /// <summary>
        /// Gets the <see cref="IHandlersProvider"/> that manages handlers for polling.
        /// </summary>
        public IHandlersProvider HandlersProvider { get; }

        /// <summary>
        /// Gets the <see cref="IAwaitingProvider"/> that manages awaiting handlers for polling.
        /// </summary>
        public IAwaitingProvider AwaitingProvider { get; }
    }
}
