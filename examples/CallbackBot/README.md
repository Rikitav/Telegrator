# CallbackBot Example

A Telegram bot that demonstrates **inline keyboards** combined with the **awaiting mechanism**. The bot sends a menu with callback buttons and waits inline for the user to press one of them.

## Prerequisites

- .NET 10 SDK
- A Telegram Bot API token (get one from [@BotFather](https://t.me/BotFather))

## Running

1. Open `appsettings.json` and replace `YOUR_BOT_TOKEN_HERE` with your real token.
2. Run the bot:

```bash
dotnet run
```

3. Send `/menu` to your bot to open the inline keyboard.
4. Press any button — the bot will reply with the selected option.

## Structure

| File | Purpose |
|------|---------|
| `Program.cs` | Hosting setup with polling |
| `MenuHandler.cs` | `MessageHandler` that sends an inline keyboard and awaits the callback query |
| `appsettings.json` | Bot token and receiver configuration |

## Key APIs Used

- `[MessageHandler]` — registers the class as a message handler
- `InlineKeyboardMarkup` / `InlineKeyboardButton.WithCallbackData` — builds the inline keyboard
- `AwaitingProvider.AwaitCallbackQuery(HandlingUpdate).BySenderId(cancellation)` — waits for the user's button press
- `MessageHandler.Reply()` — sends messages and edits
