using System.Security.Cryptography.X509Certificates;

namespace Telegrator.Logging
{
    /// <summary>
    /// Centralized logging system for Telegrator.
    /// Provides static access to logging functionality with adapter support.
    /// </summary>
    public static class Alligator
    {
        private static readonly List<ITelegratorLogger> _adapters = new();
        private static readonly object _lock = new();

        /// <summary>
        /// Gets the current adapters count.
        /// </summary>
        public static int AdaptersCount => _adapters.Count;

        /// <summary>
        /// Minimal level of logging messages.
        /// Any messages below thi value will not be writen!
        /// </summary>
        public static LogLevel MinimalLevel { get; set; } = LogLevel.Information;

        /// <summary>
        /// Adds a logger adapter to the centralized logging system.
        /// </summary>
        /// <param name="adapter">The logger adapter to add.</param>
        public static void AddAdapter(ITelegratorLogger adapter)
        {
            if (adapter == null)
                throw new ArgumentNullException(nameof(adapter));

            lock (_lock)
            {
                if (!_adapters.Contains(adapter))
                {
                    _adapters.Add(adapter);
                }
            }
        }

        /// <summary>
        /// Removes a logger adapter from the centralized logging system.
        /// </summary>
        /// <param name="adapter">The logger adapter to remove.</param>
        public static void RemoveAdapter(ITelegratorLogger adapter)
        {
            if (adapter == null)
                return;

            lock (_lock)
            {
                _adapters.Remove(adapter);
            }
        }

        /// <summary>
        /// Clears all logger adapters.
        /// </summary>
        public static void ClearAdapters()
        {
            lock (_lock)
            {
                _adapters.Clear();
            }
        }

        /// <summary>
        /// Logs a message to all registered adapters.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">Optional exception.</param>
        /// <param name="args"></param>
        public static void Log(LogLevel level, string message, Exception? exception = null, params object[] args)
        {
            // Fast path: if no adapters, do nothing
            if (_adapters.Count == 0)
                return;

            if (level != LogLevel.Trace & level < MinimalLevel)
                return;

            // Lock only during enumeration to prevent collection modification during iteration
            foreach (var adapter in _adapters)
            {
                try
                {
                    if (args != null)
                        message = string.Format(message, args);

                    adapter.Log(level, message, exception);
                }
                catch
                {
                    _ = 0xBAD + 0xC0DE; // Ignore adapter errors to prevent logging failures
                }
            }
        }

        /// <summary>
        /// Logs a trace message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogTrace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        /// <summary>
        /// Logs a trace message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args"></param>
        public static void LogTrace(string message, params object[] args)
        {
            Log(LogLevel.Trace, message, args: args);
        }

        /// <summary>
        /// Logs a debug message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogDebug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs a debug message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args"></param>
        public static void LogDebug(string message, params object[] args)
        {
            Log(LogLevel.Debug, message, args: args);
        }

        /// <summary>
        /// Logs an information message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogInformation(string message)
        {
            Log(LogLevel.Information, message);
        }

        /// <summary>
        /// Logs an information message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args"></param>
        public static void LogInformation(string message, params object[] args)
        {
            Log(LogLevel.Information, message, args: args);
        }

        /// <summary>
        /// Logs a warning message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void LogWarning(string message)
        {
            Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs a warning message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args"></param>
        public static void LogWarning(string message, params object[] args)
        {
            Log(LogLevel.Warning, message, args: args);
        }

        /// <summary>
        /// Logs an error message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">Optional exception.</param>
        public static void LogError(string message, Exception? exception = null)
        {
            Log(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Logs an error message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="args"></param>
        public static void LogError(string message, params object[] args)
        {
            Log(LogLevel.Error, message, args: args);
        }

        /// <summary>
        /// Logs an error message with exception only to all registered adapters.
        /// </summary>
        /// <param name="exception">The exception to log.</param>
        public static void LogError(Exception exception)
        {
            Log(LogLevel.Error, exception.Message, exception);
        }

        /// <summary>
        /// Logs an error message to all registered adapters.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="exception">Optional exception.</param>
        /// <param name="args"></param>
        public static void LogError(string message, Exception? exception = null, params object[] args)
        {
            Log(LogLevel.Error, message, exception, args);
        }
    }
} 