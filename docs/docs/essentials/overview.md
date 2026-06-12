---
title: "Essentials Overview"
description: "Optional filters, aspects, and utilities that extend Telegrator without bloating the core library."
---

# Telegrator.Essentials

`Telegrator.Essentials` is an optional companion package that contains helpers many bots need, but which are not required in the core framework. It targets `netstandard2.0` and can be added to any Telegrator project.

## Installation

```shell
dotnet add package Telegrator.Essentials
```

## Available Components

The library is organized into three groups:

- **Annotations** — reusable `FilterAnnotation<T>` implementations
- **Aspects** — pre- and post-processors for cross-cutting behavior
- **Extensions** — convenience methods for strings, updates, and handler containers

## Filter Annotations

### WelcomeAttribute

Responds to `/start` in private chats.

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

### PrefixedTriggerWordAttribute

Matches messages that start with a prefix and an optional word. This is useful when you want command prefixes other than `/` or when you need aliases.

```csharp
[MessageHandler]
[PrefixedTriggerWord(Prefixes = ["/", "!"], Words = ["help", "h"])]
public class HelpHandler : MessageHandler { }
```

Properties:

- `Prefixes` — array of prefixes to accept
- `Words` — optional array of exact words after the prefix
- `IgnoreCase` — default `true`
- `MatchWholeWord` — default `true`, prevents `/startnow` from matching `/start`

### CallbackDataFilterAttribute

Matches callback queries by data.

```csharp
[CallbackQueryHandler]
[CallbackDataFilter("confirm")]
public class ConfirmHandler : CallbackQueryHandler { }

[CallbackQueryHandler]
[CallbackDataFilter("page:", MatchPrefix = true)]
public class PaginationHandler : CallbackQueryHandler { }
```

### PreventDoubleSubmit

Protects callback handlers from rapid double-taps.

```csharp
[CallbackQueryHandler]
[PreventDoubleSubmit(TimeoutSeconds = 3)]
public class VoteHandler : CallbackQueryHandler { }
```

## Aspects

### RateLimitPreprocessor

Enforces a sliding-window rate limit per user. The counter is stored in `IStateStorage`, so it works across handlers and can be persisted with Redis.

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

Default policy is 10 requests per 60 seconds.

### AutoDeletePostProcessor

Deletes a bot message after a configurable delay.

```csharp
[MessageHandler]
[AfterExecution(typeof(AutoDeletePostProcessor))]
public class TempHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var sent = await Reply("Loading…", cancellationToken: cancellation);
        container.ScheduleAutoDelete(sent, TimeSpan.FromSeconds(3));
        return Ok;
    }
}
```

## Continuous Chat Actions

Keep the "typing" indicator alive while a handler performs slow work.

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

`StartContinuousAction` is also available for other `ChatAction` values such as `UploadPhoto` or `FindLocation`.

## Dependency Injection

All current components are attribute-driven and do not require explicit DI registration. However, the extension method is available for future services:

```csharp
builder.Services.AddTelegratorEssentials();
```
