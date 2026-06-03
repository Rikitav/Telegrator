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

using Telegrator.Core.Descriptors;

namespace Telegrator;

/// <summary>
/// Exception thrown when attempting to modify a frozen collection.
/// </summary>
public class CollectionFrozenException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionFrozenException"/> class.
    /// </summary>
    public CollectionFrozenException()
        : base("Can't change a frozen collection.") { }
}

/// <summary>
/// Exception thrown when a type is not a valid filter type.
/// </summary>
public class NotFilterTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFilterTypeException"/> class.
    /// </summary>
    /// <param name="type">The type that is not a filter type.</param>
    public NotFilterTypeException(Type type)
        : base(string.Format("\"{0}\" is not a filter type", type.Name)) { }
}

/// <summary>
/// Exception thrown when a handler execution fails.
/// Contains information about the handler and the inner exception.
/// </summary>
public class HandlerFaultedException : Exception
{
    /// <summary>
    /// The handler info associated with the faulted handler.
    /// </summary>
    public readonly DescribedHandlerDescriptor HandlerInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerFaultedException"/> class.
    /// </summary>
    /// <param name="handlerInfo">The handler info.</param>
    /// <param name="inner">The inner exception.</param>
    public HandlerFaultedException(DescribedHandlerDescriptor handlerInfo, Exception inner)
        : base(string.Format("Handler's \"{0}\" execution was faulted", handlerInfo.DisplayString), inner)
    {
        HandlerInfo = handlerInfo;
    }
}
