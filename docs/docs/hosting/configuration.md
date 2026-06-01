---
title: "Configuration & Logging"
description: "Using standard .NET configuration providers and logging with Telegrator."
---

# Configuration & Logging

When using `Telegrator.Hosting`, the framework respects your application's `appsettings.json` and logging configuration.

## TelegratorOptions

The main configuration object is `TelegratorOptions`. You can bind it directly from your configuration provider:

```json
{
  "Telegrator": {
    "Token": "12345:ABCDE...",
    "MaximumParallelWorkingHandlers": 10,
    "ExceptIntersectingCommandAliases": true
  }
}
```

And in `Program.cs`:
```csharp
builder.AddTelegrator(
    options: new TelegratorOptions() { ... },
    action: builder => builder.CollectHandlers()
);
```

### Key Options
- `Token`: Your Telegram Bot Token.
- `MaximumParallelWorkingHandlers`: Concurrency limit for processing updates.
- `ExceptIntersectingCommandAliases`: If true, only the highest importance handler will run when multiple command aliases match.

## Logging Integration

Telegrator uses the standard `ILogger` abstraction. All internal events (Start, Stop, Errors, Handler Execution) are logged to the configured providers (Console, Debug, Sentry, etc.).

### Controlling Log Level
You can filter Telegrator-specific logs in your `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Telegrator": "Debug" 
    }
  }
}
```

## Custom Logger Adapters
If you are NOT using Generic Host but still want Telegrator logs in your own system, you can use `TelegratorLogging`:

```csharp
TelegratorLogging.AddAdapter(new MyCustomLoggerAdapter());
```

## Limitations
- **Hosting-Only**: Many configuration features (like `IOptions<T>`) require the Generic Host environment. In a standalone `TelegratorClient` setup, you must pass options manually.
