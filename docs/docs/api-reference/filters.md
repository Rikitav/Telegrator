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

<table>
<thead>
<tr><th>Attribute</th><th>Targets</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td>`[MessageHandler]`</td><td>`Message`</td><td>Handles text and media messages</td></tr>
<tr><td>`[CommandHandler]`</td><td>`Message`</td><td>Handles messages starting with `/`</td></tr>
<tr><td>`[CallbackQueryHandler]`</td><td>`CallbackQuery`</td><td>Handles inline button clicks</td></tr>
<tr><td>`[AnyUpdateHandler]`</td><td>`Update`</td><td>Handles any update type</td></tr>
<tr><td>`[TextContains("...")]`</td><td>`Message`</td><td>Matches substrings in message text</td></tr>
<tr><td>`[TextStartsWith("...")]`</td><td>`Message`</td><td>Matches prefixes</td></tr>
<tr><td>`[ChatType(...)]`</td><td>`Message`</td><td>Matches private/group/supergroup/channel</td></tr>
<tr><td>`[FromUserId(123)]`</td><td>`Update`</td><td>Matches specific user ID</td></tr>
<tr><td>`[State<TKey, TValue>(value)]`</td><td>`Update`</td><td>Matches current state in storage</td></tr>
</tbody>
</table>

## Filter Modifiers

Control how multiple filters combine:

```csharp
[TextContains("bot", Modifiers = FilterModifier.OrNext)]
[Mentioned]
public class BotOrMentionHandler : MessageHandler { }
```

<table>
<thead>
<tr><th>Modifier</th><th>Behavior</th></tr>
</thead>
<tbody>
<tr><td>`None` (default)</td><td>AND with the next filter</td></tr>
<tr><td>`Not`</td><td>Inverts the filter</td></tr>
<tr><td>`OrNext`</td><td>OR with the next filter</td></tr>
</tbody>
</table>

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
