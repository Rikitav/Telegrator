# WebhookBot Example

A minimal Telegram bot that receives updates via **webhooks** instead of long-polling. Built on ASP.NET Core using `Telegrator.Hosting.Web`.

## Prerequisites

- .NET 10 SDK
- A Telegram Bot API token (get one from [@BotFather](https://t.me/BotFather))
- A publicly accessible HTTPS URL (use ngrok for local development)

## Running

1. Open `appsettings.json` and configure:
   - `Telegrator:Token` — your bot token
   - `WebhookerOptions:WebhookUri` — your public HTTPS URL with `/webhook` path
   - `WebhookerOptions:SecretToken` — a random secret for Telegram to validate requests

2. Run the bot:

```bash
dotnet run
```

3. The bot will automatically register the webhook with Telegram on startup.

## Local Development with ngrok

```bash
ngrok http 5000
```

Then copy the HTTPS URL into `WebhookerOptions:WebhookUri`:

```json
"WebhookUri": "https://abc123.ngrok.io/webhook"
```

## Structure

| File | Purpose |
|------|---------|
| `Program.cs` | ASP.NET Core setup: `AddTelegrator()`, `WithWeb()`, `UseTelegrator()` |
| `EchoHandler.cs` | Simple `MessageHandler` that echoes text messages |
| `appsettings.json` | Bot token and webhook configuration |

## Key APIs Used

- `WebApplication.CreateBuilder(args)` — ASP.NET Core host builder
- `AddTelegrator().WithWeb()` — configures webhook receiving mode
- `app.UseTelegrator()` — initializes Telegrator middleware
- `[MessageHandler]` — registers the echo handler
