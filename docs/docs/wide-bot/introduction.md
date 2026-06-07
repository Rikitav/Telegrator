# WideBot (MTProto) Integration

The `Telegrator.Hosting.WideBot` package brings the power of **MTProto** (the protocol used by official Telegram apps) to your Telegrator bot. While the standard Bot API is limited, MTProto allows you to do almost everything a real user can do.

## Why use MTProto?
- Access to **raw TL layers**.
- Work with groups/channels without bot-specific restrictions.
- Advanced file management (uploading large files, custom chunks).
- Listening to events that are not exposed via Bot API.

## Installation

```shell
dotnet add package Telegrator.Hosting.WideBot
```

## Basic Setup

WideBot requires `api_id` and `api_hash` from [my.telegram.org](https://my.telegram.org).

```csharp
var builder = Host.CreateApplicationBuilder(args);

string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string database = Path.Combine(appData, "Telegrator", "wtgb.db");

builder.AddTelegrator() // 1. Add Telegrator core
    .WithWide(provider => new SqliteConnection($"Data Source={database}")) // 2. Add MTProto receiver
    .Handlers.CollectHandlers(); // 3. Register handlers using source generator

var app = builder.Build();

// 4. Initialize Telegrator
app.UseTelegrator();
await app.RunAsync();
```

> [!NOTE]
> If you don't pass explicit options, `.WithWide()` automatically reads the `"WideBotOptions"` section from `appsettings.json`. See the [Configuration](configuration.md) guide for every option and every way to set it.

### Minimal `appsettings.json`

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
  },
  "WideBotOptions": {
    "ApiId": 123456,
    "ApiHash": "YOUR_API_HASH_FROM_MY_TELEGRAM_ORG",
    "DropPendingUpdates": true
  }
}
```

> [!CAUTION]
> **Obsolete Method**: `AddWideTelegrator()` is now replaced by the `.WithWide()` extension in the unified fluent registration.

## Session Management
WideBot uses `WTelegramBot` under the hood. It persists a session file (usually `session.dat`) to handle authentication. Ensure your application has write access to the folder where the session file is stored.

## Limitations
- **MTProto complexity**: MTProto uses a strictly typed system of TL (Type Language) objects.
- **Flood Waits**: MTProto has more aggressive flood-limit protection than the Bot API.
- **Beta status**: This extension is advanced and requires understanding of how MTProto works.
