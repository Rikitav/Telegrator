---
title: "Result"
description: "The immutable value type that controls routing and aspect execution."
---

# Result

`Result` is a `readonly record struct` that represents the outcome of a handler, pre-processor, or post-processor. It tells the `UpdateRouter` whether to stop routing, continue to the next handler, or chain to a specific handler type.

## Immutability

`Result` is immutable. Once created, its properties cannot be changed:

```csharp
public readonly record struct Result
{
    public bool Success { get; init; }
    public bool RouteNext { get; init; }
    public Type? NextType { get; init; }
}
```

Because it is a **value type**, returning `Result.Ok()`, `Result.Fault()`, or `Result.Next()` does not allocate on the heap.

## Factory Methods

### `Result.Ok()`
Indicates success. The router stops describing handlers unless chained.

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    await Reply("Done!");
    return Result.Ok();
}
```

### `Result.Fault()`
Indicates failure. Inside a pre-processor, it prevents the handler main block and post-processors from running.

```csharp
public override async Task<Result> BeforeExecution(IHandlerContainer container, CancellationToken cancellation)
{
    if (!IsAuthorized(container))
        return Result.Fault(); // Handler will NOT execute

    return Result.Ok();
}
```

### `Result.Next()`
Tells the router to continue describing and executing subsequent handlers.

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    await Reply("Passing through...");
    return Result.Next();
}
```

### `Result.Next<THandler>()`
Tells the router to continue, but only execute handlers of type `THandler`.

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    await Reply("Delegating to admin handler...");
    return Result.Next<AdminCommandHandler>();
}
```

## Property Reference

<table>
<thead>
<tr><th>Property</th><th>Type</th><th>Meaning</th></tr>
</thead>
<tbody>
<tr><td>`Success`</td><td>`bool`</td><td>`true` for `Ok` and `Next`; `false` for `Fault`</td></tr>
<tr><td>`RouteNext`</td><td>`bool`</td><td>`true` if the router should continue describing handlers</td></tr>
<tr><td>`NextType`</td><td>`Type?`</td><td>When set, restricts continued routing to handlers of this exact type</td></tr>
</tbody>
</table>

## Common Patterns

### Short-circuit on failure
```csharp
Result? preResult = await aspects.ExecutePre(this, container, cancellation);
if (preResult is not { Success: true } || preResult is { RouteNext: true })
    return preResult;
```

### Conditional chaining
```csharp
if (container.Update.Message!.Text == "/admin")
    return Result.Next<AdminHandler>();

return Result.Ok();
```
