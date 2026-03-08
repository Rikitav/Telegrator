using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegrator.Handlers.Diagnostics;
using Telegrator.Logging;

namespace Telegrator.Mediation
{
    /// <summary>
    /// Implementation of <see cref="IUpdateRouter"/> that routes updates to appropriate handlers.
    /// Manages the distribution of updates between regular handlers and awaiting handlers.
    /// </summary>
    public class UpdateRouter : IUpdateRouter
    {
        private readonly TelegratorOptions _options;
        private readonly IHandlersProvider _handlersProvider;
        private readonly IAwaitingProvider _awaitingProvider;
        private readonly IUpdateHandlersPool _HandlersPool;
        private readonly ITelegramBotInfo _botInfo;

        /// <inheritdoc/>
        public IHandlersProvider HandlersProvider => _handlersProvider;

        /// <inheritdoc/>
        public IAwaitingProvider AwaitingProvider => _awaitingProvider;

        /// <inheritdoc/>
        public TelegratorOptions Options => _options;

        /// <inheritdoc/>
        public IUpdateHandlersPool HandlersPool => _HandlersPool;

        /// <inheritdoc/>
        public IRouterExceptionHandler? ExceptionHandler { get; set; }

        /// <inheritdoc/>
        public IHandlerContainerFactory? DefaultContainerFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateRouter"/> class.
        /// </summary>
        /// <param name="handlersProvider">The provider for regular handlers.</param>
        /// <param name="awaitingProvider">The provider for awaiting handlers.</param>
        /// <param name="options">The bot configuration options.</param>
        /// <param name="botInfo"></param>
        public UpdateRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider, TelegratorOptions options, ITelegramBotInfo botInfo)
        {
            _options = options;
            _handlersProvider = handlersProvider;
            _awaitingProvider = awaitingProvider;
            _HandlersPool = new UpdateHandlersPool(this, _options, _options.GlobalCancellationToken);
            _botInfo = botInfo;
        }

        /// <summary>
        /// Handles errors that occur during update processing.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="source">The source of the error.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous error handling operation.</returns>
        public virtual Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
        {
            TelegratorLogging.LogDebug("Handling exception {0}", exception.GetType().Name);
            ExceptionHandler?.HandleException(botClient, exception, source, cancellationToken);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles incoming updates by routing them to appropriate handlers.
        /// </summary>
        /// <param name="botClient">The Telegram bot client.</param>
        /// <param name="update">The update to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous update handling operation.</returns>
        public virtual async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _ = HandleUpdateAsyncInternal(botClient, update, cancellationToken);
        }

        private async Task HandleUpdateAsyncInternal(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // Logging
            LogUpdate(update);

            try
            {
                Result? lastResult = null;
                foreach (DescribedHandlerDescriptor handlerInfo in GetHandlers(AwaitingProvider, botClient, update, cancellationToken))
                {
                    if (lastResult?.NextType != null)
                    {
                        if (lastResult.NextType != handlerInfo.From.HandlerType)
                            continue;
                    }

                    // Enqueuing found awiting handlers
                    await HandlersPool.Enqueue(handlerInfo);
                    await handlerInfo.AwaitResult(cancellationToken).ConfigureAwait(false);

                    lastResult = handlerInfo.Result;
                    if (lastResult == null)
                        break; // Smth went horribly wrong, better to stop routing

                    if (lastResult != null && !lastResult.RouteNext)
                        break;

                    TelegratorLogging.LogTrace("Handler '{0}' requested route continuation (Update {1})", handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
                }

                // Checking if awaiting handlers has exclusive routing
                if (Options.ExclusiveAwaitingHandlerRouting)
                {
                    TelegratorLogging.LogTrace("Receiving Update ({0}) completed with only awaiting handlers", update.Id);
                    return;
                }

                // Queuing reagular handlers for execution
                foreach (DescribedHandlerDescriptor handlerInfo in GetHandlers(HandlersProvider, botClient, update, cancellationToken))
                {
                    if (lastResult?.NextType != null)
                    {
                        if (lastResult.NextType != handlerInfo.From.HandlerType)
                            continue;
                    }

                    // Enqueuing found handlers
                    await HandlersPool.Enqueue(handlerInfo);
                    await handlerInfo.AwaitResult(cancellationToken).ConfigureAwait(false);

                    lastResult = handlerInfo.Result;
                    if (lastResult == null)
                        break; // Smth went horribly wrong, better to stop routing

                    if (lastResult != null && !lastResult.RouteNext)
                        break;

                    TelegratorLogging.LogTrace("Handler '{0}' requested route continuation (Update {1})", handlerInfo.DisplayString, handlerInfo.HandlingUpdate.Id);
                }

                TelegratorLogging.LogTrace("Receiving Update ({0}) finished", update.Id);
            }
            catch (OperationCanceledException)
            {
                TelegratorLogging.LogTrace("Receiving Update ({0}) cancelled", update.Id);
            }
            catch (Exception ex)
            {
                TelegratorLogging.LogDebug("Receiving Update ({0}) finished with exception {1}", update.Id, ex.Message);
                ExceptionHandler?.HandleException(botClient, ex, HandleErrorSource.PollingError, cancellationToken);
            }
        }

        /// <summary>
        /// Gets the handlers that match the specified update, using the provided router and client.
        /// Searches for handlers by update type, falling back to Unknown type if no specific handlers are found.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of described handler information for the update</returns>
        protected virtual IEnumerable<DescribedHandlerDescriptor> GetHandlers(IHandlersProvider provider, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            TelegratorLogging.LogTrace("Requested handlers for UpdateType.{0}", update.Type);
            if (!provider.TryGetDescriptorList(update.Type, out HandlerDescriptorList? descriptors))
            {
                TelegratorLogging.LogTrace("No registered, providing Any");
                provider.TryGetDescriptorList(UpdateType.Unknown, out descriptors);
            }

            if (descriptors == null || descriptors.Count == 0)
            {
                TelegratorLogging.LogTrace("No handlers provided");
                return [];
            }

            return DescribeDescriptors(provider, descriptors, client, update, cancellationToken);
        }

        /// <summary>
        /// Describes all handler descriptors for a given update context.
        /// Processes descriptors in reverse order and respects the ExecuteOnlyFirstFoundHanlder option.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="descriptors">The list of handler descriptors to process</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="cancellationToken"></param>
        /// <returns>A collection of described handler information</returns>
        protected virtual IEnumerable<DescribedHandlerDescriptor> DescribeDescriptors(IHandlersProvider provider, HandlerDescriptorList descriptors, ITelegramBotClient client, Update update, CancellationToken cancellationToken = default)
        {
            TelegratorLogging.LogTrace("Describing descriptors of descriptorsList.HandlingType.{0} for Update ({1})", descriptors.HandlingType, update.Id);
            foreach (HandlerDescriptor descriptor in descriptors.Reverse())
            {
                cancellationToken.ThrowIfCancellationRequested();
                DescribedHandlerDescriptor? describedHandler = DescribeHandler(provider, descriptor, client, update, out bool breakRouting, cancellationToken);
                if (breakRouting)
                    yield break;

                if (describedHandler == null)
                    continue;

                yield return describedHandler;
            }

            TelegratorLogging.LogTrace("Describing for Update ({0}) finished", update.Id);
        }

        /// <summary>
        /// Describes a single handler descriptor for a given update context.
        /// Validates the handler's filters against the update and creates a handler instance if validation passes.
        /// </summary>
        /// <param name="provider">The privode used to get handlers instance</param>
        /// <param name="descriptor">The handler descriptor to process</param>
        /// <param name="client">The Telegram bot client instance</param>
        /// <param name="update">The incoming Telegram update to process</param>
        /// <param name="breakRouting"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>The described handler info if validation passes; otherwise, null</returns>
        public virtual DescribedHandlerDescriptor? DescribeHandler(IHandlersProvider provider, HandlerDescriptor descriptor, ITelegramBotClient client, Update update, out bool breakRouting, CancellationToken cancellationToken = default)
        {
            breakRouting = false;
            cancellationToken.ThrowIfCancellationRequested();
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "handler_name", descriptor.ToString() }
            };

            UpdateHandlerBase handlerInstance = provider.GetHandlerInstance(descriptor, cancellationToken);
            FilterExecutionContext<Update> filterContext = new FilterExecutionContext<Update>(_botInfo, update, update, data, []);

            if (descriptor.Filters != null)
            {
                FiltersFallbackReport report = new FiltersFallbackReport(descriptor, filterContext);
                Result filtersResult = descriptor.Filters.Validate(filterContext, descriptor.FormReport, ref report);

                if (filtersResult.RouteNext)
                {
                    Result fallbackResult = handlerInstance.FiltersFallback(report, client, cancellationToken).Result;
                    breakRouting = !fallbackResult.RouteNext;
                    return null;
                }
                else if (!filtersResult.Positive)
                {
                    return null;
                }
            }

            return new DescribedHandlerDescriptor(descriptor, this, AwaitingProvider, client, handlerInstance, filterContext, descriptor.DisplayString);
        }

        /// <summary>
        /// Methos used to log received <see cref="Update"/> object
        /// </summary>
        /// <param name="update"></param>
        /// <exception cref="NullReferenceException"></exception>
        protected static void LogUpdate(Update update)
        {
            if (TelegratorLogging.MinimalLevel > LogLevel.Trace)
                return;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Received Update ({0}) of type \"{1}\"", update.Id, update.Type);

            switch (update)
            {
                case { Message: { } message }:
                    {
                        if (message is { From: { } from })
                            sb.AppendFormat(" from '{0}' ({1})", from.Username, from.Id);

                        if (message is { Text: { } text })
                            sb.AppendFormat(" with text '{0}'", text);

                        if (message is { Sticker: { } sticker })
                            sb.AppendFormat(" with sticker '{0}'", sticker.Emoji);

                        break;
                    }

                case { CallbackQuery: { } callback }:
                    {
                        if (callback is { From: { } from })
                            sb.AppendFormat(" from '{0}' ({1})", from.Username, from.Id);

                        if (callback is { Data: { } data })
                            sb.AppendFormat(" with data '{0}'", data);

                        break;
                    }
            }

            TelegratorLogging.LogTrace(sb.ToString());
        }

        private class BreakDescribingException : Exception { }
    }
}
