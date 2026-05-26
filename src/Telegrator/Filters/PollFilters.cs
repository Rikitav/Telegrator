using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Filters;

namespace Telegrator.Filters;

/// <summary>
/// Filter that checks <see cref="Poll"/>'s type
/// </summary>
public class PollTypeFilter(PollType pollType) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.Type == pollType;
    }
}

/// <summary>
/// Filter that checks if <see cref="Poll"/> is closed
/// </summary>
public class PollIsClosedFilter(bool isClosed = true) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.IsClosed == isClosed;
    }
}

/// <summary>
/// Filter that checks if <see cref="Poll"/> is anonymous
/// </summary>
public class PollIsAnonymousFilter(bool isAnonymous = true) : Filter<Poll>
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Poll> context)
    {
        return context.Input.IsAnonymous == isAnonymous;
    }
}
