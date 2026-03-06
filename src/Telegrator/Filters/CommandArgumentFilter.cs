using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;
using Telegrator.Handlers;

namespace Telegrator.Filters
{
    /// <summary>
    /// Abstract base class for filters that operate on command arguments.
    /// Provides functionality to extract and validate command arguments from message text.
    /// </summary>
    /// <param name="index">The index of the argument to filter (0-based).</param>
    public abstract class CommandArgumentFilterBase(int index) : Filter<Message>
    {
        /// <summary>
        /// Gets the chosen argument at the specified index.
        /// </summary>
        protected string Target { get; private set; } = null!;

        /// <summary>
        /// Determines whether the filter can pass by extracting the command argument and validating it.
        /// </summary>
        /// <param name="context">The filter execution context containing the message.</param>
        /// <returns>True if the argument exists and passes validation; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            CommandHandlerAttribute attr = context.CompletedFilters.Get<CommandHandlerAttribute>(0);
            string[] args = attr.Arguments ??= context.Input.SplitArgs();
            Target = args.ElementAtOrDefault(index);

            if (Target == null)
                return false;

            return CanPassNext(context);
        }

        /// <summary>
        /// Determines whether the filter can pass for the given context.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the filter passes; otherwise, false.</returns>
        protected abstract bool CanPassNext(FilterExecutionContext<Message> context);
    }

    /// <summary>
    /// Filter that checks if a command has arguments count >= <paramref name="count"/>.
    /// </summary>
    /// <param name="count"></param>
    public class ArgumentCountFilter(int count) : Filter<Message>
    {
        private readonly int Count = count;

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Message> context)
        {
            CommandHandlerAttribute attr = context.CompletedFilters.Get<CommandHandlerAttribute>(0);
            string[] args = attr.Arguments ??= context.Input.SplitArgs();
            return args.Length >= Count;
        }
    }

    /// <summary>
    /// Filter that checks if a command argument starts with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the argument starts with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    /// <param name="index">The index of the argument to check (0-based).</param>
    public class ArgumentStartsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        /// <summary>
        /// The content to check if the argument starts with.
        /// </summary>
        protected readonly string Content = content;

        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the command argument starts with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the argument starts with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.StartsWith(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if a command argument ends with a specified content.
    /// </summary>
    /// <param name="content">The content to check if the argument ends with.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    /// <param name="index">The index of the argument to check (0-based).</param>
    public class ArgumentEndsWithFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        /// <summary>
        /// The content to check if the argument ends with.
        /// </summary>
        protected readonly string Content = content;

        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the command argument ends with the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the argument ends with the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.EndsWith(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if a command argument contains a specified content.
    /// </summary>
    /// <param name="content">The content to check if the argument contains.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    /// <param name="index">The index of the argument to check (0-based).</param>
    public class ArgumentContainsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        /// <summary>
        /// The content to check if the argument contains.
        /// </summary>
        protected readonly string Content = content;

        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the command argument contains the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the argument contains the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.IndexOf(Content, Comparison) >= 0;
    }

    /// <summary>
    /// Filter that checks if a command argument equals a specified content.
    /// </summary>
    /// <param name="content">The content to check if the argument equals.</param>
    /// <param name="comparison">The string comparison type to use for the check.</param>
    /// <param name="index">The index of the argument to check (0-based).</param>
    public class ArgumentEqualsFilter(string content, StringComparison comparison = StringComparison.InvariantCulture, int index = 0) : CommandArgumentFilterBase(index)
    {
        /// <summary>
        /// The content to check if the argument equals.
        /// </summary>
        protected readonly string Content = content;

        /// <summary>
        /// The string comparison type to use for the check.
        /// </summary>
        protected readonly StringComparison Comparison = comparison;

        /// <summary>
        /// Checks if the command argument equals the specified content using the configured comparison.
        /// </summary>
        /// <param name="_">The filter execution context (unused).</param>
        /// <returns>True if the argument equals the specified content; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> _)
            => Target.Equals(Content, Comparison);
    }

    /// <summary>
    /// Filter that checks if a command argument matches a regular expression pattern.
    /// </summary>
    /// <param name="regex">The regular expression to match against the argument.</param>
    /// <param name="index">The index of the argument to check (0-based).</param>
    public class ArgumentRegexFilter(Regex regex, int index = 0) : CommandArgumentFilterBase(index)
    {
        private readonly Regex _regex = regex;

        /// <summary>
        /// Gets the match found by the regex.
        /// </summary>
        public Match Match { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of <see cref="ArgumentRegexFilter"/> with a regex pattern.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match.</param>
        /// <param name="options">The regex options to use.</param>
        /// <param name="matchTimeout">The timeout for the regex match operation.</param>
        /// <param name="index">The index of the argument to check (0-based).</param>
        public ArgumentRegexFilter(string pattern, RegexOptions options = RegexOptions.None, TimeSpan matchTimeout = default, int index = 0)
            : this(new Regex(pattern, options, matchTimeout), index) { }

        /// <summary>
        /// Checks if the command argument matches the regular expression pattern.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the argument matches the regex pattern; otherwise, false.</returns>
        protected override bool CanPassNext(FilterExecutionContext<Message> context)
        {
            Match = _regex.Match(Target);
            return Match.Success;
        }
    }
}
