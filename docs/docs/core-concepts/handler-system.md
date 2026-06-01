---
title: "Handler System"
description: "Learn about the core handler ideas and lifecycle."
---

# Handler System

Telegrator is built around several core ideas:

- **Native AOT Compatible**: Telegrator is ready for the future with full support for Native AOT. It avoids reflection-based discovery where possible and uses source generation for high performance.
- **Aspect-Oriented Handlers**: Each handler is a focused, reusable class that reacts to a specific type of update (message, command, callback, etc.).
- **Aspect-Oriented Programming**: Built-in support for pre and post-execution processing through aspects, enabling separation of cross-cutting concerns.
- **Mediator Pattern**: All updates are routed through a central `UpdateRouter`, which dispatches them to the appropriate handlers based on filters and priorities.
- **Filters as Attributes**: Handler classes are decorated with filter attributes that declaratively specify when the handler should run.
- **State Management**: Built-in mechanisms for managing user/chat state without external storage.
- **Concurrency Control**: Fine-grained control over how many handlers run in parallel, both globally and per-handler.

## Handler Lifecycle
1. **Registration**: Handlers are registered with the bot during startup.
2. **Discovery**: The framework uses a **Source Generator** to discover handlers at compile-time (via `CollectHandlers()`), which is faster than reflection and supports Native AOT.
3. **Filtering**: Updates are filtered to determine which handlers should run.
4. **Validation**: The **DeveloperHelperAnalyzer** checks your handler declarations at design-time to prevent common mistakes.
5. **Execution**: Selected handlers are executed in order of priority.
6. **Cleanup**: Resources are cleaned up after execution.

## Base Handler Classes

Telegrator provides specific base classes for almost every Telegram update type, ensuring type safety and code clarity:

- `MessageHandler`: For text and media messages.
- `CommandHandler`: For bot commands.
- `CallbackQueryHandler`: For inline button interactions.
- `EditedMessageHandler`: For message edits.
- `ChatMemberHandler`: For member status changes.
- `MyChatMemberHandler`: For the bot's own status changes.
- `PollHandler`: For poll updates.
- `AnyUpdateHandler`: The catch-all base class when you need raw `Update` access.

## Handler Metadata (Descriptors)

In the internal system, every handler is represented by a `HandlerDescriptor`. This is an abstract base class with two main implementations:
- `ClassHandlerDescriptor`: For standard class-based handlers.
- `MethodHandlerDescriptor`: For implicit handlers created from methods.

## DeveloperHelperAnalyzer

To ensure your bot works perfectly, Telegrator includes a **DeveloperHelperAnalyzer**. It works directly in your IDE (Visual Studio, Rider) and will point out errors such as:
- Using `[MessageHandler]` on a class that inherits from `CallbackQueryHandler`.
- Forgetting to make a keyboard generation method `partial`.
- Missing required handler attributes.

Telegrator provides two mechanisms for controlling execution order:

- **Importance**: Internal parameter based on handler type. 
    - `CommandHandler`: 1 (Highest)
    - `MessageHandler`: 0 (Default)
    - `AnyUpdateHandler`: -1 (Lowest)
- **Priority**: User-defined global priority via attribute property `Priority = X`.

**Execution Order**: Handlers are sorted first by *Importance*, then by *Priority* (descending).

## Declarative Keyboard Generation

Telegrator supports declarative keyboard generation using attributes on `partial` methods. This is the cleanest way to define complex `InlineKeyboardMarkup`.

```csharp
using Telegrator.Markups;

[CommandHandler, CommandAllias("settings")]
public partial class SettingsHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Options:", replyMarkup: SettingsKeyboard());
        return Ok;
    }

    [CallbackButton("English", "lang_en"), CallbackButton("Russian", "lang_ru")]
    [CallbackButton("Cancel", "cancel_settings")]
    private static partial InlineKeyboardMarkup SettingsKeyboard();
}
```

- **Attribute-based**: Use `[CallbackButton(text, data)]` to define buttons.
- **Row-based**: Each attribute line represents a row of buttons.
- **Source Generated**: Telegrator automatically implements the method body at compile-time.

You can create handlers directly from methods without defining full handler classes. This is useful for simple handlers or quick prototyping:

```csharp
// Simple echo handler
[MessageHandler]
[TextStartsWith("/", Modifiers = FilterModifier.Not)]
private static async Task<Result> EchoHandler(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    await container.Reply($"You said: \"{container.Input.Text}\"", cancellationToken: cancellationToken);
    return Ok;
}

// Command handler with inline keyboard
[CommandHandler]
[CommandAllias("menu")]
private static async Task<Result> MenuHandler(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    var keyboard = new InlineKeyboardMarkup(new[]
    {
        InlineKeyboardButton.WithCallbackData("Option 1", "option1"),
        InlineKeyboardButton.WithCallbackData("Option 2", "option2")
    });

    await container.Reply("Choose an option:", replyMarkup: keyboard, cancellationToken: cancellationToken);
    return Ok;
}

// Callback query handler
[CallbackQueryHandler]
[CallbackData("option1")]
private static async Task<Result> Option1Handler(IHandlerContainer<CallbackQuery> container, CancellationToken cancellationToken)
{
    await container.AnswerCallbackQuery("You selected Option 1!", cancellationToken: cancellationToken);
    await container.EditMessage("You selected Option 1!");
    return Ok;
}

// Register all methods as handlers
builder.Handlers.AddMethod<Message>(EchoHandler);
builder.Handlers.AddMethod<Message>(MenuHandler);
builder.Handlers.AddMethod<CallbackQuery>(Option1Handler);
```

**Advanced Example with State Management:**
```csharp
public enum UserState
{
    Start,
    WaitingForName,
    WaitingForAge
}

// Start conversation
[CommandHandler]
[CommandAllias("register")]
[State<SenderIdResolver, UserState>(UserState.Start)]
private static async Task<Result> StartRegistration(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    await container.StateStorage.GetStateMachine<UserState>(container.HandlingUpdate).BySenderId().Advance();
    await container.Reply("Please enter your name:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle name input
[MessageHandler]
[State<SenderIdResolver, UserState>(UserState.WaitingForName)]
private static async Task<Result> HandleName(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    var name = container.Input.Text;
    await container.StateStorage.GetStateMachine<UserState>(container.HandlingUpdate).BySenderId().Advance();
    await container.Reply($"Hello {name}! Please enter your age:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle age input
[MessageHandler]
[State<SenderIdResolver, UserState>(UserState.WaitingForAge)]
private static async Task<Result> HandleAge(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    if (int.TryParse(container.Input.Text, out int age))
    {
        await container.StateStorage.GetStateMachine<UserState>(container.HandlingUpdate).BySenderId().Reset();
        await container.Reply("Registration complete!", cancellationToken: cancellationToken);
    }
    else
    {
        await container.Reply("Please enter a valid age (number):", cancellationToken: cancellationToken);
    }

    return Ok;
}

// Register state management handlers
builder.Handlers.AddMethod<Message>(StartRegistration);
builder.Handlers.AddMethod<Message>(HandleName);
builder.Handlers.AddMethod<Message>(HandleAge);
```

> **How is it working?**
> 1. **Method Signature**: Methods must return `Task<Result>` and accept `IHandlerContainer<T>` and `CancellationToken`
> 2. **Attributes**: Apply the same filter attributes as regular handlers (`[MessageHandler]`, `[CommandHandler]`, etc.)
> 3. **Container Methods**: Use extension methods like `container.Reply()`, `container.Response()`, etc.
> 4. **Registration**: Use `AddMethod<T>()` to register methods as handlers
> 5. **State Management**: Same state management patterns as regular handlers
> 6. **Flexibility**: Can be used for simple handlers or complex multi-step conversations
