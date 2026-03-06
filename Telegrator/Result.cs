using Telegram.Bot.Types;
using Telegrator.Aspects;
using Telegrator.Handlers.Components;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore;
using Telegrator.Handlers.Diagnostics;

namespace Telegrator
{
    /// <summary>
    /// Represents handler results, allowing to communicate with router and control aspect execution
    /// </summary>
    public sealed class Result
    {
        private static readonly Result ok = new Result(true, false, null);
        private static readonly Result fault = new Result(false, false, null);
        private static readonly Result next = new Result(true, true, null);

        /// <summary>
        /// Is result positive
        /// </summary>
        public bool Positive { get; }

        /// <summary>
        /// Should router search for next matching handler
        /// </summary>
        public bool RouteNext { get; }

        /// <summary>
        /// Exact type that router should search
        /// </summary>
        public Type? NextType { get; }

        internal Result(bool positive, bool routeNext, Type? nextType)
        {
            Positive = positive;
            RouteNext = routeNext;
            NextType = nextType;
        }

        /// <summary>
        /// Represents 'success'
        /// <list type="bullet">
        /// <item>Inside <see cref="IPreProcessor"/> - let handler's main block be executed</item>
        /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - tells <see cref="IUpdateRouter"/> that he can stop describing, as needed handler was found</item>
        /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - let <see cref="IUpdateRouter"/> continue describing</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static Result Ok()
            => ok;

        /// <summary>
        /// Represents 'fault' or 'error'. Use cases:
        /// <list type="bullet">
        /// <item>Inside <see cref="IPreProcessor"/> - interupts execution of handler, main block and <see cref="IPostProcessor"/> wont be executed</item>
        /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - interupts <see cref="IUpdateRouter"/>'s describing sequence</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static Result Fault()
            => fault;

        /// <summary>
        /// Represents 'continue'. Use cases:
        /// <list type="bullet">
        /// <item>Inside <see cref="UpdateHandlerBase.FiltersFallback(FiltersFallbackReport, Telegram.Bot.ITelegramBotClient, CancellationToken)"/> - let <see cref="IUpdateRouter"/> continue describing</item>
        /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - Tells <see cref="IUpdateRouter"/> to continue describing handlers</item>
        /// </list>
        /// </summary>
        /// <returns></returns>
        public static Result Next()
            => next;

        /// <summary>
        /// Represents 'chain'. Use cases:
        /// <list type="bullet">
        /// <item>Inside <see cref="UpdateHandlerBase.ExecuteInternal(IHandlerContainer, CancellationToken)"/> - Tells <see cref="IUpdateRouter"/> to continue describing handlers and execute only handlers of exact type</item>
        /// </list>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Result Next<T>()
            => new Result(true, true, typeof(T));
    }
}
