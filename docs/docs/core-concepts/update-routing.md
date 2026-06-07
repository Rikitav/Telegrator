---
title: "Update Routing"
description: "The central component that manages how updates flow through your bot."
---

# Update Routing

The `UpdateRouter` is the central component that manages how updates flow through your bot.

## How Updates Are Processed
1. **Reception**: Updates are received from Telegram via long-polling or webhook
2. **Routing**: `ConsumeUpdateAsync` schedules the update for background processing (non-blocking) or `HandleUpdateAsync` processes it synchronously
3. **Filtering**: Each registered handler is checked against the update using its filters
4. **Selection**: Handlers that pass all filters are selected for execution
5. **Prioritization**: Selected handlers are sorted by Importance and Priority
6. **Execution**: Handlers are executed in order, with aspects applied

## `HandleUpdateAsync` vs `ConsumeUpdateAsync`

`IUpdateRouter` exposes two ways to process an incoming update:

- **`HandleUpdateAsync`**
  - Behavior: Fully blocking. Awaits the entire handler pipeline before returning.
  - Use Case: Tests, manual control, custom receivers
- **`ConsumeUpdateAsync`**
  - Behavior: Fire-and-forget. Schedules the update on `Task.Run` and returns `Task.CompletedTask` immediately.
  - Use Case: Production receivers (polling, webhooks, WideBot)

All built-in receivers (`DefaultUpdateReceiver`, `HostedUpdateWebhooker`, `WideUpdateReceiver`) use `ConsumeUpdateAsync` so that:
- **Long-polling** doesn't block the receive loop while handlers run.
- **Webhooks** return HTTP 200 to Telegram immediately, preventing retries.
- **WideBot** keeps the MTProto update stream flowing.

If you implement a custom receiver, use `ConsumeUpdateAsync` for production scenarios and `HandleUpdateAsync` only when you need deterministic awaiting (e.g. integration tests).

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
- **UpdateHandlersPool**: All handlers are queued into an **unbounded** `Channel<DescribedHandlerDescriptor>` for asynchronous execution. The channel itself does not block the producer (receivers), so webhook requests and polling loops never stall.
- **Concurrency Limits**: `TelegratorOptions.MaximumParallelWorkingHandlers` controls the global execution limit via a `SemaphoreSlim`. Backpressure is applied at the execution stage, not at the enqueue stage.
- **Resource Management**: Each scope is automatically disposed when the handler completes (`OnLifetimeEnded`).
