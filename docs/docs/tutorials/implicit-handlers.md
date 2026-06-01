---
title: "Implicit Handlers"
description: "Create handlers directly from methods."
---

# Implicit Handlers from Methods

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
    Start = 0,
    WaitingForName = 1,
    WaitingForAge = 2
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
