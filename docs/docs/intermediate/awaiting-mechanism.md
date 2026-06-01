---
title: "Awaiting Mechanism"
description: "Pause the handler directly to await the user's next update."
---

# Awaiting Mechanism

Use `AwaitingProvider` to wait for a user's next update (message or callback) inside a handler. This allows for a linear flow in complex multi-step interactions.

```csharp
[CommandHandler]
[CommandAllias("ask")]
public class AskHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");
        
        // Pause execution until the same user sends another message
        Update update = await AwaitingProvider.AwaitAny(HandlingUpdate).BySenderId(cancellation);
        Message nextMessage = update.Message;
        
        await Reply($"Hello, {nextMessage.Text}!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Awaiting Provider**: `container.AwaitMessage()` creates a temporary "waiting" handler.
> 2. **Correlation**: `.BySenderId(cancellation)` ensures the bot only captures the message from the same person in the current chat.
> 3. **Non-blocking**: The internal router handles this using `TaskCompletionSource`, so the thread is freed while waiting.
> 4. **Cleanup**: The temporary handler is automatically removed once the message is received or the token is cancelled.
