---
title: "HandlerBuilderBase & IHandlerBuilder"
description: "Configure filters, state keepers, and indexing for handlers."
---

# HandlerBuilderBase

`HandlerBuilderBase` provides the foundation for configuring handler descriptors before they are registered. It is used both by implicit awaiter builders (`AwaiterHandlerBuilder<TUpdate>`) and explicit regular handlers (`HandlerBuilder<TUpdate>`).

## Interface

```csharp
public interface IHandlerBuilder
{
    IHandlerBuilder SetUpdateValidating(UpdateValidateAction validateAction);
    IHandlerBuilder SetConcurreny(int concurrency);
    IHandlerBuilder SetPriority(int priority);
    IHandlerBuilder SetIndexer(int concurrency, int priority);
    IHandlerBuilder AddFilter(IFilter<Update> filter);
    IHandlerBuilder AddFilters(params IFilter<Update>[] filters);
    IHandlerBuilder SetState<TKey, TValue>(TValue? state)
        where TKey : IStateKeyResolver, new()
        where TValue : IEquatable<TValue>;
    IHandlerBuilder AddTargetedFilter<TFilterTarget>(...);
    IHandlerBuilder AddTargetedFilters<TFilterTarget>(...);
}
```

## Fluent API

The base class returns `HandlerBuilderBase` from every configuration method, enabling method chaining:

```csharp
builder
    .SetPriority(10)
    .SetConcurreny(5)
    .AddFilter(MyCustomFilter)
    .SetState<SenderIdResolver, int>(42);
```

For generic subclasses (e.g., `HandlerBuilder<Message>`), extension methods in `Telegrator.Handlers.Building` preserve the concrete type during chaining.

## Key Methods

<table>
<thead>
<tr><th>Method</th><th>Purpose</th></tr>
</thead>
<tbody>
<tr><td>`SetPriority(int)`</td><td>Sets the handler priority (higher = earlier execution)</td></tr>
<tr><td>`SetConcurreny(int)`</td><td>Sets the per-handler concurrency limit</td></tr>
<tr><td>`SetIndexer(int, int)`</td><td>Sets both priority and concurrency in one call</td></tr>
<tr><td>`AddFilter(IFilter<Update>)`</td><td>Adds a raw update filter</td></tr>
<tr><td>`AddFilters(params IFilter<Update>[])`</td><td>Adds multiple raw update filters</td></tr>
<tr><td>`SetState<TKey, TValue>(TValue?)`</td><td>Attaches a state keeper with a key resolver</td></tr>
<tr><td>`AddTargetedFilter<T>(Func<Update, T?>, IFilter<T>)`</td><td>Adds a filter targeted at a specific sub-type (e.g., `Message`)</td></tr>
<tr><td>`SetUpdateValidating(UpdateValidateAction)`</td><td>Sets a validation action that runs before filters</td></tr>
</tbody>
</table>

## State Keepers

State keepers are a special kind of filter that persists across updates. They are typically used with the `StateAttribute`:

```csharp
builder.SetState<SenderIdResolver, QuizState>(QuizState.Start);
```

## Roslyn-Generated Extensions

Filter attributes (e.g., `[TextContains]`, `[ChatType]`) automatically generate extension methods on `IHandlerBuilder` at compile time via `Telegrator.RoslynGenerators`. This allows you to write:

```csharp
builder.TextContains("hello").ChatType(ChatType.Private);
```

instead of manually constructing filter instances.
