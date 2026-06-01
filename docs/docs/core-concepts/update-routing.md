---
title: "Update Routing"
description: "The central component that manages how updates flow through your bot."
---

# Update Routing

The `UpdateRouter` is the central component that manages how updates flow through your bot.

## How Updates Are Processed
1. **Reception**: Updates are received from Telegram via long-polling or webhook
2. **Filtering**: Each registered handler is checked against the update using its filters
3. **Selection**: Handlers that pass all filters are selected for execution
4. **Prioritization**: Selected handlers are sorted by Importance and Priority
5. **Execution**: Handlers are executed in order, with aspects applied

## Router Configuration
```csharp
var options = new TelegratorOptions
{
    MaximumParallelWorkingHandlers = 10,
    ExclusiveAwaitingHandlerRouting = true,
    ExceptIntersectingCommandAliases = true
};

var bot = new TelegratorClient("<BOT_TOKEN>", options);
```

## Error Handling
The router includes built-in error handling:
- **Exception Handler**: Global exception handler for all errors
- **Handler Errors**: Individual handler errors are caught and logged
- **Recovery**: The router continues processing other handlers even if one fails

## Performance Considerations
- **UpdateHandlersPool**: All handlers are queued into a `Channel`-based pool for asynchronous execution.
- **Concurrency Limits**: `TelegratorOptions.MaximumParallelWorkingHandlers` controls the global execution limit (via `SemaphoreSlim`).
- **Resource Management**: Each scope is automatically disposed when the handler completes (`OnLifetimeEnded`).
