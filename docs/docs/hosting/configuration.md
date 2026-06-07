---
title: "Configuration & Logging"
description: "Complete guide to configuring Telegrator.Hosting via appsettings.json, code, and environment variables."
---

# Hosting Configuration

When using `Telegrator.Hosting`, the framework respects your application's `appsettings.json` and all standard .NET configuration providers (environment variables, command-line arguments, Azure Key Vault, etc.).

This guide covers every configuration option, every way to set it, and common pitfalls.

---

## Configuration Sections

The Hosting layer reads from two main configuration sections:

| Options Type | Primary Section | Fallback Section | Used By |
|--------------|-----------------|------------------|---------|
| `TelegratorOptions` | `"Telegrator"` | `"TelegratorOptions"` | Core framework (token, routing, limits) |
| `ReceiverOptions` | `"Receiver"` | `"ReceiverOptions"` | Long-polling receiver (offset, limit, allowed updates) |

If a section is missing and you haven't registered the options manually, the framework throws a `MissingMemberException` at startup.

---

## `TelegratorOptions` Reference

This is the **most important** configuration object. It controls the bot token, retry behavior, routing, and concurrency.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Token` | `string` | *(required)* | Your Telegram Bot API token. Example: `"123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"` |
| `BaseUrl` | `string?` | `null` | Custom Bot API server URL. Use this if you run a local Bot API server instead of `https://api.telegram.org`. |
| `UseTestEnvironment` | `bool` | `false` | If `true`, the bot connects to the Telegram test environment instead of the production API. |
| `RetryThreshold` | `int` | `60` | Seconds to wait before retrying a failed request to Telegram. |
| `RetryCount` | `int` | `3` | Maximum number of retries for failed requests. |
| `MaximumParallelWorkingHandlers` | `int?` | `null` | Global concurrency limit for handler execution. If set, a `SemaphoreSlim` limits how many handlers run simultaneously. `null` means unlimited. |
| `ExclusiveAwaitingHandlerRouting` | `bool` | `false` | If `true`, when an update matches an awaiting handler, no regular handlers are executed for that update. |
| `ExceptIntersectingCommandAliases` | `bool` | `true` | If `true`, when multiple command handlers match the same message, only the highest-importance one runs. |

### `appsettings.json` Example

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    "BaseUrl": null,
    "UseTestEnvironment": false,
    "RetryThreshold": 60,
    "RetryCount": 3,
    "MaximumParallelWorkingHandlers": 20,
    "ExclusiveAwaitingHandlerRouting": true,
    "ExceptIntersectingCommandAliases": true
  }
}
```

Or using the fallback section name:

```json
{
  "TelegratorOptions": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    "MaximumParallelWorkingHandlers": 20
  }
}
```

---

## `ReceiverOptions` Reference

These options only apply when using **long-polling** (`.WithPolling()`).

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Offset` | `int?` | `null` | Identifier of the first update to be returned. `null` means start from the latest. |
| `Limit` | `int?` | `null` | Limits the number of updates to be retrieved (1–100). `null` uses Telegram's default. |
| `DropPendingUpdates` | `bool?` | `null` | If `true`, drops all pending updates on startup. |
| `AllowedUpdates` | `UpdateType[]?` | `null` | Array of update types to receive. `null` means receive all update types except `ChatMember`. |

### `appsettings.json` Example

```json
{
  "Receiver": {
    "Offset": 0,
    "Limit": 100,
    "DropPendingUpdates": true,
    "AllowedUpdates": [
      "Message",
      "CallbackQuery",
      "InlineQuery"
    ]
  }
}
```

Or using the fallback section name:

```json
{
  "ReceiverOptions": {
    "DropPendingUpdates": true,
    "AllowedUpdates": [ "Message", "CallbackQuery" ]
  }
}
```

> [!TIP]
> Limiting `AllowedUpdates` reduces server load and improves startup time, because Telegram won't send irrelevant updates.

---

## Configuration Methods

### 1. `appsettings.json` (Recommended)

The simplest and most maintainable approach. Call `AddTelegrator()` and `WithPolling()` without explicit options:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator()   // Reads "Telegrator" section automatically
    .WithPolling()        // Reads "Receiver" section automatically
    .Handlers.CollectHandlers();
```

The framework binds configuration sections automatically if the options are not already registered in DI.

### 2. Inline Objects in `Program.cs`

Pass options directly to `AddTelegrator`:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator(
    options: new TelegratorOptions
    {
        Token = "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
        MaximumParallelWorkingHandlers = 10,
        ExclusiveAwaitingHandlerRouting = true
    })
    .WithPolling()
    .Handlers.CollectHandlers();
```

For receiver options, use `ConfigureReceiver`:

```csharp
builder.Services.ConfigureReceiver(new ReceiverOptions
{
    DropPendingUpdates = true,
    Limit = 100,
    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery]
});

builder.AddTelegrator()
    .WithPolling()
    .Handlers.CollectHandlers();
```

> [!IMPORTANT]
> If you pass an explicit `TelegratorOptions` object to `AddTelegrator`, the framework **does not** read `appsettings.json` for `TelegratorOptions`. It only falls back to configuration when the parameter is `null`.

### 3. `ConfigureTelegrator` for `IHostBuilder`

When using the classic `IHostBuilder` API (e.g. `Host.CreateDefaultBuilder`), use `ConfigureTelegrator`:

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureTelegrator(
        options: new TelegratorOptions { Token = "..." },
        action: builder =>
        {
            builder.WithPolling();
            builder.Handlers.CollectHandlers();
        })
    .Build();

host.UseTelegrator();
await host.RunAsync();
```

### 4. Environment Variables

Use double-underscore (`__`) as the section separator. This works on all platforms.

```bash
# Linux / macOS / Docker
export Telegrator__Token="123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
export Telegrator__MaximumParallelWorkingHandlers="20"
export Telegrator__ExclusiveAwaitingHandlerRouting="true"
export Receiver__DropPendingUpdates="true"
export Receiver__Limit="100"
```

In a `docker-compose.yml`:

```yaml
services:
  bot:
    environment:
      - Telegrator__Token=${BOT_TOKEN}
      - Telegrator__MaximumParallelWorkingHandlers=20
      - Receiver__DropPendingUpdates=true
```

In Kubernetes:

```yaml
env:
  - name: Telegrator__Token
    valueFrom:
      secretKeyRef:
        name: bot-secrets
        key: token
```

### 5. User Secrets (Development)

Store sensitive values like the bot token in user secrets during development:

```bash
dotnet user-secrets init
dotnet user-secrets set "Telegrator:Token" "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
dotnet user-secrets set "Telegrator:MaximumParallelWorkingHandlers" "20"
```

> [!TIP]
> User secrets take precedence over `appsettings.json` but are overridden by environment variables. This is the standard .NET Configuration precedence.

---

## Configuration Precedence

.NET Configuration uses the following precedence (last wins):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables
5. Command-Line Arguments

This means you can set safe defaults in `appsettings.json` and override sensitive values via environment variables or command-line args in production.

---

## Logging Integration

Telegrator uses the standard `ILogger` abstraction. All internal events (startup, handler execution, errors) are logged to the configured providers.

### Filtering Log Levels

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Telegrator": "Debug",
      "Telegrator.Hosting.Web": "Information"
    }
  }
}
```

### Custom Logger Adapters

If you are not using Generic Host but still want Telegrator logs in your own system:

```csharp
TelegratorLogging.AddAdapter(new MyCustomLoggerAdapter());
```

---

## Validation & Common Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `MissingMemberException`: Token is required. | `TelegratorOptions.Token` is missing from all sources. | Add `"Token"` to the `"Telegrator"` section or pass it inline. |
| `MissingMemberException`: Auto configuration enabled, yet no options of type 'ReceiverOptions'... | `Receiver`/`ReceiverOptions` section missing and `.WithPolling()` called without pre-registered options. | Add the section to `appsettings.json` or call `ConfigureReceiver`. |
| `MissingMemberException`: Auto configuration enabled, yet no options of type 'TelegratorOptions'... | `Telegrator`/`TelegratorOptions` section missing and `AddTelegrator()` called without explicit options. | Add the section or pass options inline. |
| Handlers not executing | `MaximumParallelWorkingHandlers` is too low and all slots are occupied. | Increase the limit or set to `null` for unlimited. |

---

## Standalone `TelegratorClient` (No Hosting)

If you are not using `Telegrator.Hosting`, configuration is manual:

```csharp
var options = new TelegratorOptions
{
    Token = "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    MaximumParallelWorkingHandlers = 10
};

var bot = new TelegratorClient(options.Token, options);
bot.Handlers.CollectHandlers();
bot.StartReceiving();
```

> [!NOTE]
> `IOptions<T>`, `appsettings.json` binding, and `ILogger` integration are only available through `Telegrator.Hosting`. In standalone mode, pass options directly to constructors.
