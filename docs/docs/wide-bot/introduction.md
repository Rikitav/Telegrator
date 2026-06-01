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

// 1. Configure credentials
builder.Services.ConfigureWideBot(new WideBotOptions(
    token: "BOT_TOKEN",
    apiId: 123456, 
    apiHash: "YOUR_HASH"
));

string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string database = Path.Combine(appData, "wtgb.db");

builder.AddTelegrator() // 2. Add Telegrator
    .WithWide(provider => new SqliteConnection($"Data Source={database}")) // 3. Add update receiving method
    .Handlers.CollectHandlers(); // 4. Register handlers using source generator

var app = builder.Build();

// 5. Initialize Telegrator
app.UseTelegrator()
   .Run();
```

> [!CAUTION]
> **Obsolete Method**: `AddWideTelegrator()` is now replaced by the `.WithWide()` extension in the unified fluent registration.

## Session Management
WideBot uses `WTelegramBot` under the hood. It persists a session file (usually `session.dat`) to handle authentication. Ensure your application has write access to the folder where the session file is stored.

## Limitations
- **MTProto complexity**: MTProto uses a strictly typed system of TL (Type Language) objects.
- **Flood Waits**: MTProto has more aggressive flood-limit protection than the Bot API.
- **Beta status**: This extension is advanced and requires understanding of how MTProto works.