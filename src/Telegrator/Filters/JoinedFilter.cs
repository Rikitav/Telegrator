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

using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Base class for filters that join multiple filters together.
/// </summary>
/// <typeparam name="T">The type of the input for the filter.</typeparam>
public abstract class JoinedFilter<T>(params IFilter<T>[] filters) : Filter<T>, IJoinedFilter<T> where T : class
{
    /// <summary>
    /// Gets the array of joined filters.
    /// </summary>
    public IFilter<T>[] Filters { get; } = filters;
}

/// <summary>
/// A filter that passes only if both joined filters pass.
/// </summary>
/// <typeparam name="T">The type of the input for the filter.</typeparam>
public class AndFilter<T> : JoinedFilter<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AndFilter{T}"/> class.
    /// </summary>
    /// <param name="leftFilter">The left filter.</param>
    /// <param name="rightFilter">The right filter.</param>
    public AndFilter(IFilter<T> leftFilter, IFilter<T> rightFilter)
        : base(leftFilter, rightFilter) { }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<T> context)
        => Filters[0].CanPass(context) && Filters[1].CanPass(context);
}

/// <summary>
/// A filter that passes if at least one of the joined filters passes.
/// </summary>
/// <typeparam name="T">The type of the input for the filter.</typeparam>
public class OrFilter<T> : JoinedFilter<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrFilter{T}"/> class.
    /// </summary>
    /// <param name="leftFilter">The left filter.</param>
    /// <param name="rightFilter">The right filter.</param>
    public OrFilter(IFilter<T> leftFilter, IFilter<T> rightFilter)
        : base(leftFilter, rightFilter) { }

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<T> context)
        => Filters[0].CanPass(context) || Filters[1].CanPass(context);
}
