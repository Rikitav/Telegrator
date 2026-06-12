using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Core.Filters;

namespace Telegrator.Annotations;

/// <summary>
/// Filter annotation that matches messages starting with one of the configured prefixes
/// and optionally one of the configured trigger words.
/// </summary>
public class PrefixedTriggerWordAttribute : FilterAnnotation<Message>
{
    /// <summary>
    /// Gets or sets the prefixes to check (e.g. <c>"/"</c>, <c>"!"</c>).
    /// </summary>
    public string[] Prefixes { get; set; } = ["/"];

    /// <summary>
    /// Gets or sets the trigger words to match after the prefix.
    /// When empty, any text after the prefix is accepted.
    /// </summary>
    public string[]? Words { get; set; }

    /// <summary>
    /// Gets or sets whether the comparison is case-insensitive.
    /// </summary>
    public bool IgnoreCase { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the trigger word must be followed by whitespace or end of string.
    /// When <see langword="false"/>, <c>/startnow</c> would also match <c>/start</c>.
    /// </summary>
    public bool MatchWholeWord { get; set; } = true;

    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.Input.Text is not { Length: > 0 } text)
            return false;

        string? matchedPrefix = Prefixes.FirstOrDefault(p => text.StartsWith(p, IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        if (matchedPrefix is null)
            return false;

        if (Words is null || Words.Length == 0)
            return true;

        ReadOnlySpan<char> body = text.AsSpan(matchedPrefix.Length);
        if (body.IsEmpty)
            return false;

        StringComparison comparison = IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        foreach (string word in Words)
        {
            if (word.Length == 0)
                continue;

            if (!body.StartsWith(word.AsSpan(), comparison))
                continue;

            if (!MatchWholeWord)
                return true;

            ReadOnlySpan<char> tail = body.Slice(word.Length);
            if (tail.IsEmpty || char.IsWhiteSpace(tail[0]) || tail[0] == '@')
                return true;
        }

        return false;
    }
}
