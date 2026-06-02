---
title: "TelegratorClient & ITelegratorBot"
description: "The main entry point for running a Telegrator bot."
---

# TelegratorClient

`TelegratorClient` is the default implementation of `ITelegratorBot`. It connects to the Telegram Bot API, manages the update router, and provides a simple way to start receiving updates.

## Interface

```csharp
public interface ITelegratorBot
{
    TelegratorOptions Options { get; }
    IHandlersCollection Handlers { get; }
    ITelegramBotInfo BotInfo { get; }
    IUpdateRouter UpdateRouter { get; }

    Task StartReceivingAsync(ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default);
}
```

## Basic Usage

```csharp
var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");

bot.Handlers.CollectHandlers(); // Source-generated discovery
await bot.StartReceivingAsync();
```

## Properties

| Property | Description |
|----------|-------------|
| `Options` | Global `TelegratorOptions` (concurrency, routing flags) |
| `Handlers` | `HandlersCollection` for registering handlers |
| `BotInfo` | Information about the bot user (populated after `GetMe`) |
| `UpdateRouter` | The `IUpdateRouter` that processes incoming updates |

## Starting Reception

`StartReceivingAsync` creates a `DefaultUpdateReceiver` and begins long-polling:

```csharp
await bot.StartReceivingAsync(new ReceiverOptions
{
    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
    DropPendingUpdates = true
});
```

## Hosting Integration

When using `Telegrator.Hosting`, you do not instantiate `TelegratorClient` manually. The hosting package registers it in the DI container and starts it as a `BackgroundService`.

## Testing

For unit and integration tests, use `TestTelegratorClient` from the `Telegrator.Testing` package instead of the real client. It mocks `ITelegramBotClient` and allows you to push synthetic updates directly into the router.
