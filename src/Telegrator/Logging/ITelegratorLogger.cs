using System;

namespace Telegrator.Logging
{
    /// <summary>
    /// Interface for Telegrator logging system.
    /// Provides abstraction for logging without external dependencies.
    /// </summary>
    public interface ITelegratorLogger
    {
        /// <summary>
        /// Logs a message with specified level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">Optional exception.</param>
        void Log(LogLevel level, string message, Exception? exception = null);
    }

    /// <summary>
    /// Log levels for Telegrator logging system.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace level - most detailed logging.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Debug level - detailed debugging information.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Information level - general information.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Warning level - warning messages.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Error level - error messages.
        /// </summary>
        Error = 4
    }
} 