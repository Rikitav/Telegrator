---
title: "Redis Setup"
description: "Connecting Telegrator to Redis for persistent state management."
---

# Redis State Storage Setup

Persistent state management is key for production bots. While `InMemoryStateStorage` is easy to use, `Telegrator.RedisStateStorage` ensures that your user's progress is never lost during deployments or crashes.

## Installation

```shell
dotnet add package Telegrator.RedisStateStorage
```

## Basic Configuration

The easiest way to configure Redis is by using the `StackExchange.Redis` library.

```csharp
using Telegrator.Hosting;
using Telegrator.States;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// 1. Setup Redis Connection
var redis = ConnectionMultiplexer.Connect("localhost:6379");
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);

// 2. Add Telegrator as usual
builder.AddTelegrator();

// 3. Register Redis as the State Storage
builder.Services.AddStateStorage<RedisStateStorage>();

var host = builder.Build();
host.UseTelegrator();
await host.RunAsync();
```

## Key Configuration (Optional)
You can configure how keys are stored in Redis via `RedisStateOptions`:

```csharp
builder.Services.Configure<RedisStateOptions>(options => {
    options.KeyPrefix = "mybot_"; // Prefix for all Redis keys
    options.DefaultTtl = TimeSpan.FromDays(7); // Auto-expire unused states
});
```

## Advantages
- **Persistence**: Survives server restarts.
- **Horizontal Scaling**: Multiple bot instances can share the same Redis server to handle load.
- **Performance**: Redis is extremely fast for simple key-value reads/writes.

## Limitations
- **Dependency**: Requires a running Redis instance.
- **Serialization**: Objects must be JSON serializable.
