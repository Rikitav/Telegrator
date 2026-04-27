using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegrator.Core;
using WUpdate = WTelegram.Types.Update;

namespace Telegrator.Mediation;

/// <summary>
/// Reactive implementation of <see cref="IUpdateReceiver"/> for polling updates from Telegram.
/// Provides custom update receiving logic with error handling and configuration options.
/// </summary>
/// <param name="client">The Telegram bot client for making API requests.</param>
// <param name="options">Optional receiver options for configuring update polling behavior.</param>
public class WideUpdateReceiver(WTelegramBotClient client) : IUpdateReceiver
{
    private readonly WTelegramBotClient _client = client;
    private IUpdateHandler? _updateHandler = null;
    private CancellationToken _cancellation = default;

    /// <inheritdoc/>
    public async Task ReceiveAsync(IUpdateHandler updateHandler, CancellationToken cancellationToken = default)
    {
        _updateHandler = updateHandler;
        _cancellation = cancellationToken;

        TaskCompletionSource<object> tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        await using CancellationTokenRegistration registration = cancellationToken.Register(() => tcs.TrySetResult(null!));

        try
        {
            _client.OnUpdate += OnUpdate;
            await tcs.Task.ConfigureAwait(false);
        }
        finally
        {
            _client.OnUpdate -= OnUpdate;
        }
    }

    private async Task OnUpdate(WUpdate update)
    {
        if (_updateHandler == null)
            throw new Exception("Router not initialized (got null)");

        try
        {
            await _updateHandler.HandleUpdateAsync(_client, update, _cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            await _updateHandler.HandleErrorAsync(_client, ex, HandleErrorSource.HandleUpdateError, _cancellation).ConfigureAwait(false);
        }
    }
}