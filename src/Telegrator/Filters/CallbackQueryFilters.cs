using System.Text.RegularExpressions;
using Telegram.Bot.Types;
using Telegrator.Filters.Components;

namespace Telegrator.Filters
{
    /// <summary>
    /// Filter thet checks <see cref="CallbackQuery"/>'s data
    /// </summary>
    public class CallbackDataFilter : Filter<CallbackQuery>
    {
        private readonly string _data;

        /// <summary>
        /// Initialize new instance of <see cref="CallbackDataFilter"/>
        /// </summary>
        /// <param name="data"></param>
        public CallbackDataFilter(string data)
        {
            _data = data;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.Data == _data;
        }
    }

    /// <summary>
    /// Filter that checks if <see cref="CallbackQuery"/> belongs to a specific message
    /// </summary>
    public class CallbackInlineIdFilter : Filter<CallbackQuery>
    {
        private readonly string _inlineMessageId;

        /// <summary>
        /// Initialize new instance of <see cref="CallbackInlineIdFilter"/>
        /// </summary>
        /// <param name="inlineMessageId"></param>
        public CallbackInlineIdFilter(string inlineMessageId)
        {
            _inlineMessageId = inlineMessageId;
        }

        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<CallbackQuery> context)
        {
            return context.Input.InlineMessageId == _inlineMessageId;
        }
    }

    /// <summary>
    /// Filters callback queries by matching their data with a regular expression.
    /// </summary>
    public class CallbackRegexFilter : RegexFilterBase<CallbackQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackRegexFilter"/> class with a pattern and options.
        /// </summary>
        /// <param name="pattern">The regex pattern.</param>
        /// <param name="regexOptions">The regex options.</param>
        public CallbackRegexFilter(string pattern, RegexOptions regexOptions = default)
            : base(clb => clb.Data, pattern, regexOptions) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackRegexFilter"/> class with a regex object.
        /// </summary>
        /// <param name="regex">The regex object.</param>
        public CallbackRegexFilter(Regex regex)
            : base(clb => clb.Data, regex) { }
    }
}
