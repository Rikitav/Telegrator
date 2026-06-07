/*
 * Copyright (c) 2026 Rikitav Tim4ik
 */

using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;

namespace Telegrator.Examples;

[MessageHandler]
public class EchoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input?.Text is not { Length: > 0 } text)
            return Ok;

        await Reply($"Echo: {text}", cancellationToken: cancellation);
        return Ok;
    }
}
