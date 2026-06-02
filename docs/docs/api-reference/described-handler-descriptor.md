---
title: "DescribedHandlerDescriptor"
description: "The execution context that travels with a handler through the routing pipeline."
---

# DescribedHandlerDescriptor

`DescribedHandlerDescriptor` is the runtime representation of a matched handler. It carries all the context needed for execution: the handler instance, filters, update data, client, and a `TaskCompletionSource` used to await the handler's result.

## Construction

Descriptors are created by `UpdateRouter.DescribeHandler` after a handler's filters pass. You do not typically construct them manually.

## Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `From` | `HandlerDescriptor` | The static metadata descriptor |
| `UpdateRouter` | `IUpdateRouter` | The router that created this descriptor |
| `AwaitingProvider` | `IAwaitingProvider` | Provider for temporary awaiting handlers |
| `StateStorage` | `IStateStorage` | State persistence service |
| `Client` | `ITelegramBotClient` | The Telegram bot client |
| `HandlerInstance` | `UpdateHandlerBase` | The actual handler instance to execute |
| `ExtraData` | `Dictionary<string, object>` | Additional context data from filters |
| `CompletedFilters` | `CompletedFiltersList` | Filters that have already been evaluated |
| `HandlingUpdate` | `Update` | The incoming Telegram update |
| `DisplayString` | `string` | Human-readable name for logging |
| `Result` | `Result?` | The execution result (set after completion) |

## Awaiting the Result

```csharp
public async Task AwaitResult(CancellationToken cancellationToken)
```

Blocks until the handler completes or the cancellation token is triggered. Internally uses a `TaskCompletionSource`.

## Reporting the Result

```csharp
public void ReportResult(Result? result)
```

Called by `UpdateHandlersPool.ProcessHandler` after the handler finishes. Sets the result and completes the awaiting task.

> [!CAUTION]
> Calling `ReportResult` twice throws `InvalidOperationException`.

## Example: Custom Pool

If you implement a custom `IUpdateHandlersPool`, you will work directly with descriptors:

```csharp
public override async Task Enqueue(params IEnumerable<DescribedHandlerDescriptor> handlers)
{
    foreach (var descriptor in handlers)
    {
        // Custom scheduling logic
        _ = Task.Run(async () =>
        {
            await descriptor.HandlerInstance.Execute(descriptor);
            descriptor.ReportResult(Result.Ok());
        });
    }
}
```
