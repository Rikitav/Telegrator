using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Logging;

namespace Telegrator;

/// <summary>
/// Represents a continuous chat action that runs in the background until cancelled or disposed.
/// </summary>
public class ContinuousAction : IDisposable
{
    private readonly ITelegramBotClient _client;
    private readonly ChatAction _action;
    private readonly ChatId _chat;
    private readonly TimeSpan _delay;

    private readonly CancellationTokenSource _linkedCts;
    private readonly Task _workerTask;

    private int _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContinuousAction"/> class.
    /// </summary>
    /// <param name="client">The Telegram bot client.</param>
    /// <param name="chat">The target chat.</param>
    /// <param name="action">The action to perform continuously.</param>
    /// <param name="delay">The delay between actions. Defaults to 4 seconds.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public ContinuousAction(ITelegramBotClient client, ChatId chat, ChatAction action, TimeSpan? delay = null, CancellationToken cancellationToken = default)
    {
        _client = client;
        _chat = chat;
        _action = action;
        _delay = delay ?? TimeSpan.FromSeconds(4);

        _linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _workerTask = StartActorAsync(_linkedCts.Token);
    }

    private async Task StartActorAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await _client.SendChatAction(_chat, _action, cancellationToken: token).ConfigureAwait(false);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    TelegratorLogging.LogTrace("Continuous action throwed an exception.\n{0}", ex);
                }

                await Task.Delay(_delay, token);
            }
        }
        catch (OperationCanceledException)
        {
            TelegratorLogging.LogTrace("Continuous action cancelled");
        }
    }

    /// <summary>
    /// Cancels the continuous action.
    /// </summary>
    public void Cancel()
    {
        if (Interlocked.CompareExchange(ref _disposed, 0, 0) == 0)
        {
            try
            {
                _linkedCts.Cancel();
            }
            catch (ObjectDisposedException)
            {
                _ = 0xDEADBEEF;
            }
        }
    }

    /// <summary>
    /// Waits for the background worker task to complete.
    /// </summary>
    public async Task WaitAsync()
    {
        await _workerTask.ConfigureAwait(false);
    }

    /// <summary>
    /// Disposes the instance and stops the continuous action.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
            return;

        try
        {
            _linkedCts.Cancel();
        }
        finally
        {
            _linkedCts.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
