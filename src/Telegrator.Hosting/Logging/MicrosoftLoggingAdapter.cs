namespace Telegrator.Logging;

/// <summary>
/// Adapter for Microsoft.Extensions.Logging to work with Telegrator logging system.
/// This allows seamless integration with ASP.NET Core logging infrastructure.
/// </summary>
public class MicrosoftLoggingAdapter : ITelegratorLogger
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    /// <summary>
    /// Initializes a new instance of MicrosoftLoggingAdapter.
    /// </summary>
    /// <param name="logger">The Microsoft.Extensions.Logging logger instance.</param>
    public MicrosoftLoggingAdapter(Microsoft.Extensions.Logging.ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        var msLogLevel = level switch
        {
            LogLevel.Trace => Microsoft.Extensions.Logging.LogLevel.Trace,
            LogLevel.Debug => Microsoft.Extensions.Logging.LogLevel.Debug,
            LogLevel.Information => Microsoft.Extensions.Logging.LogLevel.Information,
            LogLevel.Warning => Microsoft.Extensions.Logging.LogLevel.Warning,
            LogLevel.Error => Microsoft.Extensions.Logging.LogLevel.Error,
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
