# Telegrator.RedisStateStorage

**Telegrator.RedisStateStorage** is an extension for the Telegrator framework that provides a Redis-powered `IStateStorage` implementation.

---

## Requirements
- .NET Standard 2.1 or later
- [Telegrator](https://github.com/Rikitav/Telegrator)
- [StackExchange.Redis](https://www.nuget.org/packages/StackExchange.Redis/)

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
using Telegrator.RedisStateStorage;
using StackExchange.Redis;

// Creating builder
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Registering handlers
builder.AddTelegrator()
    .WithPolling()
    .Handlers.CollectHandlers();

// Register Redis state storage
builder.Services.AddSingleton<IStateStorage, RedisStateStorage>(services =>
    new RedisStateStorage(ConnectionMultiplexer.Connect("server1:6379,server2:6379")));

// Building and running application
var host = builder.Build();
host.Run();
```

---

## Documentation
- [Telegrator Main Docs](https://github.com/Rikitav/Telegrator)
- [Getting Started Guide](https://github.com/Rikitav/Telegrator/wiki/Getting-started)
- [Annotation Overview](https://github.com/Rikitav/Telegrator/wiki/Annotation-overview)

---

## License
MIT
