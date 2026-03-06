using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegrator.Attributes;
using Telegrator.Filters.Components;
using Telegrator.Handlers.Components;

namespace Telegrator.Handlers
{
    /// <summary>
    /// Attribute that marks a handler to process inline queries.
    /// IMPORTANT! You can have only ONE instance of this handler.
    /// </summary>
    public class InlineQueryHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<InlineQueryHandler>(UpdateType.InlineQuery, importance)
    {
        /// <inheritdoc/>
        public override bool CanPass(FilterExecutionContext<Update> context) => context.Input.InlineQuery is { } | context.Input.ChosenInlineResult is { };
    }

    /// <summary>
    /// Abstract base class for handlers that process inline queries.
    /// IMPORTANT! You can have only ONE instance of this handler.
    /// </summary>
    public abstract class InlineQueryHandler() : AbstractUpdateHandler<Update>(UpdateType.InlineQuery)
    {
        /// <summary>
        /// Handler container for the current <see cref="InlineQuery"/> update.
        /// </summary>
        protected IHandlerContainer<InlineQuery> QueryContainer { get; private set; } = null!;

        /// <summary>
        /// Handler container for the current <see cref="ChosenInlineResult"/> update.
        /// </summary>
        protected IHandlerContainer<ChosenInlineResult> ChosenContainer { get; private set; } = null!;

        /// <summary>
        /// Incoming update of type <see cref="InlineQuery"/>.
        /// </summary>
        protected InlineQuery InputQuery { get; private set; } = null!;

        /// <summary>
        /// Incoming update of type <see cref="ChosenInlineResult"/>.
        /// </summary>
        protected ChosenInlineResult InputChosen { get; private set; } = null!;

        /// <inheritdoc/>
        public override async Task<Result> Execute(IHandlerContainer<Update> container, CancellationToken cancellation)
        {
            switch (container.HandlingUpdate.Type)
            {
                case UpdateType.InlineQuery:
                    {
                        QueryContainer = HandlerContainer<InlineQuery>.From(container);
                        InputQuery = QueryContainer.ActualUpdate;
                        return await Requested(QueryContainer, cancellation).ConfigureAwait(false);
                    }

                case UpdateType.ChosenInlineResult:
                    {
                        ChosenContainer = HandlerContainer<ChosenInlineResult>.From(container);
                        InputChosen = ChosenContainer.ActualUpdate;
                        return await Chosen(ChosenContainer, cancellation).ConfigureAwait(false);
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Executes handler logic if received update is <see cref="UpdateType.InlineQuery"/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public abstract Task<Result> Requested(IHandlerContainer<InlineQuery> container, CancellationToken cancellation);

        /// <summary>
        /// Executes handler logic if received update is <see cref="UpdateType.ChosenInlineResult"/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public abstract Task<Result> Chosen(IHandlerContainer<ChosenInlineResult> container, CancellationToken cancellation);

        /// <summary>
        /// Answers inline query
        /// </summary>
        /// <param name="results"></param>
        /// <param name="cacheTime"></param>
        /// <param name="isPersonal"></param>
        /// <param name="nextOffset"></param>
        /// <param name="button"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task Answer(
            IEnumerable<InlineQueryResult> results,
            int? cacheTime = null,
            bool isPersonal = false,
            string? nextOffset = null,
            InlineQueryResultsButton? button = null,
            CancellationToken cancellationToken = default)
            => await QueryContainer.AnswerInlineQuery(
                results, cacheTime,
                isPersonal, nextOffset,
                button, cancellationToken);
    }
}
