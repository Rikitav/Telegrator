using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Aspects;
using Telegrator.Core.Handlers;

namespace Telegrator.Essentials.Aspects;

/// <summary>
/// Post-processor that automatically deletes a bot message after a configured delay.
/// The handler must schedule the message using <see cref="Extensions.AutoDeleteExtensions.ScheduleAutoDelete"/>.
/// </summary>
public class AutoDeletePostProcessor : IPostProcessor
{
    /// <summary>
    /// Extra data key used to store the message to delete.
    /// </summary>
    public const string MessageKey = "Telegrator.Essentials.AutoDelete.Message";

    /// <summary>
    /// Extra data key used to store the deletion delay.
    /// </summary>
    public const string DelayKey = "Telegrator.Essentials.AutoDelete.Delay";

    /// <inheritdoc/>
    public async Task<Result> AfterExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
    {
        if (!container.ExtraData.TryGetValue(MessageKey, out object? messageValue)
            || messageValue is not Message message)
        {
            return Result.Ok();
        }

        TimeSpan delay = container.ExtraData.TryGetValue(DelayKey, out object? delayValue) && delayValue is TimeSpan ts
            ? ts
            : TimeSpan.FromSeconds(5);

        if (delay <= TimeSpan.Zero)
            return Result.Ok();

        _ = DeleteAfterDelayAsync(container, message, delay, cancellationToken);
        return Result.Ok();
    }

    private static async Task DeleteAfterDelayAsync(IHandlerContainer container, Message message, TimeSpan delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

            if (message.Chat.Id != 0 && message.MessageId != 0)
            {
                await container.Client.DeleteMessage(message.Chat.Id, message.MessageId, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when the host is shutting down.
        }
        catch
        {
            // Deletion is best-effort; swallow exceptions to avoid breaking the pipeline.
        }
    }
}
