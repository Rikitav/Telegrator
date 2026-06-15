/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Extensions.Logging;

namespace Telegrator.Logging;

/// <summary>
/// Centralized logging system for Telegrator.
/// Provides static access to logging functionality with adapter support.
/// </summary>
public class TelegratorLogging : ILogger
{
    private static readonly TelegratorLogging _instance = new TelegratorLogging();
    private static readonly List<ILogger> _adapters = new();
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
    public static void AddAdapter(ILogger adapter)
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
    public static void RemoveAdapter(ILogger adapter)
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

    /// <inheritdoc/>
    public static bool Enabled(LogLevel logLevel)
    {
        return _instance.IsEnabled(logLevel);
    }

    /// <inheritdoc/>
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= MinimalLevel;
    }

    /// <inheritdoc/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    /// <inheritdoc/>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        // Fast path: if no adapters, do nothing
        if (_adapters.Count == 0)
            return;

        if (!IsEnabled(MinimalLevel))
            return;

        // Snapshot copy to prevent collection modification during iteration
        ILogger[] adaptersSnapshot;
        lock (_lock)
        {
            adaptersSnapshot = _adapters.ToArray();
        }

        Parallel.ForEach(adaptersSnapshot, adapter =>
        {
            try
            {
                adapter.Log(logLevel, eventId, state, exception, formatter);
            }
            catch
            {
                // Ignore adapter errors to prevent logging failures
                _ = 0xBAD + 0xC0DE;
            }
        });
    }

    /// <summary>
    /// Logs a trace message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogTrace(string message)
    {
        _instance.Log(LogLevel.Trace, message);
    }

    /// <summary>
    /// Logs a trace message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args"></param>
    public static void LogTrace(string message, params object[] args)
    {
        _instance.Log(LogLevel.Trace, message, args: args);
    }

    /// <summary>
    /// Logs a debug message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogDebug(string message)
    {
        _instance.Log(LogLevel.Debug, message);
    }

    /// <summary>
    /// Logs a debug message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args"></param>
    public static void LogDebug(string message, params object[] args)
    {
        _instance.Log(LogLevel.Debug, message, args: args);
    }

    /// <summary>
    /// Logs an information message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogInformation(string message)
    {
        _instance.Log(LogLevel.Information, message);
    }

    /// <summary>
    /// Logs an information message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args"></param>
    public static void LogInformation(string message, params object[] args)
    {
        _instance.Log(LogLevel.Information, message, args: args);
    }

    /// <summary>
    /// Logs a warning message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    public static void LogWarning(string message)
    {
        _instance.Log(LogLevel.Warning, message);
    }

    /// <summary>
    /// Logs a warning message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args"></param>
    public static void LogWarning(string message, params object[] args)
    {
        _instance.Log(LogLevel.Warning, message, args: args);
    }

    /// <summary>
    /// Logs an error message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">Optional exception.</param>
    public static void LogError(string message, Exception? exception = null)
    {
        _instance.Log(LogLevel.Error, message, exception);
    }

    /// <summary>
    /// Logs an error message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="args"></param>
    public static void LogError(string message, params object[] args)
    {
        _instance.Log(LogLevel.Error, message, args: args);
    }

    /// <summary>
    /// Logs an error message with exception only to all registered adapters.
    /// </summary>
    /// <param name="exception">The exception to log.</param>
    public static void LogError(Exception exception)
    {
        _instance.Log(LogLevel.Error, exception.Message, exception);
    }

    /// <summary>
    /// Logs an error message to all registered adapters.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">Optional exception.</param>
    /// <param name="args"></param>
    public static void LogError(string message, Exception? exception = null, params object[] args)
    {
        _instance.Log(LogLevel.Error, message, exception, args);
    }
}
