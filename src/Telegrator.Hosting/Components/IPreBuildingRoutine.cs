namespace Telegrator.Hosting.Components
{
    /// <summary>
    /// Interface for pre-building routines that can be executed during host construction.
    /// Allows for custom initialization and configuration steps before the bot host is built.
    /// </summary>
    public interface IPreBuildingRoutine
    {
        /// <summary>
        /// Executes the pre-building routine on the specified host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to configure.</param>
        public static abstract void PreBuildingRoutine(ITelegramBotHostBuilder hostBuilder);
    }
}
