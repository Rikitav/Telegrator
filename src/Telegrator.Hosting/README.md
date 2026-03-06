# Telegrator.Hosting

**Telegrator.Hosting** is an extension for the Telegrator framework that provides seamless integration with the .NET Generic Host, enabling production-ready, scalable, and maintainable Telegram bot applications.

---

## Features
- Integration with `Microsoft.Extensions.Hosting` (background services, DI, configuration, logging)
- Automatic handler discovery and registration
- Strongly-typed configuration via `appsettings.json` and environment variables
- Graceful startup/shutdown and lifecycle management
- Advanced error handling and logging
- Supports all Telegrator handler/filter/state features

---

## Requirements
- .NET 8.0 or later
- [Telegrator](https://github.com/Rikitav/Telegrator)

---

## Installation

```shell
dotnet add package Telegrator.Hosting
```

---

## Quick Start Example

**Program.cs:**
```csharp
using Telegrator.Hosting;

// Creating builder
TelegramBotHostBuilder builder = TelegramBotHost.CreateBuilder(new TelegramBotHostBuilderSettings()
{
    Args = args,
    ExceptIntersectingCommandAliases = true
});

// Registerring handlers
builder.Handlers.CollectHandlersAssemblyWide();

// Register your services
builder.Services.AddSingleton<IMyService, MyService>();

// Building and running application
TelegramBotHost telegramBot = builder.Build();
telegramBot.SetBotCommands();
telegramBot.Run();
```

---

## Configuration (appsettings.json)

```json
{
  "TelegramBotClientOptions": {
    "Token": "YOUR_BOT_TOKEN"
  },

  "HostOptions": {
    "ShutdownTimeout": 10,
    "BackgroundServiceExceptionBehavior": "StopHost"
  },

  "ReceiverOptions": {
    "DropPendingUpdates": true,
    "Limit": 10
  }
}
```

- `TelegramBotClientOptions`: Bot token and client settings
- `HostOptions`: Host lifecycle and shutdown behavior
- `ReceiverOptions`: Long-polling configuration

---

## Documentation
- [Telegrator Main Docs](https://github.com/Rikitav/Telegrator)
- [Getting Started Guide](https://github.com/Rikitav/Telegrator/wiki/Getting-started)
- [Annotation Overview](https://github.com/Rikitav/Telegrator/wiki/Annotation-overview)

---

## License
GPLv3 