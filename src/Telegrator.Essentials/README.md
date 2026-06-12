# Telegrator.Essentials

A companion library for Telegrator that provides commonly-requested filters, aspects, and extension methods that are intentionally kept out of the core library. It is fully optional and targets `netstandard2.0` so it can be used from any Telegrator-supported project. Sort of a playground for Telegrator related features that i dont want to put in Core library.

## What is included

- **Filter annotations** ready to be placed on handlers
- **Pre- and post-processors** for cross-cutting concerns
- **Extension methods** for strings, updates, and handler containers
- **Continuous chat actions** to keep the "typing" indicator alive

## Installation

```shell
dotnet add package Telegrator.Essentials
```

## Filter Annotations

### `WelcomeAttribute`

Matches `/start` commands sent in private chats.

```csharp
[MessageHandler]
[Welcome]
public class StartHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome!", cancellationToken: cancellation);
        return Ok;
    }
}
```

### `PrefixedTriggerWordAttribute`

Matches messages that start with a configured prefix and an optional trigger word. Useful for custom command prefixes such as `!`, `.`, or `/`.

```csharp
[MessageHandler]
[PrefixedTriggerWord(Prefixes = ["/", "!"], Words = ["help", "h"], IgnoreCase = true)]
public class HelpHandler : MessageHandler
{
    // handles /help, /h, !help, !h
}
```

### `CallbackDataFilterAttribute`

Matches callback queries by exact data or prefix.

```csharp
[CallbackQueryHandler]
[CallbackDataFilter("confirm")]
public class ConfirmHandler : CallbackQueryHandler { }

[CallbackQueryHandler]
[CallbackDataFilter("page:", MatchPrefix = true)]
public class PaginationHandler : CallbackQueryHandler { }
```

### `PreventDoubleSubmit`

Ignores duplicate callback queries from the same user within a timeout window.

```csharp
[CallbackQueryHandler]
[PreventDoubleSubmit(TimeoutSeconds = 3)]
public class VoteHandler : CallbackQueryHandler { }
```

## Aspects

### `RateLimitPreprocessor`

A pre-processor that enforces a per-user sliding-window rate limit. State is stored in `IStateStorage`, so the limit is shared across handlers and survives restarts when persistent storage is used.

```csharp
[MessageHandler]
[BeforeExecution(typeof(MyRateLimit))]
public class HeavyHandler : MessageHandler { }

public class MyRateLimit : RateLimitPreprocessor
{
    public override int MaxRequests => 5;
    public override int WindowSeconds => 30;
}
```

### `AutoDeletePostProcessor`

A post-processor that automatically deletes a bot message after a delay. Schedule it from inside the handler:

```csharp
[MessageHandler]
[AfterExecution(typeof(AutoDeletePostProcessor))]
public class TempMessageHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var sent = await Reply("This message will self-destruct in 5 seconds.", cancellationToken: cancellation);
        container.ScheduleAutoDelete(sent, TimeSpan.FromSeconds(5));
        return Ok;
    }
}
```

## Extension Methods

### Update helpers

```csharp
long? userId = update.GetUserId();
long? chatId = update.GetChatId();
bool isPrivate = update.IsPrivateChat();
```

### String helpers

```csharp
"hello world".Truncate(10);           // "hello worl…"
"a_b".EscapeMarkdownV2();             // "a\\_b"
"<b>".EscapeHtml();                   // "&lt;b&gt;"
```

## Continuous Action

Keep the "typing" status (or any other chat action) alive while a long-running handler executes:

```csharp
[MessageHandler]
public class SlowHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        using var typing = container.StartTypingAction(cancellation);
        await Task.Delay(TimeSpan.FromSeconds(5), cancellation);
        await Reply("Done!", cancellationToken: cancellation);
        return Ok;
    }
}
```

## Dependency Injection

Current Essentials components are attribute-driven and do not require DI registration. The extension method is provided for future services:

```csharp
builder.Services.AddTelegratorEssentials();
```

## License

MIT — same as the rest of the Telegrator project.
