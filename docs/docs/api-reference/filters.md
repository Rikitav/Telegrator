---
title: "Filters"
description: "The declarative filter system that decides which handlers run for each update."
---

# Filters

Filters are the backbone of Telegrator's routing. They evaluate incoming updates and determine whether a handler should be executed. Filters can be applied as attributes, registered imperatively, or composed programmatically.

## Core Interface

```csharp
public interface IFilter<in T>
{
    bool CanPass(FilterExecutionContext<T> context);
}
```

For updates, the common type is `IFilter<Update>`.

## Declarative Filters (Attributes)

The most common way to define filters is via attributes on handler classes:

```csharp
[MessageHandler]
[TextContains("hello")]
[ChatType(ChatType.Private)]
public class HelloHandler : MessageHandler { }
```

### Built-in Filter Attributes

- **`[MessageHandler]`**
  - Targets: `Message`
  - Description: Handles text and media messages
- **`[CommandHandler]`**
  - Targets: `Message`
  - Description: Handles messages starting with `/`
- **`[CallbackQueryHandler]`**
  - Targets: `CallbackQuery`
  - Description: Handles inline button clicks
- **`[AnyUpdateHandler]`**
  - Targets: `Update`
  - Description: Handles any update type
- **`[TextContains("...")]`**
  - Targets: `Message`
  - Description: Matches substrings in message text
- **`[TextStartsWith("...")]`**
  - Targets: `Message`
  - Description: Matches prefixes
- **`[ChatType(...)]`**
  - Targets: `Message`
  - Description: Matches private/group/supergroup/channel
- **`[FromUserId(123)]`**
  - Targets: `Update`
  - Description: Matches specific user ID
- **`[State<TKey, TValue>(value)]`**
  - Targets: `Update`
  - Description: Matches current state in storage

## Filter Modifiers

Control how multiple filters combine:

```csharp
[TextContains("bot", Modifiers = FilterModifier.OrNext)]
[Mentioned]
public class BotOrMentionHandler : MessageHandler { }
```

- **`None` (default)**: AND with the next filter
- **`Not`**: Inverts the filter
- **`OrNext`**: OR with the next filter

## Imperative Filters

Register filters at runtime via the builder API:

```csharp
builder.AddFilter(Filter<Update>.If(ctx => ctx.Update.Message?.Date > DateTime.UtcNow.AddMinutes(-5)));
```

## Targeted Filters

Filters that operate on a specific sub-type of `Update` (e.g., `Message`, `User`):

```csharp
builder.AddTargetedFilter(
    getFilteringTarget: update => update.Message,
    filter: new MyMessageFilter()
);
```

## Custom Filter Attribute

Create your own filter by inheriting from `UpdateFilterAttribute`:

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AdminFilterAttribute : UpdateFilterAttribute
{
    public override bool CanPass(FilterExecutionContext<Update> context)
    {
        return context.Update.Message?.From?.Id == AdminUserId;
    }
}
```

Roslyn generators will automatically create a `AdminFilter()` extension method for `IHandlerBuilder`.
