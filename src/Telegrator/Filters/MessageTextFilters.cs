using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using static System.Net.Mime.MediaTypeNames;

namespace Telegrator.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on message text content.
    /// Provides common functionality for extracting and validating message text.
    /// </summary>
    public abstract class MessageTextFilter : MessageFilterBase
    {
        /// <summary>
        /// Gets the current message being processed by the filter.
        /// </summary>
        public Message Message { get; private set; } = null!;

        /// <summary>
        /// Gets the extracted text content from the current message.
        /// </summary>
        public string Text { get; private set; } = null!;

        /// <summary>
        /// Determines if the message can pass through the filter by validating the message
        /// and extracting its text content for further processing.
        /// </summary>
        /// <param name="context">The filter execution context containing the message update.</param>
        /// <returns>True if the message is valid and can be processed further; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            if (!base.CanPassBase(context))
                return false;

            if (Target is not { Id: > 0, Text.Length: > 0 })
                return false;

            Text = Target.Text;
            return CanPassNext(context);
        }
    }

    /// <summary>
    /// Filter that checks if the message text starts with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text starts with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextStartsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text starts with.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text starts with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text starts with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.StartsWith(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text ends with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text ends with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextEndsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text ends with.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text ends with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text ends with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.EndsWith(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text contains a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text contains.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextContainsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text contains.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text contains the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text contains the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.IndexOf(Content, Comparison) >= 0;
    }

    /// <summary>
    /// Filter that checks if the message text equals a specified content.
    /// </summary>
    /// <param name="content">The content to check if the message text equals.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    public class TextEqualsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text equals.
        /// </summary>
        protected readonly string Content = content;
        
        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the message text equals the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text equals the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Text.Equals(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if the message text is not null or empty.
    /// </summary>
    public class TextNotNullOrEmptyFilter() : MessageTextFilter
    {
        /// <summary>
        /// Checks if the message text is not null or empty.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the text is not null or empty; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => !string.IsNullOrEmpty(Text);
    }

    /// <summary>
    /// Filter that checks if the message text contains a 'word'.
    /// 'Word' must be a separate member of the text, and not have any alphabetic characters next to it.
    /// </summary>
    public class TextContainsWordFilter(string word, StringComparison comparison = StringComparison.InvariantCulture, int startIndex = 0) : MessageTextFilter
    {
        /// <summary>
        /// The content to check if the message text equals.
        /// </summary>
        protected readonly string Word = word;

        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// The search starting position.
        /// </summary>
        protected readonly int StartIndex = startIndex;

        /// <inheritdoc/>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
            => Text.ContainsWord(Word, Comparison, StartIndex);
    }
}
