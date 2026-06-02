---
title: "UpdateHandlerBase"
description: "The base class for all Telegram update handlers."
---

# UpdateHandlerBase

All handlers in Telegrator derive from `UpdateHandlerBase`. It provides the execution lifecycle, container creation, aspect integration, and convenient reply methods.

## Lifecycle

When the router selects a handler, it calls `Execute(DescribedHandlerDescriptor, CancellationToken)`, which performs the following steps:

1. **Container creation** — builds an `IHandlerContainer` with update data, client, and context.
2. **Pre-processing** — runs `IPreProcessor.BeforeExecution` if aspects are configured.
3. **Handler execution** — calls the abstract `ExecuteInternal` method implemented by the subclass.
4. **Post-processing** — runs `IPostProcessor.AfterExecution` if aspects are configured.
5. **Lifetime cleanup** — marks the handler's `LifetimeToken` as ended.

## Abstract Method

```csharp
protected abstract Task<Result> ExecuteInternal(
    IHandlerContainer container,
    CancellationToken cancellationToken);
```

Implement this in your handler to define the actual behavior.

## Convenient Properties

Inside any handler method you have access to:

| Property | Description |
|----------|-------------|
| `HandlingUpdate` | The raw `Update` object |
| `Client` | The `ITelegramBotClient` instance |
| `Input` | Shortcut to `HandlingUpdate.Message` (for message handlers) |
| `StateStorage` | The `IStateStorage` for state machines |
| `AwaitingProvider` | The `IAwaitingProvider` for awaiting user input |
| `Ok` | Shorthand for `Result.Ok()` |
| `Fault` | Shorthand for `Result.Fault()` |
| `Next` | Shorthand for `Result.Next()` |

## Reply Helpers

```csharp
await Reply("Hello!");
await Reply("Hello!", replyMarkup: keyboard);
await AnswerCallbackQuery("Done!");
```

## Example

```csharp
[MessageHandler]
[TextContains("hello")]
public class HelloHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"Hello, {container.Update.Message!.From!.FirstName}!");
        return Ok;
    }
}
```

## Filters Fallback

If a handler's filters do not match, the router calls `FiltersFallback` instead of `ExecuteInternal`. By default this returns `Result.Ok()`, but you can override it to send a "command not recognized" message:

```csharp
public override async Task<Result> FiltersFallback(
    FiltersFallbackReport report,
    ITelegramBotClient client,
    CancellationToken cancellationToken)
{
    await client.SendTextMessageAsync(report.ChatId, "Unknown command.");
    return Result.Fault();
}
```
