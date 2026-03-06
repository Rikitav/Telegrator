using System;

namespace Telegrator.Logging
{
    /// <summary>
    /// Null logger implementation that does nothing.
    /// Used when logging is not required or disabled.
    /// </summary>
    public class NullLogger : ITelegratorLogger
    {
        /// <summary>
        /// Singleton instance of NullLogger.
        /// </summary>
        public static readonly NullLogger Instance = new();

        private NullLogger() { }

        /// <inheritdoc/>
        public void Log(LogLevel level, string message, Exception? exception = null) { }
    }
} 