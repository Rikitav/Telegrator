# EchoBot Example

A minimal Telegram bot that echoes every text message back to the sender. This example demonstrates the Hosting setup with `HostApplicationBuilder`, long-polling, and a simple `MessageHandler`.

## Prerequisites

- .NET 10 SDK
- A Telegram Bot API token (get one from [@BotFather](https://t.me/BotFather))

## Running

1. Open `appsettings.json` and replace `YOUR_BOT_TOKEN_HERE` with your real token.
2. Run the bot:

```bash
dotnet run
```

3. Send any text message to your bot — it will reply with `Echo: <your text>`.

## Structure

| File | Purpose |
|------|---------|
| `Program.cs` | Hosting setup: `AddTelegrator()`, `CollectHandlers()`, `WithPolling()`, `UseTelegrator()` |
| `EchoHandler.cs` | `MessageHandler` that replies with the received text |
| `appsettings.json` | Bot token and receiver configuration |

## Key APIs Used

- `[MessageHandler]` — marks the class as a handler for incoming messages
- `MessageHandler.Input` — shortcut to the incoming `Message`
- `MessageHandler.Reply()` — sends a reply to the same chat
- `HostApplicationBuilder` + `AddTelegrator()` + `WithPolling()` — production-ready hosting setup
