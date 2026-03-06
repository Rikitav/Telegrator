using Telegrator.MadiatorCore;

namespace Telegrator.Hosting.Providers.Components
{
    /// <summary>
    /// Collection class for managing handler descriptors organized by update type for host apps.
    /// Provides functionality for collecting, adding, scanning, and organizing handlers.
    /// </summary>
    public interface IHostHandlersCollection : IHandlersCollection
    {
        /// <summary>
        /// List of tasks that should be completed right before building the bot
        /// </summary>
        public List<PreBuildingRoutine> PreBuilderRoutines { get; }
    }
}
