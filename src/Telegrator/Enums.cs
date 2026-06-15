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

namespace Telegrator;

/// <summary>
/// Enumeration of dice types supported by Telegram.
/// Used for filtering dice messages and determining dice emoji representations.
/// </summary>
public enum DiceType
{
    /// <summary>
    /// Standard dice (🎲).
    /// </summary>
    Dice,

    /// <summary>
    /// Darts (🎯).
    /// </summary>
    Darts,

    /// <summary>
    /// Bowling (🎳).
    /// </summary>
    Bowling,

    /// <summary>
    /// Basketball (🏀).
    /// </summary>
    Basketball,

    /// <summary>
    /// Football (⚽).
    /// </summary>
    Football,

    /// <summary>
    /// Casino slot machine (🎰).
    /// </summary>
    Casino
}

/// <summary>
/// Flags version of <see cref="Telegram.Bot.Types.Enums.ChatType"/>
/// Type of the <see cref="Telegram.Bot.Types.Chat"/>, from which the message or inline query was sent
/// </summary>
[Flags]
public enum ChatTypeFlags
{
    /// <summary>
    /// Normal one-to-one chat with a user or bot
    /// </summary>
    Private = 0x1,

    /// <summary>
    /// Normal group chat
    /// </summary>
    Group = 0x2,

    /// <summary>
    /// A channel
    /// </summary>
    Channel = 0x4,

    /// <summary>
    /// A supergroup
    /// </summary>
    Supergroup = 0x8,

    /// <summary>
    /// Value possible only in <see cref="Telegram.Bot.Types.InlineQuery.ChatType"/>: private chat with the inline query sender
    /// </summary>
    Sender
}
