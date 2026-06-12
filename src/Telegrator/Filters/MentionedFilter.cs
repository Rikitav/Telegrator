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
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks if a message contains a mention of the bot or a specific user.
/// Requires a <see cref="MessageHasEntityFilter"/> to be applied first to identify mention entities.
/// </summary>
public class MentionedFilter : MessageFilterBase
{
    /// <summary>
    /// The username to check for in the mention (null means check for bot's username).
    /// </summary>
    private readonly string? Mention;

    /// <summary>
    /// Initializes a new instance of the <see cref="MentionedFilter"/> class that checks for bot mentions.
    /// </summary>
    public MentionedFilter()
    {
        Mention = null;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MentionedFilter"/> class that checks for specific user mentions.
    /// </summary>
    /// <param name="mention">The username to check for in the mention.</param>
    public MentionedFilter(string mention)
    {
        Mention = mention.TrimStart('@');
    }

    /// <summary>
    /// Checks if the message contains a mention of the specified user or bot.
    /// This filter requires a <see cref="MessageHasEntityFilter"/> to be applied first
    /// to identify mention entities in the message.
    /// </summary>
    /// <param name="context">The filter execution context containing the message and completed filters.</param>
    /// <returns>True if the message contains the specified mention; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the bot username is null and no specific mention is provided.</exception>
    protected override bool CanPassNext(FilterExecutionContext<Message> context)
    {
        if (Target.Text == null)
            return false;

        string userName = Mention ?? context.BotInfo.User.Username ?? throw new ArgumentNullException(nameof(context), "MentionedFilter requires BotInfo to be initialized");
        IEnumerable<MessageEntity> entities = context.CompletedFilters
            .Get<MessageHasEntityFilter>()
            .SelectMany(ent => ent.FoundEntities)
            .Where(ent => ent.Type == MessageEntityType.Mention);

        foreach (MessageEntity ent in entities)
        {
            string mention = Target.Text.Substring(ent.Offset + 1, ent.Length - 1);
            if (mention == userName)
                return true;
        }

        return false;
    }
}
