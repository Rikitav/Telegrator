---
title: "Performance Optimization"
description: "Optimize your bot for high performance."
---

# Performance Optimization

Optimize your bot for high performance:

## Concurrency Settings
```csharp
var options = new TelegratorOptions
{
    MaximumParallelWorkingHandlers = 20,  // Adjust based on server capabilities
    ExclusiveAwaitingHandlerRouting = true,
    ExceptIntersectingCommandAliases = true
};
```

## Memory Management
- Use `using` statements for disposable resources
- Implement proper cleanup in custom aspects
- Monitor memory usage in long-running bots

## Caching Strategies
```csharp
public class CachingAspect : IPreProcessor
{
    private readonly Dictionary<string, object> _cache = new();

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var key = container.HandlingUpdate.Message?.Text;
        if (_cache.TryGetValue(key, out var cached))
        {
            // Use cached result
            return Ok;
        }
        return Ok;
    }
}
```

## Quick Tips
- **Use webhooks** for production bots
- **Implement caching** for frequently accessed data
- **Limit concurrent executions** based on server capabilities
- **Use async/await** properly throughout your code
- **Monitor memory usage** in long-running bots
- **Implement proper error handling** to prevent crashes

## Fire-and-Forget Update Processing

All production receivers use `ConsumeUpdateAsync` instead of `HandleUpdateAsync`:
- **Polling**: `DefaultUpdateReceiver` and `WideUpdateReceiver` call `ConsumeUpdateAsync` so the receive loop never blocks on handler execution.
- **Webhooks**: `HostedUpdateWebhooker` returns HTTP 200 immediately while handlers run in the background.

This ensures that slow handlers (e.g. AI inference, database queries) do not stall the update ingestion pipeline. Concurrency is controlled separately by `MaximumParallelWorkingHandlers`.
