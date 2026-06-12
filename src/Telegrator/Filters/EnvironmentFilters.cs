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

using System.Diagnostics;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Abstract base class for filters that operate based on the current environment.
/// Provides functionality to detect debug vs release environments.
/// </summary>
public abstract class EnvironmentFilter : Filter<Update>
{
    /// <summary>
    /// Gets a value indicating whether the current environment is debug mode.
    /// This is set during static initialization based on the DEBUG conditional compilation symbol.
    /// </summary>
    protected static bool IsCurrentEnvDebug { get; private set; } = false;

    /// <summary>
    /// Static constructor that initializes the environment detection.
    /// </summary>
    static EnvironmentFilter()
        => SetIsCurrentEnvDebug();

    /// <summary>
    /// Sets the debug environment flag. This method is only compiled in DEBUG builds.
    /// </summary>
    [Conditional("DEBUG")]
    private static void SetIsCurrentEnvDebug()
        => IsCurrentEnvDebug = true;
}

/// <summary>
/// Filter that only passes in debug environment builds.
/// </summary>
public class IsDebugEnvironmentFilter() : EnvironmentFilter
{
    /// <summary>
    /// Checks if the current environment is debug mode.
    /// </summary>
    /// <param name="_">The filter execution context (unused).</param>
    /// <returns>True if the current environment is debug mode; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<Update> _)
        => IsCurrentEnvDebug;
}

/// <summary>
/// Filter that only passes in release environment builds.
/// </summary>
public class IsReleaseEnvironmentFilter() : EnvironmentFilter
{
    /// <summary>
    /// Checks if the current environment is release mode.
    /// </summary>
    /// <param name="_">The filter execution context (unused).</param>
    /// <returns>True if the current environment is release mode; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<Update> _)
        => !IsCurrentEnvDebug;
}

/// <summary>
/// Filter that checks environment variable values.
/// </summary>
/// <param name="variable">The environment variable name to check.</param>
/// <param name="value">The expected value of the environment variable (optional).</param>
/// <param name="comparison">The string comparison type to use for value matching.</param>
public class EnvironmentVariableFilter(string variable, string? value, StringComparison comparison) : Filter<Update>
{
    /// <summary>
    /// The environment variable name to check.
    /// </summary>
    private readonly string _variable = variable;

    /// <summary>
    /// The expected value of the environment variable (optional).
    /// </summary>
    private readonly string? _value = value;

    /// <summary>
    /// The string comparison type to use for value matching.
    /// </summary>
    private readonly StringComparison _comparison = comparison;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentVariableFilter"/> class with a specific value.
    /// </summary>
    /// <param name="variable">The environment variable name to check.</param>
    /// <param name="value">The expected value of the environment variable.</param>
    public EnvironmentVariableFilter(string variable, string? value)
        : this(variable, value, StringComparison.InvariantCulture) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentVariableFilter"/> class that checks for non-null values.
    /// </summary>
    /// <param name="variable">The environment variable name to check.</param>
    public EnvironmentVariableFilter(string variable)
        : this(variable, "{NOT_NULL}", StringComparison.InvariantCulture) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentVariableFilter"/> class with custom comparison.
    /// </summary>
    /// <param name="variable">The environment variable name to check.</param>
    /// <param name="comparison">The string comparison type to use.</param>
    public EnvironmentVariableFilter(string variable, StringComparison comparison)
        : this(variable, "{NOT_NULL}", comparison) { }

    /// <summary>
    /// Checks if the environment variable matches the expected criteria.
    /// </summary>
    /// <param name="_">The filter execution context (unused).</param>
    /// <returns>True if the environment variable matches the criteria; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<Update> _)
    {
        string? envValue = Environment.GetEnvironmentVariable(_variable);

        if (envValue == null)
            return _value == null;

        if (_value == "{NOT_NULL}")
            return true;

        return envValue.Equals(_value, _comparison);
    }
}
