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
using Telegrator.Attributes;
using Telegrator.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Attribute for filtering messages based on command aliases.
/// Allows handlers to respond to multiple command variations using a single attribute.
/// </summary>
public class CommandAlliasAttribute : UpdateFilterAttribute<Message>
{
    /// <summary>
    /// Gets the allowed update types for this filter.
    /// </summary>
    public override UpdateType[] AllowedTypes => [UpdateType.Message];

    /// <summary>
    /// The description of the command (defaults to "no description provided").
    /// </summary>
    private string _description = "no description provided";

    /// <summary>
    /// Gets the array of command aliases that this filter will match.
    /// </summary>
    public string[] Alliases
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the description of the command.
    /// Must be between 0 and 256 characters in length.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the description length is outside the allowed range.</exception>
    public string Description
    {
        get => _description;
        set => _description = value is { Length: <= 256 and >= 0 }
            ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Initializes a new instance of the CommandAlliasAttribute with the specified command aliases.
    /// </summary>
    /// <param name="alliases">The command aliases to match against.</param>
    public CommandAlliasAttribute(params string[] alliases)
        : base(new CommandAlliasFilter(alliases.Select(c => c.TrimStart('/')).ToArray()))
        => Alliases = alliases.Select(c => c.TrimStart('/')).ToArray();

    /// <summary>
    /// Gets the filtering target (Message) from the update.
    /// </summary>
    /// <param name="update">The Telegram update.</param>
    /// <returns>The message from the update, or null if not present.</returns>
    public override Message? GetFilterringTarget(Update update) => update.Message;
}
