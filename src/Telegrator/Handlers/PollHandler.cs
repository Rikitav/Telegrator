using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process PollHandler updates.
/// </summary>
public class PollHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PollHandler>(UpdateType.Poll, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { Poll: { } };
}

/// <summary>
/// Abstract base class for handlers that process PollHandler updates.
/// </summary>
public abstract class PollHandler() : AbstractUpdateHandler<Poll>(UpdateType.Poll)
{
}


/// <summary>
/// Abstract base class for branching handlers that process PollHandler updates.
/// </summary>
public abstract class BranchingPollHandler() : BranchingUpdateHandler<Poll>(UpdateType.Poll)
{
}

