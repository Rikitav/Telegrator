---
title: "Dependency Injection"
description: "Integrate Telegrator directly with DI containers."
---

# Dependency Injection (DI)

Telegrator is designed to work seamlessly with DI containers (e.g., ASP.NET Core). Handlers and their dependencies are automatically registered.

## Lifetime Mappings

When using `Telegrator.Hosting`, handlers are registered with specific lifetimes based on their `DescriptorType`:

- **General & Keyed**: Registered as **Scoped**. A new scope is created for each handler execution.
- **Singleton**: Registered as **Singleton**.
- **Implicit (Methods)**: Registered as **Keyed Singleton**.

## Keyed Services Support
Telegrator supports .NET 8+ Keyed Services for handlers:

```csharp
// The descriptor will be registered with a service key
builder.Handlers.AddDescriptor(new HandlerDescriptor(typeof(MyHandler), "my_key"));
```

## Example
[MessageHandler]
public class MyHandler : MessageHandler
{
    private readonly IMyService _myService;
    private readonly ILogger<MyHandler> _logger;

    public MyHandler(IMyService myService, ILogger<MyHandler> logger)
    {
        _myService = myService;
        _logger = logger;
    }

    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        _logger.LogInformation("MyHandler executed!");
        var result = _myService.DoSomething();
        await Reply(result);
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Constructor Injection**: Dependencies (`IMyService`, `ILogger`) are automatically injected by the DI container.
> 2. **Service Registration**: When using `Telegrator.Hosting`, services are automatically registered with the DI container.
> 3. **Handler Instantiation**: Telegrator creates handler instances through the DI container, resolving all dependencies.
> 4. **Logging Integration**: Built-in logging support allows for comprehensive debugging and monitoring.
