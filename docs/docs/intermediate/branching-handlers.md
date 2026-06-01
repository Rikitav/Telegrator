---
title: "Branching Handlers"
description: "Combine multiple related routes in a single handler."
---

# Branching Handlers

For complex scenarios where a single handler needs to handle multiple different update types or conditions, you can use `BranchingUpdateHandler` to create handlers with multiple entry points.

**Example:**
```csharp
[MessageHandler]
public class ComplexHandler : BranchingMessageHandler
{
    [TextContains("hello")]
    public async Task<Result> HandleGreeting(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello there!");
        return Ok;
    }

    [TextContains("help")]
    public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("How can I help you?");
        return Ok;
    }

    [FromUser("John")]
    public async Task<Result> HandleAdmin(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Admin command received!");
        return Ok;
    }
}
```

**Branching Command Handler:**
```csharp
[CommandHandler]
public class SettingsHandler : BranchingCommandHandler
{
    [CommandAllias("settings")]
    public async Task<Result> ShowSettings(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Settings menu:");
        // Show settings options
        return Ok;
    }

    [CommandAllias("settings", "language")]
    public async Task<Result> SetLanguage(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var language = Arguments.FirstOrDefault();
        await Reply($"Language set to: {language}");
        return Ok;
    }

    [CommandAllias("settings", "theme")]
    public async Task<Result> SetTheme(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var theme = Arguments.FirstOrDefault();
        await Reply($"Theme set to: {theme}");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Multiple Entry Points**: Each method with filters becomes a separate handler entry point.
> 2. **Individual Filtering**: Each method can have its own set of filters and conditions.
> 3. **Shared Context**: All methods share the same handler instance and context.
> 4. **Automatic Registration**: Each method is automatically registered as a separate handler.
> 5. **Command Arguments**: In `BranchingCommandHandler`, you can access command arguments via the `Arguments` property.
> 6. **Flexible Routing**: Perfect for complex bots with many related commands or message patterns.
