/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Telegrator.Core.Filters;

/// <summary>
/// Interface for filters that have a name for identification and debugging purposes.
/// </summary>
public interface INamedFilter
{
    /// <summary>
    /// Gets the name of the filter.
    /// </summary>
    public string Name { get; }
}

/// <summary>
/// Interface for filters that can be collected into a completed filters list.
/// Provides information about whether a filter should be tracked during execution.
/// </summary>
public interface IFilterCollectable
{
    /// <summary>
    /// Gets if filter can be collected to <see cref="CompletedFiltersList"/>
    /// </summary>
    public bool IsCollectible { get; }
}

/// <summary>
/// Represents a filter for a specific update type.
/// </summary>
/// <typeparam name="T">The type of the update to filter.</typeparam>
public interface IFilter<T> : IFilterCollectable where T : class
{
    /// <summary>
    /// Determines whether the filter can pass for the given context.
    /// </summary>
    /// <param name="info">The filter execution context.</param>
    /// <returns>True if the filter passes; otherwise, false.</returns>
    public bool CanPass(FilterExecutionContext<T> info);
}

/// <summary>
/// Represents a filter that joins multiple filters together.
/// </summary>
/// <typeparam name="T">The type of the input for the filter.</typeparam>
public interface IJoinedFilter<T> : IFilter<T> where T : class
{
    /// <summary>
    /// Gets the array of joined filters.
    /// </summary>
    public IFilter<T>[] Filters { get; }
}
