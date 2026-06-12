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
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process PurchasedPaidMediaHandler updates.
/// </summary>
public class PurchasedPaidMediaHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PurchasedPaidMediaHandler>([typeof(BranchingPurchasedPaidMediaHandler)], UpdateType.PurchasedPaidMedia, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { PurchasedPaidMedia: { } };
}

/// <summary>
/// Abstract base class for handlers that process PurchasedPaidMediaHandler updates.
/// </summary>
public abstract class PurchasedPaidMediaHandler() : AbstractUpdateHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia)
{
}


/// <summary>
/// Abstract base class for branching handlers that process PurchasedPaidMediaHandler updates.
/// </summary>
public abstract class BranchingPurchasedPaidMediaHandler() : BranchingUpdateHandler<PaidMediaPurchased>(UpdateType.PurchasedPaidMedia)
{
}

