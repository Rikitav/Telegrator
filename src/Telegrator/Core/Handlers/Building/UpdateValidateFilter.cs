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

using Telegram.Bot.Types;
using Telegrator.Core.Filters;

namespace Telegrator.Core.Handlers.Building;

/// <summary>
/// Delegate for validating an update in a filter context.
/// </summary>
/// <param name="context">The filter execution context.</param>
/// <returns>True if the update is valid; otherwise, false.</returns>
public delegate bool UpdateValidateAction(FilterExecutionContext<Update> context);

/// <summary>
/// Filter that uses a delegate to validate updates.
/// </summary>
public class UpdateValidateFilter : IFilter<Update>
{
    /// <summary>
    /// Gets a value indicating whether this filter is collectable. Always false for this filter.
    /// </summary>
    public bool IsCollectible => false;
    private readonly UpdateValidateAction UpdateValidateAction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateValidateFilter"/> class.
    /// </summary>
    /// <param name="updateValidateAction">The validation delegate to use.</param>
    public UpdateValidateFilter(UpdateValidateAction updateValidateAction)
    {
        UpdateValidateAction = updateValidateAction;
    }

    /// <summary>
    /// Determines whether the filter can pass for the given context using the validation delegate.
    /// </summary>
    /// <param name="info">The filter execution context.</param>
    /// <returns>True if the filter passes; otherwise, false.</returns>
    public bool CanPass(FilterExecutionContext<Update> info)
        => UpdateValidateAction.Invoke(info);
}
