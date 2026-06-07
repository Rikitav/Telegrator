# AwaitBot Example

A Telegram bot that demonstrates the **awaiting mechanism** for building multi-step conversational wizards. The bot waits inline for the user's next messages without leaving the current handler execution.

## Prerequisites

- .NET 10 SDK
- A Telegram Bot API token (get one from [@BotFather](https://t.me/BotFather))

## Running

1. Open `appsettings.json` and replace `YOUR_BOT_TOKEN_HERE` with your real token.
2. Run the bot:

```bash
dotnet run
```

3. Send `/start` to your bot to launch the wizard:
   - Bot asks for your name
   - Bot asks for your age
   - Bot summarizes the collected data

## Structure

| File | Purpose |
|------|---------|
| `Program.cs` | Hosting setup with polling |
| `WizardHandler.cs` | `MessageHandler` that runs a 3-step wizard using `AwaitMessage` |
| `appsettings.json` | Bot token and receiver configuration |

## Key APIs Used

- `[MessageHandler]` — registers the class as a message handler
- `AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation)` — pauses execution and waits for the next message from the same user
- `MessageHandler.Reply()` — sends a reply to the current chat
