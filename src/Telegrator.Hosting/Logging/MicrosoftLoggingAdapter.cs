using Microsoft.Extensions.Logging;
using Telegrator.Logging;

namespace Telegrator.Hosting.Logging
{
    /// <summary>
    /// Adapter for Microsoft.Extensions.Logging to work with Telegrator logging system.
    /// This allows seamless integration with ASP.NET Core logging infrastructure.
    /// </summary>
    public class MicrosoftLoggingAdapter : ITelegratorLogger
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of MicrosoftLoggingAdapter.
        /// </summary>
        /// <param name="logger">The Microsoft.Extensions.Logging logger instance.</param>
        public MicrosoftLoggingAdapter(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public void Log(Telegrator.Logging.LogLevel level, string message, Exception? exception = null)
        {
            var msLogLevel = level switch
            {
                Telegrator.Logging.LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
                Telegrator.Logging.LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
                Telegrator.Logging.LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
                Telegrator.Logging.LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
                Telegrator.Logging.LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
                _ => Microsoft.Extensions.Logging.LogLevel.Information
            };

            if (exception != null)
            {
                _logger.Log(msLogLevel, default, message, exception, (str, exc) => string.Format("{0} : {1}", str, exc));
            }
            else
            {
                _logger.Log(msLogLevel, default, message, null, (str, _) => str);
            }
        }
    }
} 