---
title: "UpdateRouter & IUpdateRouter"
description: "The central mediator that routes incoming Telegram updates to the appropriate handlers."
---

# UpdateRouter

`UpdateRouter` is the heart of Telegrator. Every incoming `Update` from Telegram passes through the router, which determines which handlers should execute, in what order, and whether routing should continue or stop.

## Interface

```csharp
public interface IUpdateRouter
{
    TelegratorOptions Options { get; }
    ITelegramBotInfo BotInfo { get; }
    IHandlersProvider HandlersProvider { get; }
    IAwaitingProvider AwaitingProvider { get; }
    IStateStorage StateStorage { get; }
    IUpdateHandlersPool HandlersPool { get; }
    IRouterExceptionHandler? ExceptionHandler { get; set; }
    IHandlerContainerFactory? DefaultContainerFactory { get; set; }

    /// <summary>
    /// Fully awaits the handler pipeline. Blocking call.
    /// </summary>
    Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);

    /// <summary>
    /// Schedules the update for background processing without blocking the caller.
    /// </summary>
    Task ConsumeUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken);
}
```

## Routing Pipeline

When `HandleUpdateAsync` is called, the router performs the following steps:

1. **Awaiting handlers first** — handlers created by `AwaitingProvider` (e.g., waiting for a user's reply) are checked and executed with **exclusive routing** if configured.
2. **Regular handlers** — registered handlers from `HandlersProvider` are filtered, sorted by priority, and enqueued into `HandlersPool`.
3. **Result evaluation** — after each handler completes, its `Result` is inspected:
   - `Result.Ok()` → stop routing (default)
   - `Result.Next()` → continue to the next handler
   - `Result.Next<T>()` → continue, but only for handlers of type `T`
   - `Result.Fault()` → stop routing immediately

## Configuration

Control routing behavior via `TelegratorOptions`:

<table>
<thead>
<tr><th>Option</th><th>Effect</th></tr>
</thead>
<tbody>
<tr><td>`MaximumParallelWorkingHandlers`</td><td>Global concurrency limit for handler execution</td></tr>
<tr><td>`ExclusiveAwaitingHandlerRouting`</td><td>If `true`, only awaiting handlers are considered for an update</td></tr>
<tr><td>`ExceptIntersectingCommandAliases`</td><td>Prevents multiple command handlers from matching the same message</td></tr>
</tbody>
</table>

## OpenTelemetry Spans

The router automatically creates an `Activity` named `HandleUpdate` with the following tags:

- `update.id`
- `update.type`

Enable OpenTelemetry in your application to collect distributed traces:

```csharp
services.AddOpenTelemetry()
    .WithTracing(builder => builder.AddSource("Telegrator.UpdateRouter"));
```

## Custom Router

You can subclass `UpdateRouter` to override routing logic:

```csharp
public class CustomRouter : UpdateRouter
{
    public CustomRouter(IHandlersProvider handlersProvider, IAwaitingProvider awaitingProvider,
                        IStateStorage stateStorage, TelegratorOptions options, ITelegramBotInfo botInfo)
        : base(handlersProvider, awaitingProvider, stateStorage, options, botInfo) { }

    protected override async IAsyncEnumerable<DescribedHandlerDescriptor> DescribeDescriptors(
        IServiceProvider provider, HandlerDescriptorList descriptors, ITelegramBotClient client,
        Update update, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Custom descriptor discovery logic
    }
}
```
