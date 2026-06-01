---
title: "Dependency Injection & Lifetimes"
description: "Understand how handlers and services interact within the DI container."
---

# Dependency Injection & Lifetimes

Telegrator is built for modern .NET applications where Dependency Injection is a first-class citizen. When using `Telegrator.Hosting`, all your handlers are automatically registered in the container.

## Handler Lifetimes

By default, the registration methods in `builder.Handlers` manage lifetimes automatically:

| Handler Type | Default Lifetime | Reason |
| :--- | :--- | :--- |
| **Standard Class Handlers** | `Scoped` | Allows injecting scoped services like EF Core `DbContext`. |
| **Implicit (Static Methods)** | `Singleton` (Keyed) | Static methods don't have instances, proxy is singleton. |
| **Singleton Handlers** | `Singleton` | If manually specified via `HandlerDescriptor`. |

## Injecting Services

You can inject any registered service into your handler's constructor:

```csharp
[CommandHandler]
[CommandAllias("profile")]
public class ProfileHandler : CommandHandler
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileHandler> _logger;

    public ProfileHandler(IUserService userService, ILogger<ProfileHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var userId = container.Update.Message!.From!.Id;
        var user = await _userService.GetByIdAsync(userId);
        
        _logger.LogInformation("Profile requested for user {UserId}", userId);
        
        await Reply($"Hello {user.Name}!");
        return Ok;
    }
}
```

## Scoped Services & EF Core

Telegrator creates a **new scope for every update**. This is critical for database operations:

```csharp
public class MyContext : DbContext { ... }

// Inside handler
public MyHandler(MyContext db) 
{
    // 'db' is unique to this specific update processing
}
```

## Limitations & Best Practices

1. **Avoid Service Locator**: Don't use `IServiceProvider` inside handlers if you can use constructor injection.
2. **Singleton Thread-Safety**: If you register a handler as a `Singleton`, ensure it is thread-safe (no private state variables), as it will process updates from multiple users concurrently.
3. **Circular Dependencies**: Be careful when injecting services that might also need to interact with the bot client, as it can cause circular references in complex setups.
