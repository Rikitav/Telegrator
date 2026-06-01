---
title: "Aspects & Cross-Cutting Concerns"
description: "Tap into the AOP system handles logging, validation, etc."
---

# Aspects & Cross-Cutting Concerns

Telegrator provides a powerful aspect-oriented programming (AOP) system for handling cross-cutting concerns like logging, validation, authorization, and error handling. This system allows you to separate business logic from infrastructure concerns.

**Key Concepts:**
- **Pre-Execution Aspects**: Code that runs before handler execution
- **Post-Execution Aspects**: Code that runs after handler execution
- **Self-Processing**: Handler implements interfaces directly
- **Typed Processing**: External processor classes via attributes

**Common Use Cases:**
- Input validation
- Logging and monitoring
- Authorization and access control
- Error handling and recovery
- Performance metrics collection
- Audit trails

## Self-Processing Example

```csharp
using Telegrator.Aspects;

[MessageHandler]
public class LoggingHandler : MessageHandler, IPreProcessor, IPostProcessor
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message processed successfully!");
        return Ok;
    }

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var user = container.HandlingUpdate.Message?.From;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User {user?.Id} ({user?.Username}) sent: {container.HandlingUpdate.Message?.Text}");
        return Ok;
    }

    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Message processing completed");
        return Ok;
    }
}
```

## Typed Processing Example

```csharp
using Telegrator.Aspects;

// Validation processor
public class MessageValidationProcessor : IPreProcessor
{
    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var message = container.HandlingUpdate.Message;
        
        if (message?.Text == null)
            return Result.Fault(); // Stop execution
            
        if (message.Text.Length > 1000)
            return Result.Fault(); // Message too long
            
        return Ok;
    }
}

// Logging processor
public class LoggingProcessor : IPostProcessor
{
    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"Handler execution completed for update {container.HandlingUpdate.Id}");
        return Ok;
    }
}

// Handler with external processors
[MessageHandler]
[BeforeExecution<MessageValidationProcessor>]
[AfterExecution<LoggingProcessor>]
public class ValidatedHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Valid message received and processed!");
        return Ok;
    }
}
```

## Combined Approach Example

```csharp
using Telegrator.Aspects;

[MessageHandler]
[BeforeExecution<AuthorizationProcessor>]
public class SecureHandler : MessageHandler, IPostProcessor
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Secure operation completed!");
        return Ok;
    }

    // Custom post-processing
    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"Secure operation completed for user {container.HandlingUpdate.Message?.From?.Id}");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Self-Processing**: Handlers implement `IPreProcessor` and/or `IPostProcessor` interfaces directly
> 2. **Typed Processing**: External processor classes are applied via `[BeforeExecution<T>]` and `[AfterExecution<T>]` attributes
> 3. **Execution Order**: Pre-execution aspects run first, then handler main logic, then post-execution aspects
> 4. **Flow Control**: Return `Result.Fault()` from pre-execution to stop handler execution
> 5. **Separation of Concerns**: Business logic is separated from cross-cutting concerns like logging and validation
