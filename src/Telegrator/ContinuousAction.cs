using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Logging;

namespace Telegrator;

public class ContinuousAction : IDisposable
{
    private readonly ITelegramBotClient _client;
    private readonly ChatAction _action;
    private readonly ChatId _chat;
    private readonly TimeSpan _delay;

    private readonly CancellationTokenSource _linkedCts;
    private readonly Task _workerTask;

    private int _disposed;

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

    public async Task WaitAsync()
    {
        await _workerTask.ConfigureAwait(false);
    }

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
    }
}
