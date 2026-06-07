---
title: "Deployment & Hosting Options"
description: "Learn to deploy in Console, ASP.NET Core, or Webhooks."
---

# Integration & Deployment

Telegrator works in console, hosted applications, and ASP.NET Core (webhook) projects.

## Console Applications

Simple console bot setup:

```csharp
class Program
{
    static void Main(string[] args)
    {
        var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
        
        // Recommended for Native AOT
        bot.Handlers.CollectHandlers(); 
        
        bot.StartReceiving();
        Console.ReadLine();
    }
}
```

> [!TIP]
> `CollectHandlersDomainWide()` is now obsolete. Use the generated `CollectHandlers()` method for better performance and Native AOT compatibility.

## ASP.NET Core Hosting

Telegrator provides seamless integration with .NET's generic host through the `Telegrator.Hosting` package, making it easy to build production-ready bot applications.

Host your bot in ASP.NET Core applications:

```csharp
using Telegrator.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Telegrator with standard setup
builder.ConfigureTelegrator(action: b => {
    b.Handlers.CollectHandlers();
    b.WithPolling(); // Use polling for this example 
});

var app = builder.Build();

app.MapFallback(() => "Bot is running...");

// 2. Initialize Telegrator
app.UseTelegrator();

app.Run();
```

> **How is it working?**
> 1. **Generic Host Integration**: `TelegramBotHost` implements `IHost` and integrates with .NET's generic host.
> 2. **Lifecycle Management**: The host manages the bot's startup, shutdown, and graceful termination.
> 3. **Dependency Injection**: All handlers and services are automatically registered with the DI container.
> 4. **Configuration**: Supports standard .NET configuration patterns (appsettings.json, environment variables, etc.).
> 5. **Logging**: Integrates with .NET's logging infrastructure for comprehensive monitoring.
> 6. **Health Checks**: Can be integrated with .NET's health check system for production monitoring.


## Webhook Deployment

Deploy your bot using webhooks for production using the `Telegrator.Hosting.Web` package:

```csharp
using Telegrator.Hosting.Web;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Telegrator with Web integration
builder.ConfigureTelegrator(action: b => {
    // 2. Register handlers using source generator
    b.Handlers.CollectHandlers();
    b.WithWeb();
});

// 3. Configure services
builder.Services.AddSingleton<IMyService, MyService>();

var app = builder.Build();

// 4. Initialize Telegrator (maps webhook endpoint)
app.UseTelegrator();

await app.RunAsync();
```

## Native AOT Publishing

Telegrator is designed to work with .NET's Native AOT publishing. This allows you to create high-performance, small-footprint binaries that don't require the .NET runtime to be installed on the target machine.

### Key requirements for AOT:
1. **Source Generated Handlers**: Use `CollectHandlers()` instead of reflection-based methods.
2. **JSON Serialization**: Use `System.Text.Json` source generator for any custom objects you store in state.
3. **No Dynamic Code**: Telegrator's core avoids `Reflection.Emit` and other APIs that are incompatible with trimming.

To publish your bot as Native AOT:
```shell
dotnet publish -c Release -r linux-x64 --self-contained
```
> 2. **Security**: Supports secret token validation for secure webhook handling.
> 3. **Scalability**: Webhook hosting is more efficient for high-traffic bots compared to long-polling.
> 4. **Production Ready**: Includes health checks, logging, and monitoring capabilities.
> 5. **SSL Required**: Webhook hosting requires HTTPS for production use.
