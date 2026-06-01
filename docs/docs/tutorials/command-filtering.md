---
title: "Command Filtering"
description: "Tutorial for separating commands like /start."
---

# Command Filtering

Message is considered a command if it starts with '/' and has a not-null or not-empty name.
Instead of using the `MessageHandler` for commands (such as `/start`), you should use `CommandHandler`.

```csharp
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Use /help to see available commands.");
        return Ok;
    }
}

[CommandHandler]
[CommandAllias("help")]
public class HelpHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Available commands:\n/start - Start the bot\n/help - Show this help");
        return Ok;
    }
}

[MessageHandler]
[TextStartsWith("/", Modifiers = FilterModifier.Not)]
public class EchoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"You said: \"{Input.Text}\"");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Command Handlers**: `[CommandHandler]` and `[CommandAllias]` work together to handle specific commands like `/start` and `/help`.
> 2. **Echo Handler**: `[TextStartsWith("/", Modifiers = FilterModifier.Not)]` catches all messages that don't start with "/" (non-commands).
> 3. **Handler Separation**: Each command has its own dedicated handler, making the code modular and maintainable.
> 4. **Filter Modifiers**: The `Not` modifier inverts the filter logic to exclude command messages.
