using System.Text.RegularExpressions;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Base class for filters that use regular expressions to match text in updates.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public abstract class RegexFilterBase<T> : Filter<T> where T : class
    {
        private readonly Func<T, string?> getString;
        private readonly Regex regex;

        /// <summary>
        /// Gets the collection of matches found by the regex.
        /// </summary>
        public MatchCollection Matches { get; private set; } = null!;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexFilterBase{T}"/> class with a regex object.
        /// </summary>
        /// <param name="getString">Function to extract the string to match from the input.</param>
        /// <param name="regex">The regex object to use for matching.</param>
        protected RegexFilterBase(Func<T, string?> getString, Regex regex)
        {
            this.getString = getString;
            this.regex = regex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegexFilterBase{T}"/> class with a pattern and options.
        /// </summary>
        /// <param name="getString">Function to extract the string to match from the input.</param>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="regexOptions">The regex options.</param>
        protected RegexFilterBase(Func<T, string?> getString, string pattern, RegexOptions regexOptions = default)
        {
            this.getString = getString;
            regex = new Regex(pattern, regexOptions);
        }

        /// <summary>
        /// Determines whether the regex matches the text extracted from the input.
        /// </summary>
        /// <param name="context">The filter execution context.</param>
        /// <returns>True if the regex matches; otherwise, false.</returns>
        public override bool CanPass(FilterExecutionContext<T> context)
        {
            string? text = getString.Invoke(context.Input);
            if (string.IsNullOrEmpty(text))
                return false;

            Matches = regex.Matches(text);
            return Matches.Count > 0;
        }
    }
}
