---
title: "Importance & Priority"
description: "Understand the execution order mechanisms in Telegrator."
---

# Handler Importance & Priority

Telegrator provides two different mechanisms for controlling handler execution order: `Importance` and `Priority`. These serve different purposes in the framework's execution model.

## Importance (Internal Type Priority)
`Importance` is an internal parameter used to control priority between different handler types that process the same update type. It's automatically set by the framework based on the handler type.

**Built-in Importance Values:**
- `CommandHandler`: Importance = 1 (highest priority for message updates)
- `MessageHandler`: Importance = 0 (default priority for message updates)
- `CallbackQueryHandler`: Importance = 0 (default for callback updates)
- `AnyUpdateHandler`: Importance = -1 (lowest priority, catches all updates)

**Example of Importance in Action:**
```csharp
[CommandHandler] // Importance = 1 (automatically set)
[CommandAllias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Command handler executed first!");
        return Ok;
    }
}

[MessageHandler] // Importance = 0 (automatically set)
[TextContains("hello")]
public class HelloHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message handler executed second!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Automatic Importance**: The framework automatically sets importance based on handler type.
> 2. **Command Priority**: Commands are processed before regular messages due to higher importance.
> 3. **Type-based Ordering**: This ensures critical handlers (like commands) run before general handlers.
> 4. **Framework Control**: Importance is managed internally and shouldn't be manually overridden.

## Priority (Global Execution Control)
`Priority` is a user-controlled parameter that regulates the execution order among all registered handlers in the application, regardless of their type.

**Priority Usage:**
```csharp
[MessageHandler(Priority = 10)] // High priority among all handlers
public class HighPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with high priority!");
        return Ok;
    }
}

[MessageHandler(Priority = 0)] // Default priority
public class NormalPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with normal priority!");
        return Ok;
    }
}

[MessageHandler(Priority = -10)] // Low priority
public class LowPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with low priority!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Global Priority**: `Priority` controls execution order across all handler types.
> 2. **Higher Priority First**: Handlers with higher priority numbers run before those with lower numbers.
> 3. **User Control**: Priority is manually set by developers for custom execution ordering.
> 4. **Default Priority**: When not specified, handlers have priority 0.
> 5. **Cross-Type Ordering**: Priority works across different handler types (MessageHandler, CommandHandler, etc.).

## Combined Execution Order
The final execution order is determined by both Importance and Priority:

1. **First**: Handlers are sorted by `Importance` (type-based priority)
2. **Second**: Within the same importance level, handlers are sorted by `Priority` (user-defined priority)

**Example:**
```csharp
[CommandHandler(Priority = 5)] // Importance = 1, Priority = 5
[CommandAllias("admin")]
public class AdminCommandHandler : CommandHandler
{
    // Executes first (highest importance + high priority)
}

[CommandHandler(Priority = 0)] // Importance = 1, Priority = 0  
[CommandAllias("start")]
public class StartCommandHandler : CommandHandler
{
    // Executes second (highest importance + normal priority)
}

[MessageHandler(Priority = 10)] // Importance = 0, Priority = 10
[TextContains("urgent")]
public class UrgentMessageHandler : MessageHandler
{
    // Executes third (normal importance + high priority)
}

[MessageHandler(Priority = 0)] // Importance = 0, Priority = 0
[TextContains("hello")]
public class HelloMessageHandler : MessageHandler
{
    // Executes last (normal importance + normal priority)
}
```
