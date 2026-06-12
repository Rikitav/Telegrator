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

Create a console or ASP.NET Core project and add the hosting package:

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

Put the bot token in `appsettings.json`:

```json
{
  "Telegrator": {
    "Token": "YOUR_BOT_TOKEN"
  },
  "Receiver": {
    "AllowedUpdates": ["Message"]
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

## Handler Template

Handlers are discovered automatically when marked with the matching attribute. Inherit from the concrete base class and override `Execute`. Constructor injection works because the hosting provider resolves handlers from DI.

```csharp
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;

[MessageHandler]
public class EchoHandler : MessageHandler
{
    private readonly ILogger<EchoHandler> _logger;

    public EchoHandler(ILogger<EchoHandler> logger)
    {
        _logger = logger;
    }

    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input?.Text is not { } text)
            return Ok;

        _logger.LogInformation("Received: {Text}", text);
        await Reply($"Echo: {text}", cancellationToken: cancellation);
        return Ok;
    }
}
```

Common handler base classes:

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

Apply attributes on handler classes to restrict when they run:

```csharp
[MessageHandler]
[Welcome]
public class StartHandler : MessageHandler { }

[MessageHandler]
[CommandAllias("help", "h")]
public class HelpHandler : MessageHandler { }
```

If a handler calls an awaiting method such as `AwaitMessage`, decorate it with `[MightAwait(UpdateType.Message)]` to satisfy the framework analyzer.

## Awaiting Updates

Pause execution and wait for the user's next message inline:

```csharp
[MessageHandler]
[MightAwait(UpdateType.Message)]
public class AskNameHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?", cancellationToken: cancellation);
        Message? next = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
        await Reply($"Hello, {next?.Text}!", cancellationToken: cancellation);
        return Ok;
    }
}
```

## State Management

Use `IStateStorage` to persist state across updates. The default implementation is in-memory, so state is lost when the process restarts. For production, add Redis:

```shell
dotnet add package Telegrator.RedisStateStorage
```

```csharp
builder.Services.AddStateStorage<RedisStateStorage>();
```

Route handlers by state using `StateAttribute<TKey, TValue>`:

```csharp
public enum WizardState { Start, AskName, AskAge, Complete }

[MessageHandler]
[StateAttribute<BySenderIdResolver, WizardState>(WizardState.AskName)]
public class AskNameStepHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?", cancellationToken: cancellation);
        return Ok;
    }
}
```

Advance the state machine from another handler:

```csharp
await StateStorage
    .GetStateMachine<WizardState>(HandlingUpdate)
    .BySenderId()
    .Advance(cancellation);
```

## Aspects

Use `[BeforeExecution(typeof(T))]` and `[AfterExecution(typeof(T))]` for cross-cutting logic such as rate limiting or cleanup.

Aspects are instantiated with `Activator.CreateInstance`, so they do not support constructor injection. Pass configuration through properties or use the handler container inside `BeforeExecution`/`AfterExecution`.

```csharp
[MessageHandler]
[BeforeExecution(typeof(LogPreProcessor))]
public class HeavyHandler : MessageHandler { }
```

```csharp
using Telegrator.Aspects;
using Telegrator.Core.Handlers;

public class LogPreProcessor : IPreProcessor
{
    public Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling update {container.HandlingUpdate.Id}");
        return Task.FromResult(Result.Ok());
    }
}
```

## Optional Packages

### Telegrator.Essentials

```shell
dotnet add package Telegrator.Essentials
```

Provides ready-to-use filters (`WelcomeAttribute`, `PrefixedTriggerWordAttribute`, `CallbackDataFilterAttribute`, `PreventDoubleSubmit`), rate limiting aspects, auto-delete post-processor, and string/update extensions. Prefer these over custom implementations for common tasks.

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

All environment-specific values must live in `appsettings.json` (or environment variables / user secrets). Do not hardcode tokens, connection strings, or webhook URIs in handlers or `Program.cs`.

Bind strongly-typed options with `IOptions<T>`:

```csharp
public class BotOptions
{
    public string AdminUserId { get; set; } = string.Empty;
}

builder.Services.Configure<BotOptions>(builder.Configuration.GetSection("Bot"));
```

```csharp
public class AdminHandler : MessageHandler
{
    private readonly BotOptions _options;

    public AdminHandler(IOptions<BotOptions> options)
    {
        _options = options.Value;
    }
}
```

Configuration sections used by the framework:

- `Telegrator:Token` — bot token
- `Telegrator:ExceptIntersectingCommandAliases` — command alias validation
- `Receiver:AllowedUpdates` — update types to receive
- `WebhookerOptions:*` — webhook settings

## Testing

For integration tests use the `Telegrator.Testing` package and replace the receiving mode with `WithTestServer()`.

For unit tests, mock `IHandlerContainer` or create `FilterExecutionContext<T>` manually.

## Troubleshooting

When something does not work, check in this order:

- Handler is not triggered:
  - Verify the handler class has the correct `[*Handler]` attribute.
  - Verify `CollectHandlers()` is called in `Program.cs`.
  - Verify the update type is included in `Receiver:AllowedUpdates`.
  - Verify no filter attribute rejects the update.

- Webhooks do not work:
  - Verify `WebhookerOptions:WebhookUri` is a publicly reachable HTTPS URL.
  - Verify `WebhookerOptions:SecretToken` matches what Telegram sends (or leave empty for local tests).
  - Verify no other receiving mode is registered.

- State is lost after restart:
  - The default `IStateStorage` is in-memory. Add `Telegrator.RedisStateStorage` and call `AddStateStorage<RedisStateStorage>()`.

- Analyzer warning TLG201:
  - Add `[MightAwait(UpdateType.*)]` to any handler that uses `Await*` methods.

## Common Anti-Patterns

- Do not combine `.WithPolling()`, `.WithWeb()`, or `.WithWide()` in one host.
- Do not write manual `switch` routing on `Update.Type`; use handler base classes and attributes.
- Do not block threads in `Execute`; use async/await.
- Do not store non-serializable objects when persistent storage is configured.
- Do not forget `[MightAwait]` on handlers that use awaiting methods.
- Do not implement custom debounce, rate limiting, or welcome filtering when `Telegrator.Essentials` already provides them.
- Do not hardcode configuration in handlers or `Program.cs`; use `IOptions<T>` and `appsettings.json`.
