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
using Telegrator.Handlers;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks if a command matches any of the specified aliases.
/// Requires a <see cref="CommandHandlerAttribute"/> to be applied first to extract the command.
/// </summary>
/// <param name="alliases">The command aliases to check against.</param>
public class CommandAlliasFilter(params string[] alliases) : Filter<Message>
{
    /// <summary>
    /// Gets the command that was received and extracted by the <see cref="CommandHandlerAttribute"/>.
    /// </summary>
    public string ReceivedCommand { get; private set; } = string.Empty;

    /// <summary>
    /// Checks if the received command matches any of the specified aliases.
    /// This filter requires a <see cref="CommandHandlerAttribute"/> to be applied first
    /// to extract the command from the message.
    /// </summary>
    /// <param name="context">The filter execution context containing the completed filters.</param>
    /// <returns>True if the command matches any of the specified aliases; otherwise, false.</returns>
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        ReceivedCommand = context.CompletedFilters.Get<CommandHandlerAttribute>(0).ReceivedCommand;
        return alliases.Contains(ReceivedCommand, StringComparer.InvariantCultureIgnoreCase);
    }
}
