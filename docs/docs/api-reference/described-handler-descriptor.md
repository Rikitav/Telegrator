---
title: "DescribedHandlerDescriptor"
description: "The execution context that travels with a handler through the routing pipeline."
---

# DescribedHandlerDescriptor

`DescribedHandlerDescriptor` is the runtime representation of a matched handler. It carries all the context needed for execution: the handler instance, filters, update data, client, and a `TaskCompletionSource` used to await the handler's result.

## Construction

Descriptors are created by `UpdateRouter.DescribeHandler` after a handler's filters pass. You do not typically construct them manually.

## Key Properties

<table>
<thead>
<tr><th>Property</th><th>Type</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td>`From`</td><td>`HandlerDescriptor`</td><td>The static metadata descriptor</td></tr>
<tr><td>`UpdateRouter`</td><td>`IUpdateRouter`</td><td>The router that created this descriptor</td></tr>
<tr><td>`AwaitingProvider`</td><td>`IAwaitingProvider`</td><td>Provider for temporary awaiting handlers</td></tr>
<tr><td>`StateStorage`</td><td>`IStateStorage`</td><td>State persistence service</td></tr>
<tr><td>`Client`</td><td>`ITelegramBotClient`</td><td>The Telegram bot client</td></tr>
<tr><td>`HandlerInstance`</td><td>`UpdateHandlerBase`</td><td>The actual handler instance to execute</td></tr>
<tr><td>`ExtraData`</td><td>`Dictionary<string, object>`</td><td>Additional context data from filters</td></tr>
<tr><td>`CompletedFilters`</td><td>`CompletedFiltersList`</td><td>Filters that have already been evaluated</td></tr>
<tr><td>`HandlingUpdate`</td><td>`Update`</td><td>The incoming Telegram update</td></tr>
<tr><td>`DisplayString`</td><td>`string`</td><td>Human-readable name for logging</td></tr>
<tr><td>`Result`</td><td>`Result?`</td><td>The execution result (set after completion)</td></tr>
</tbody>
</table>

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
