# Telegrator.RedisStateStorage

**Telegrator.RedisStateStorage** is an extension for the Telegrator framework that provides Redis powered IStateStorage implementation.

---

## Requirements
- .NET standart 2.1 or later
- [Telegrator](https://github.com/Rikitav/Telegrator)

---

## Installation

```shell
dotnet add package Telegrator.RedisStateStorage
```

---

## Quick Start Example

**Program.cs:**
```csharp
using Telegrator.Hosting;

// Creating builder
TelegramBotHostBuilder builder = TelegramBotHost.CreateBuilder(new HostApplicationBuilderSettings()
{
    Args = args,
    ApplicationName = "TelegramBotHost example",
});

// Registerring handlers
builder.Handlers.CollectHandlersAssemblyWide();

// Register your services and 
builder.Services.AddService<IStateStorage, RedisStateStorage>(services =>
    new RedisStateStorage(ConnectionMultiplexer.Connect("server1:6379, server2:6379")));

// Building and running application
TelegramBotHost telegramBot = builder.Build();
telegramBot.SetBotCommands();
telegramBot.Run();
```

---

## Documentation
- [Telegrator Main Docs](https://github.com/Rikitav/Telegrator)
- [Getting Started Guide](https://github.com/Rikitav/Telegrator/wiki/Getting-started)
- [Annotation Overview](https://github.com/Rikitav/Telegrator/wiki/Annotation-overview)

---

## License
GPLv3 