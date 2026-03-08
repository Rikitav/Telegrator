using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Core.Descriptors
{
    /// <summary>
    /// Contains information about a described handler, including its context, client, and execution logic.
    /// </summary>
    public class DescribedHandlerDescriptor
    {
        private readonly ManualResetEventSlim ResetEvent = new ManualResetEventSlim(false);

        /// <summary>
        /// Descriptor from that handler was described from.
        /// </summary>
        public HandlerDescriptor From { get; }

        /// <summary>
        /// The update router associated with this handler.
        /// </summary>
        public IUpdateRouter UpdateRouter { get; }

        /// <summary>
        /// The awaiting provider to fetch new updates inside handler
        /// </summary>
        public IAwaitingProvider AwaitingProvider { get; }

        /// <summary>
        /// The Telegram bot client used for this handler.
        /// </summary>
        public ITelegramBotClient Client { get; }

        /// <summary>
        /// The handler instance being described.
        /// </summary>
        public UpdateHandlerBase HandlerInstance { get; }

        /// <summary>
        /// Extra data associated with the handler execution.
        /// </summary>
        public Dictionary<string, object> ExtraData { get; }

        /// <summary>
        /// List of completed filters for this handler.
        /// </summary>
        public CompletedFiltersList CompletedFilters { get; }

        /// <summary>
        /// The update being handled.
        /// </summary>
        public Update HandlingUpdate { get; }

        /// <summary>
        /// Lifetime token for the handler instance.
        /// </summary>
        public HandlerLifetimeToken HandlerLifetime => HandlerInstance.LifetimeToken;

        /// <summary>
        /// Display string for the handler (for debugging or logging).
        /// </summary>
        public string DisplayString { get; set; }

        /// <summary>
        /// The final execution result.
        /// </summary>
        public Result? Result { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescribedHandlerDescriptor"/> class.
        /// </summary>
        /// <param name="fromDescriptor">The descriptor from which this handler was described.</param>
        /// <param name="updateRouter">The update router.</param>
        /// <param name="awaitingProvider">The awaiting provider.</param>
        /// <param name="client">The Telegram bot client.</param>
        /// <param name="handlerInstance">The handler instance.</param>
        /// <param name="filterContext">The filter execution context.</param>
        /// <param name="displayString">Optional display string.</param>
        public DescribedHandlerDescriptor(
            HandlerDescriptor fromDescriptor,
            IUpdateRouter updateRouter,
            IAwaitingProvider awaitingProvider,
            ITelegramBotClient client,
            UpdateHandlerBase handlerInstance,
            FilterExecutionContext<Update> filterContext,
            string? displayString)
        {
            From = fromDescriptor;
            UpdateRouter = updateRouter;
            AwaitingProvider = awaitingProvider;
            Client = client;
            HandlerInstance = handlerInstance;
            ExtraData = filterContext.Data;
            CompletedFilters = filterContext.CompletedFilters;
            HandlingUpdate = filterContext.Update;
            DisplayString = displayString ?? fromDescriptor.HandlerType.Name;
        }

        /// <summary>
        /// Waits for the handler execution result.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task AwaitResult(CancellationToken cancellationToken)
        {
            await Task.Yield();
            ResetEvent.Reset();
            ResetEvent.Wait(cancellationToken);
        }

        /// <summary>
        /// Reports the execution result and signals completion.
        /// </summary>
        /// <param name="result">The execution result.</param>
        public void ReportResult(Result? result)
        {
            if (Result != null)
                throw new InvalidOperationException("Result already reported");

            Result = result;
            ResetEvent.Set();
        }

        /// <inheritdoc/>
        public override string ToString()
            => DisplayString ?? From.HandlerType.Name;
    }
}
