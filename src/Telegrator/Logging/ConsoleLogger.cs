using System;

namespace Telegrator.Logging
{
    /// <summary>
    /// Console logger implementation that writes to System.Console.
    /// This logger is optional and can be used for simple console output.
    /// </summary>
    public class ConsoleLogger : ITelegratorLogger
    {
        private readonly LogLevel _minimumLevel;
        private readonly bool _includeTimestamp;

        /// <summary>
        /// Initializes a new instance of ConsoleLogger.
        /// </summary>
        /// <param name="minimumLevel">Minimum log level to output. Default is Information.</param>
        /// <param name="includeTimestamp">Whether to include timestamp in log messages. Default is true.</param>
        public ConsoleLogger(LogLevel minimumLevel = LogLevel.Information, bool includeTimestamp = true)
        {
            _minimumLevel = minimumLevel;
            _includeTimestamp = includeTimestamp;
        }

        /// <inheritdoc/>
        public void Log(LogLevel level, string message, Exception? exception = null)
        {
            if (level < _minimumLevel)
                return;

            var timestamp = _includeTimestamp ? $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] " : "";
            var levelStr = $"[{level.ToString().ToUpper()}] ";
            var logMessage = $"{timestamp}{levelStr}{message}";

            // Add exception if present
            if (exception != null)
            {
                logMessage += $" | Exception: {exception.Message}";
            }

            // Write to console with appropriate color
            var originalColor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = level switch
                {
                    LogLevel.Trace => ConsoleColor.Gray,
                    LogLevel.Debug => ConsoleColor.Cyan,
                    LogLevel.Information => ConsoleColor.White,
                    LogLevel.Warning => ConsoleColor.Yellow,
                    LogLevel.Error => ConsoleColor.Red,
                    _ => ConsoleColor.White
                };

                Console.WriteLine(logMessage);

                // Write exception details if present
                if (exception != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Exception Details: {exception}");
                }
            }
            finally
            {
                Console.ForegroundColor = originalColor;
            }
        }
    }
} 