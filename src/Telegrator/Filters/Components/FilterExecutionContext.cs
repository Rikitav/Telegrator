using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Configuration;

namespace Telegrator.Filters.Components
{
    /// <summary>
    /// Represents the context for filter execution, including update, input, and additional data.
    /// </summary>
    /// <typeparam name="T">The type of the input for the filter.</typeparam>
    public class FilterExecutionContext<T> where T : class
    {
        /// <summary>
        /// Gets the <see cref="ITelegramBotInfo"/> for the current context.
        /// </summary>
        public ITelegramBotInfo BotInfo { get; }

        /// <summary>
        /// Gets the additional data dictionary for the context.
        /// </summary>
        public Dictionary<string, object> Data { get; }

        /// <summary>
        /// Gets the list of completed filters for the context.
        /// </summary>
        public CompletedFiltersList CompletedFilters { get; }

        /// <summary>
        /// Gets the <see cref="Update"/> being processed.
        /// </summary>
        public Update Update { get; }

        /// <summary>
        /// Gets the <see cref="UpdateType"/> of the update.
        /// </summary>
        public UpdateType Type { get; }

        /// <summary>
        /// Gets the input object for the filter.
        /// </summary>
        public T Input { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExecutionContext{T}"/> class with all parameters.
        /// </summary>
        /// <param name="botInfo">The bot info.</param>
        /// <param name="update">The update.</param>
        /// <param name="input">The input object.</param>
        /// <param name="data">The additional data dictionary.</param>
        /// <param name="completedFilters">The list of completed filters.</param>
        public FilterExecutionContext(ITelegramBotInfo botInfo, Update update, T input, Dictionary<string, object> data, CompletedFiltersList completedFilters)
        {
            BotInfo = botInfo;
            Data = data;
            CompletedFilters = completedFilters;
            Update = update;
            Type = update.Type;
            Input = input;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExecutionContext{T}"/> class with default data and filters.
        /// </summary>
        /// <param name="botInfo">The bot info.</param>
        /// <param name="update">The update.</param>
        /// <param name="input">The input object.</param>
        public FilterExecutionContext(ITelegramBotInfo botInfo, Update update, T input)
            : this(botInfo, update, input, [], []) { }

        /// <summary>
        /// Creates a child context for a different input type, sharing the same data and completed filters.
        /// </summary>
        /// <typeparam name="C">The type of the new input.</typeparam>
        /// <param name="input">The new input object.</param>
        /// <returns>A new <see cref="FilterExecutionContext{C}"/> instance.</returns>
        public FilterExecutionContext<C> CreateChild<C>(C input) where C : class
            => new FilterExecutionContext<C>(BotInfo, Update, input, Data, CompletedFilters);
    }
}
