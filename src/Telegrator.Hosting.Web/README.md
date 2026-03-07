# Telegrator.Hosting.Web

**Telegrator.Hosting.Web** is an extension for the Telegrator framework that enables seamless integration with ASP.NET Core and webhook-based Telegram bots. It is designed for scalable, production-ready web applications.

---

## Features
- ASP.NET Core integration for webhook-based bots
- Automatic handler discovery and registration
- Strongly-typed configuration via `appsettings.json` and environment variables
- Dependency injection and middleware support
- Graceful startup/shutdown and lifecycle management
- Advanced error handling and logging
- Supports all Telegrator handler/filter/state features

---

## Requirements
- .NET 10.0 or later
- ASP.NET Core
- [Telegrator.Hosting](https://github.com/Rikitav/Telegrator)

---

## Installation

```shell
dotnet add package Telegrator.Hosting.Web
```

---

## Quick Start Example

**Program.cs (ASP.NET Core):**
```csharp
using Telegrator.Hosting;
using Telegrator.Hosting.Web;

// Creating builder
TelegramBotWebHostBuilder builder = TelegramBotWebHost.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ApplicationName = "TelegramBotWebHost example",
});

// Register handlers
builder.Handlers.CollectHandlersAssemblyWide();

// Register your services
builder.Services.AddSingleton<IMyService, MyService>();

// Building and running application
TelegramBotWebHost telegramBot = builder.Build();
telegramBot.SetBotCommands();
telegramBot.Run();
```

---

## Application integration Example

**Program.cs (ASP.NET Core):**
```csharp
using Telegrator.Hosting;
using Telegrator.Hosting.Web;

// Creating builder
WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
    ApplicationName = "WebApplication example",
});

// Adding Telegrator
builder.AddTelegratorWeb();

// Register handlers
builder.Handlers.CollectHandlersAssemblyWide();

// Register your services
builder.Services.AddSingleton<IMyService, MyService>();

// Building and running application
builder.Build()
    .UseTelegratorWeb()
    .SetBotCommands();
    .Run();
```

---

## Configuration (appsettings.json)

```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN",
    "ExceptIntersectingCommandAliases": true
  }

  "WebhookerOptioons": {
    "WebhookUri" = "https://you-public-host.ru/bot",
    "SecretToken": "MEDIC_GAMING"
    "DropPendingUpdates": true
  }
}
```

---

## Documentation
- [Telegrator Main Repository](https://github.com/Rikitav/Telegrator)
- [Getting Started Guide](https://github.com/Rikitav/Telegrator/wiki/Getting-started)
- [Annotation Overview](https://github.com/Rikitav/Telegrator/wiki/Annotation-overview)

---

## License
GPLv3 