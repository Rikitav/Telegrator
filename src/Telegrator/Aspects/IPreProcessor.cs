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

using Telegrator.Core.Handlers;

namespace Telegrator.Aspects;

/// <summary>
/// Interface for pre-execution processors that are executed before handler execution.
/// Implement this interface to add cross-cutting concerns like validation, logging, or authorization.
/// </summary>
public interface IPreProcessor
{
    /// <summary>
    /// Executes before the handler's main execution logic.
    /// </summary>
    /// <param name="container">The handler container containing the current update and context.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A <see cref="Result"/> indicating whether execution should continue or be stopped.</returns>
    public Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default);
}
