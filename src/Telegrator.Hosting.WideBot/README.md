# Telegrator.Hosting.WideBot

Extension for the Telegrator framework that integrates with **WTelegramBot**, enabling advanced bot capabilities such as MTProto proxy support, SQL command detection, and extended Telegram API access.

---

## ✨ Features

- Integration with `WTelegramBot` library (v9.6.0+)
- `TelegratorWClient` — extends `WTelegramBotClient` with Telegrator's mediator and handler system
- MTProto proxy support via `MTProxy` option
- SQL command detection configuration (`SqlCommands`)
- Full compatibility with Telegrator handlers, filters, state, and awaiting mechanisms
- Seamless integration with `Telegrator.Hosting`

---

## Requirements

- .NET 10.0 or later
- [Telegrator.Hosting](https://github.com/Rikitav/Telegrator)
- [WTelegramBot](https://www.nuget.org/packages/WTelegramBot/) (9.6.0+)

---

## Installation

```shell
dotnet add package Telegrator.Hosting.WideBot
```

---

## Quick Start

### Program.cs

```csharp
using Telegrator.Hosting;
using Telegrator.Hosting.WideBot;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator()
    .WithWideBot()
    .Handlers.CollectHandlers();

var host = builder.Build();
host.Run();
```

### Configuration (appsettings.json)

```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN"
  },

  "WideBotOptions": {
    "ApiId": 123456,
    "ApiHash": "your_api_hash",
    "MTProxy": "https://t.me/proxy?server=...",
    "DropPendingUpdates": true
  }
}
```

### WideBotOptions

| Property | Type | Description |
|----------|------|-------------|
| `ApiId` | `int` | Telegram API ID (required) |
| `ApiHash` | `string` | Telegram API Hash (required) |
| `MTProxy` | `string?` | Optional MTProto proxy URL |
| `DropPendingUpdates` | `bool` | Whether to discard pending updates on startup |

---

## Handler Access to WTelegram Features

Handlers running inside a WideBot host can access WTelegram-specific functionality through extension methods on `AbstractUpdateHandler<TUpdate>`.

---

## Related Packages

| Package | Purpose |
|---------|---------|
| `Telegrator` | Core framework |
| `Telegrator.Hosting` | .NET Generic Host integration |
| `WTelegramBot` | Underlying WTelegramBot client library |

---

## License

MIT
