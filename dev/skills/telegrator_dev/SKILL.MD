---
name: telegrator_dev
description: Guide for coding agents building Telegram bot host applications with the Telegrator framework consumed via NuGet. Use when creating, modifying, or debugging bots built on Telegrator, Telegrator.Hosting, Telegrator.Hosting.Web, Telegrator.Hosting.WideBot, or related packages.
---

# Telegrator Bot Development

This skill covers building and maintaining Telegram bot host applications with the Telegrator framework when it is consumed as a NuGet package.

## When to Use

Use this skill when the task involves:

- Creating a new Telegram bot project
- Writing or modifying handlers, filters, aspects, or state machines
- Configuring hosting (long-polling, webhooks, MTProto/WideBot)
- Working with `IStateStorage`, `IAwaitingProvider`, `IUpdateRouter`, or `IUpdateHandlersPool`
- Adding `Telegrator.OpenTelemetry`, `Telegrator.Essentials`, `Telegrator.Localized`, or `Telegrator.RedisStateStorage`
- Writing tests for Telegrator-based code

## Quick Start

A minimal bot using Generic Host and long-polling:

```shell
dotnet add package Telegrator.Hosting
```

```csharp
using Microsoft.Extensions.Hosting;
using Telegrator.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

ITelegramBotHostBuilder tg = builder.AddTelegrator();
tg.Handlers.CollectHandlers();
tg.WithPolling();

IHost host = builder.Build();
host.UseTelegrator();
host.Run();
```

The bot token is read from configuration:

```json
{
  "Telegrator": {
    "Token": "YOUR_BOT_TOKEN"
  }
}
```

## Hosting Modes

Choose exactly one receiving mode per host:

| Mode | Package | Extension |
|------|---------|-----------|
| Long-polling | `Telegrator.Hosting` | `.WithPolling()` |
| Webhooks | `Telegrator.Hosting.Web` | `.WithWeb()` |
| MTProto / user-bot | `Telegrator.Hosting.WideBot` | `.WithWide()` |

Do not combine receiving modes in the same service collection.

### Webhooks

```shell
dotnet add package Telegrator.Hosting.Web
```

```csharp
using Microsoft.AspNetCore.Builder;
using Telegrator.Hosting;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddTelegrator()
    .WithWeb()
    .Handlers.CollectHandlers();

WebApplication app = builder.Build();
app.UseTelegrator();
app.Run();
```

Configure `WebhookerOptions` in `appsettings.json`:

```json
{
  "WebhookerOptions": {
    "WebhookUri": "https://your-host/webhook",
    "SecretToken": "your-secret",
    "DropPendingUpdates": true
  }
}
```

## Handlers

Handlers are discovered automatically when marked with the matching attribute. Inherit from the concrete base class and override `Execute`:

```csharp
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;

[MessageHandler]
public class EchoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input?.Text is not { } text)
            return Ok;

        await Reply($"Echo: {text}", cancellationToken: cancellation);
        return Ok;
    }
}
```

Common handler base classes:

- `Commandhandler` → `[CommandHandler]`
- `MessageHandler` → `[MessageHandler]`
- `CallbackQueryHandler` → `[CallbackQueryHandler]`
- `InlineQueryHandler` → `[InlineQueryHandler]`
- `ChosenInlineResultHandler` → `[ChosenInlineResultHandler]`

Useful members inside a handler:

- `Input` — strongly-typed update
- `Container` — full execution context
- `AwaitingProvider` — wait for follow-up updates
- `HandlingUpdate` — raw Telegram update
- `Reply(...)`, `EditMessage(...)` — helper send/edit methods

## Filters and Attributes

Apply attributes on handler classes to restrict when they run. Examples:

```csharp
[MessageHandler]
[Welcome]
public class StartHandler : MessageHandler { }

[CommandHandler]
[CommandAllias("help", "h")]
public class HelpHandler : CommandHandler { }
```

If a handler calls an awaiting method such as `AwaitMessage`, decorate it with `[MightAwait(UpdateType.Message)]` to satisfy the framework analyzer.

## Awaiting Updates

Pause execution and wait for the user's next message inline:

```csharp
await Reply("What is your name?", cancellationToken: cancellation);
Message? next = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
await Reply($"Hello, {next?.Text}!", cancellationToken: cancellation);
return Ok;
```

## State Management

Use `IStateStorage` to persist state across updates. The default implementation is in-memory. For production, add Redis:

```shell
dotnet add package Telegrator.RedisStateStorage
```

```csharp
builder.Services.AddStateStorage<RedisStateStorage>();
```

For simple enum-based state machines:

```csharp
public enum WizardState { Start, AskName, AskAge, Complete }

await StateStorage.GetStateMachine<WizardState>(HandlingUpdate)
    .BySenderId()
    .Advance(cancellation);
```

## Aspects

Use `[BeforeExecution(typeof(T))]` and `[AfterExecution(typeof(T))]` for cross-cutting logic such as rate limiting or cleanup:

```csharp
[MessageHandler]
[BeforeExecution(typeof(MyRateLimit))]
public class HeavyHandler : MessageHandler { }
```

Implement `IPreProcessor` or `IPostProcessor` in `T`.

## Optional Packages

### Telegrator.Essentials

```shell
dotnet add package Telegrator.Essentials
```

Provides ready-to-use filters (`WelcomeAttribute`, `PrefixedTriggerWordAttribute`, `CallbackDataFilterAttribute`, `PreventDoubleSubmit`), rate limiting aspects, auto-delete post-processor, and string/update extensions.

```csharp
builder.Services.AddTelegratorEssentials();
```

### Telegrator.OpenTelemetry

```shell
dotnet add package Telegrator.OpenTelemetry
```

Adds `ActivitySource`/`Meter` instrumentation. Register after `AddTelegrator()`:

```csharp
builder.Services.AddTelegratorOpenTelemetry();
```

Use source name `Telegrator` and meter name `Telegrator` when configuring exporters.

### Telegrator.Localized

Use for multi-language bots with resolvers and templating.

## Configuration

Configuration sections used by the framework:

- `Telegrator:Token` — bot token
- `Telegrator:ExceptIntersectingCommandAliases` — command alias validation
- `Receiver:AllowedUpdates` — update types to receive
- `WebhookerOptions:*` — webhook settings

## Testing

For integration tests use the `Telegrator.Testing` package and replace the receiving mode with `WithTestServer()`.

For unit tests, mock `IHandlerContainer` or create `FilterExecutionContext<T>` manually.

## Common Anti-Patterns

- Do not combine `.WithPolling()`, `.WithWeb()`, or `.WithWide()` in one host
- Do not block threads in `Execute`; use async/await
- Do not store non-serializable objects when persistent storage is configured
- Do not forget `[MightAwait]` on handlers that use awaiting methods
