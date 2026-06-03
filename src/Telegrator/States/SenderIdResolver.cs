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

using Telegram.Bot.Types;
using Telegrator.Core.States;

namespace Telegrator.States;

/// <summary>
/// Resolves sender ID from Telegram updates for state management purposes.
/// Extracts the sender identifier from various types of updates to provide a consistent key for state operations.
/// </summary>
public class SenderIdResolver : IStateKeyResolver
{
    /// <summary>
    /// Resolves the sender ID from a Telegram update.
    /// </summary>
    /// <param name="keySource">The Telegram update to extract the sender ID from.</param>
    /// <returns>The sender ID as a long value.</returns>
    /// <exception cref="ArgumentException">Thrown when the update does not contain a valid sender ID.</exception>
    public string ResolveKey(Update keySource)
        => keySource.GetSenderId()?.ToString() ?? throw new ArgumentException("Cannot resolve SenderID for this Update");
}
