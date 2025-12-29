using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace Telegrator.Polling
{
    /// <summary>
    /// Reactive implementation of <see cref="IUpdateReceiver"/> for polling updates from Telegram.
    /// /// Provides custom update receiving logic with error handling and configuration options.
    /// </summary>
    /// <param name="client">The Telegram bot client for making API requests.</param>
    /// <param name="options">Optional receiver options for configuring update polling behavior.</param>
    public class ReactiveUpdateReceiver(ITelegramBotClient client, ReceiverOptions? options) : IUpdateReceiver
    {
        /// <summary>
        /// Gets the receiver options for configuring update polling behavior.
        /// </summary>
        public readonly ReceiverOptions? Options = options;
        
        /// <summary>
        /// Gets the Telegram bot client for making API requests.
        /// </summary>
        public readonly ITelegramBotClient Client = client;

        /// <summary>
        /// Receives updates from Telegram using long polling.
        /// Handles update processing, error handling, and cancellation.
        /// </summary>
        /// <param name="updateHandler">The update handler to process received updates.</param>
        /// <param name="cancellationToken">The cancellation token to stop receiving updates.</param>
        /// <returns>A task representing the asynchronous update receiving operation.</returns>
        public async Task ReceiveAsync(IUpdateHandler updateHandler, CancellationToken cancellationToken)
        {
            cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).Token;
            GetUpdatesRequest request = new GetUpdatesRequest()
            {
                AllowedUpdates = Options?.AllowedUpdates ?? [],
                Limit = Options?.Limit.GetValueOrDefault(100),
                Offset = Options?.Offset
            };

            if (Options?.DropPendingUpdates ?? false)
            {
                try
                {
                    Update[] array = await Client.GetUpdates(-1, 1, 0, [], cancellationToken).ConfigureAwait(false);
                    request.Offset = array.Length != 0 ? array[^1].Id + 1 : 0;
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    request.Timeout = (int)Client.Timeout.TotalSeconds;
                    foreach (Update update in await Client.SendRequest(request, cancellationToken).ConfigureAwait(false))
                    {
                        try
                        {
                            request.Offset = update.Id + 1;
                            await updateHandler.HandleUpdateAsync(Client, update, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
                        }
                        catch (Exception exception2)
                        {
                            await updateHandler.HandleErrorAsync(Client, exception2,
                                HandleErrorSource.HandleUpdateError, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception exception)
                {
                    await updateHandler.HandleErrorAsync(Client, exception, HandleErrorSource.PollingError, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
