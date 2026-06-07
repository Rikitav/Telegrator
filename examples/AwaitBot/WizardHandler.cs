/*
 * Copyright (c) 2026 Rikitav Tim4ik
 */

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Attributes;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;

namespace Telegrator.Examples;

/// <summary>
/// Demonstrates a multi-step conversation wizard using the awaiting mechanism.
/// The handler pauses execution and waits for the user's next messages inline.
/// </summary>
[MessageHandler]
[MightAwait(UpdateType.Message)]
public class AwaitHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input?.Text is not { } text)
            return Ok;

        if (text.Equals("/start", StringComparison.OrdinalIgnoreCase))
        {
            await Reply("Welcome to the wizard! What is your name?", cancellationToken: cancellation);

            Message? nameMessage = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
            if (nameMessage?.Text is not { } name)
            {
                await Reply("Invalid input. Wizard cancelled.", cancellationToken: cancellation);
                return Ok;
            }

            await Reply($"Nice to meet you, {name}! How old are you?", cancellationToken: cancellation);

            Message? ageMessage = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
            if (ageMessage?.Text is not { } ageText || !int.TryParse(ageText, out int age))
            {
                await Reply("Invalid age. Wizard cancelled.", cancellationToken: cancellation);
                return Ok;
            }

            await Reply(
                $"Thank you, {name}! You are {age} years old.\\n\\nWizard completed!",
                cancellationToken: cancellation);

            return Ok;
        }

        await Reply("Send /start to begin the wizard.", cancellationToken: cancellation);
        return Ok;
    }
}
