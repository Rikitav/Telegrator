---
title: "Getting Started with Hosting"
description: "Integrate Telegrator with .NET Generic Host for background processing."
---

# Getting Started with Telegrator.Hosting

The `Telegrator.Hosting` package is the standard way to run your bot as a background service in .NET applications. It provides full integration with the .NET Generic Host, allowing you to use standard Dependency Injection, Logging, and Configuration patterns.

## Installation

```shell
dotnet add package Telegrator.Hosting
```

## Basic Setup

In your `Program.cs`, use the extension methods to add Telegrator to your host builder:

```csharp
using Telegrator.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureReceiver(new ReceiverOptions() { ... });

builder.AddTelegrator(options: new TelegratorOptions() { ... }) // 1. Add Telegrator with Polling (Long-polling)
    .WithPolling() // 2. Add update receiving method
    .Handlers.CollectHandlers(); // 3. Register handlers using source generator

var host = builder.Build();

// 4. Initialize Telegrator
host.UseTelegrator();

await host.RunAsync();
```

### IHostBuilder Support

Telegrator also supports the classic `IHostBuilder` API (e.g. when using `Host.CreateDefaultBuilder` or ASP.NET Core `WebApplicationBuilder`):

```csharp
var host = Host.CreateDefaultBuilder(args)
    .ConfigureTelegrator(options: new TelegratorOptions { ... }, action: builder =>
    {
        builder.WithPolling();
        builder.Handlers.CollectHandlers();
    })
    .Build();

host.UseTelegrator();
await host.RunAsync();
```

> [!CAUTION]
> **Obsolete Method**: `CollectHandlersAssemblyWide()` and other reflection-based discovery methods are now obsolete. Please use the source-generated `CollectHandlers()` method, which is required for Native AOT and better performance.

## How It Works

`Telegrator.Hosting` implements a `BackgroundService` that:
1. **Starts Receiving**: Opens a long-polling connection to Telegram via `TelegratorClient`.
2. **Manages Scopes**: Creates a new DI scope for every incoming update, ensuring that scoped services (like Database Contexts) are cleaned up correctly.
3. **Graceful Shutdown**: Automatically stops receiving and processes remaining updates before the application exits.

## Advantages
- **Unified Logging**: Uses `ILogger` for all internal framework logs.
- **Service Lifetimes**: Handlers can inject services with `Scoped`, `Transient`, or `Singleton` lifetimes.
- **Easy Deployment**: Works perfectly with Docker, Windows Services, and Systemd.

> [!NOTE]
> For web applications where you prefer Webhooks over long-polling, use the `Telegrator.Hosting.Web` package instead.
