---
title: "Branching Handlers"
description: "Combine multiple related routes in a single handler."
---

# Branching Handlers

For complex scenarios where a single handler class needs to handle multiple different conditions, you can use `BranchingUpdateHandler` to create handlers with multiple entry points (branches). Each branch method is registered as a separate descriptor, sharing the same handler instance.

## Branch Method Signatures

Branch methods support two signatures:
- **Parameterless**: `public async Task<Result> HandleSomething()` or `public void HandleSomething()`
- **With container and cancellation**: `public async Task<Result> HandleSomething(IHandlerContainer<TUpdate> container, CancellationToken cancellation)`

## Basic Example

```csharp
[MessageHandler]
public class ComplexHandler : BranchingMessageHandler
{
    [TextContains("hello")]
    public async Task<Result> HandleGreeting(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello there!", cancellationToken: cancellation);
        return Ok;
    }

    [TextContains("help")]
    public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("How can I help you?", cancellationToken: cancellation);
        return Ok;
    }

    [FromUser("John")]
    public async Task<Result> HandleAdmin(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Admin command received!", cancellationToken: cancellation);
        return Ok;
    }
}
```

## Branching Command Handler

```csharp
[CommandHandler]
public class SettingsHandler : BranchingCommandHandler
{
    [CommandAllias("settings")]
    public async Task<Result> ShowSettings(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Settings menu:", cancellationToken: cancellation);
        return Ok;
    }

    [CommandAllias("settings", "language")]
    public async Task<Result> SetLanguage(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var language = Arguments.FirstOrDefault();
        await Reply($"Language set to: {language}", cancellationToken: cancellation);
        return Ok;
    }

    [CommandAllias("settings", "theme")]
    public async Task<Result> SetTheme(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var theme = Arguments.FirstOrDefault();
        await Reply($"Theme set to: {theme}", cancellationToken: cancellation);
        return Ok;
    }
}
```

## Registration

Branching handlers implement `ICustomDescriptorsProvider`, so each branch is automatically discovered and registered:

- **Source generator**: `CollectHandlers()` correctly includes branching handlers and their branches.
- **Manual registration**: `bot.Handlers.AddHandler<ComplexHandler>()` works as expected.
- **Analyzer**: `DeveloperHelperAnalyzer` (TLG101/102/103) recognizes branching base classes (e.g. `BranchingMessageHandler`) as valid.

## Filtering Rules

1. **Class-level filters** are applied to **all** branches.
2. **Method-level filters** are applied **only** to their respective branch.
3. The final descriptor contains the **union** of class and method filters.

```csharp
[MessageHandler]
[ChatType(ChatType.Private)]  // Applied to all branches
public class FilteredBranchingHandler : BranchingMessageHandler
{
    [TextContains("hello")]     // Applied only to HandleHello
    public async Task<Result> HandleHello(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello!", cancellationToken: cancellation);
        return Ok;
    }

    [TextContains("bye")]       // Applied only to HandleBye
    public async Task<Result> HandleBye(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Goodbye!", cancellationToken: cancellation);
        return Ok;
    }
}
```

## Awaiting in Branches

If a branch method calls awaiting methods (e.g. `container.AwaitMessage()`), add `[MightAwait]` to the handler class. The analyzer auto-injects it during `CollectHandlers()` generation.

```csharp
[MessageHandler]
[MightAwait(UpdateType.Message)]
public class AskBranchingHandler : BranchingMessageHandler
{
    [TextContains("ask")]
    public async Task<Result> HandleAsk(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?", cancellationToken: cancellation);
        var next = await container.AwaitMessage().BySenderId(cancellation);
        await Reply($"Hello, {next.Text}!", cancellationToken: cancellation);
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Multiple Entry Points**: Each method with filters becomes a separate handler entry point.
> 2. **Individual Filtering**: Each method can have its own set of filters and conditions.
> 3. **Shared Context**: All methods share the same handler instance and context.
> 4. **Automatic Registration**: Each method is automatically registered as a separate handler via `ICustomDescriptorsProvider`.
> 5. **Command Arguments**: In `BranchingCommandHandler`, you can access command arguments via the `Arguments` property.
> 6. **Flexible Routing**: Perfect for complex bots with many related commands or message patterns.
