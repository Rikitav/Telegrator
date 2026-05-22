using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Attributes;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;

namespace Telegrator.Handlers;

/// <summary>
/// Attribute that marks a handler to process PollAnswerHandler updates.
/// </summary>
public class PollAnswerHandlerAttribute(int importance = 0) : UpdateHandlerAttribute<PollAnswerHandler>(UpdateType.PollAnswer, importance)
{
    /// <inheritdoc/>
    public override bool CanPass(FilterExecutionContext<Update> context) => context.Input is { PollAnswer: { } };
}

/// <summary>
/// Abstract base class for handlers that process PollAnswerHandler updates.
/// </summary>
public abstract class PollAnswerHandler() : AbstractUpdateHandler<PollAnswer>(UpdateType.PollAnswer)
{
}

