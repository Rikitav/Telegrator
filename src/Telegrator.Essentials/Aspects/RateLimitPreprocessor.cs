using Telegram.Bot.Types;
using Telegrator.Aspects;
using Telegrator.Core.Handlers;

namespace Telegrator.Essentials.Aspects;

/// <summary>
/// Pre-processor that limits how many updates a single user can trigger within a sliding time window.
/// State is persisted via <see cref="IHandlerContainer.StateStorage"/> so the limit survives restarts
/// when a persistent storage implementation is registered.
/// </summary>
public class RateLimitPreprocessor : IPreProcessor
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed inside the window.
    /// Override in a derived class or configure via an options object to change defaults.
    /// </summary>
    public virtual int MaxRequests { get; set; } = 10;

    /// <summary>
    /// Gets or sets the sliding window size in seconds.
    /// </summary>
    public virtual int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the state key prefix. The final key is <c>{Prefix}:{userId}</c>.
    /// </summary>
    public virtual string StateKeyPrefix { get; set; } = "telegrator.essentials.ratelimit";

    /// <inheritdoc/>
    public async Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
    {
        long? userId = ResolveUserId(container.HandlingUpdate);
        if (userId is null)
            return Result.Ok();

        string key = $"{StateKeyPrefix}:{userId}";
        DateTime now = DateTime.UtcNow;
        DateTime windowStart = now.AddSeconds(-WindowSeconds);

        List<DateTime> requests = await container.StateStorage.GetAsync<List<DateTime>>(key, cancellationToken).ConfigureAwait(false) ?? [];
        requests.RemoveAll(t => t < windowStart);

        if (requests.Count >= MaxRequests)
            return Result.Fault();

        requests.Add(now);
        await container.StateStorage.SetAsync(key, requests, cancellationToken).ConfigureAwait(false);

        return Result.Ok();
    }

    /// <summary>
    /// Resolves the user identifier from the incoming update.
    /// </summary>
    protected virtual long? ResolveUserId(Update update)
    {
        return update.Message?.From?.Id
            ?? update.CallbackQuery?.From?.Id
            ?? update.InlineQuery?.From.Id
            ?? update.ChosenInlineResult?.From.Id
            ?? update.PollAnswer?.User?.Id
            ?? update.MyChatMember?.From.Id
            ?? update.ChatMember?.From.Id
            ?? update.ChatJoinRequest?.From.Id
            ?? update.MessageReaction?.User?.Id
            ?? update.BusinessConnection?.User?.Id;
    }
}
