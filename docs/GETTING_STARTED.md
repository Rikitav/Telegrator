# Getting Started with Telegrator

---

This guide will walk you through the core concepts and advanced features of **Telegrator** — a modern, aspect-oriented, mediator-based framework for building powerful and maintainable Telegram bots in C#.

- [1. Installation](#1-installation)
- [2. Framework Mechanics Overview](#2-framework-mechanics-overview)
  - [2.1. Basic Concepts](#21-basic-concepts)
  - [2.2. Practice: Minimal Bot](#22-practice-minimal-bot)
  - [2.3. Working with Filters](#23-working-with-filters)
  - [2.4. State Management](#24-state-management)
  - [2.5. Concurrency & Awaiting](#25-concurrency--awaiting)
  - [2.6. Extensibility](#26-extensibility)
  - [2.7. Integration](#27-integration)
- [3. Step-by-Step Tutorials](#3-step-by-step-tutorials)
  - [3.1. Minimal Bot Creation](#31-minimal-bot-creation)
  - [3.2. Command Filtering](#32-command-filtering)
  - [3.3. State Management Wizard](#33-state-management-wizard)
  - [3.4. Awaiting CallbackQuery](#34-awaiting-callbackquery)
  - [3.5. Adding a Custom Filter](#35-adding-a-custom-filter)
- [4. Advanced Topics](#4-advanced-topics)
  - [4.1. Handler Priority](#41-handler-priority)
  - [4.2. Dependency Injection (DI)](#42-dependency-injection-di)
  - [4.3. Custom State Keepers](#43-custom-state-keepers)
  - [4.4. Automatic Handler Discovery](#44-automatic-handler-discovery)
  - [4.5. Hosting Integration](#45-hosting-integration)
  - [4.6. Error Handling and Logging](#46-error-handling-and-logging)
  - [4.7. Performance Optimization](#47-performance-optimization)
  - [4.8. Best Practices](#48-best-practices)
- [5. FAQ & Best Practices](#5-faq--best-practices)
  - [Q: My handler is not being triggered. What should I do?](#q-my-handler-is-not-being-triggered-what-should-i-do)
  - [Q: How can I access the `ITelegramBotClient` or the original `Update` object inside a handler?](#q-how-can-i-access-the-itelegrambotclient-or-the-original-update-object-inside-a-handler)
  - [Q: How do I handle errors?](#q-how-do-i-handle-errors)
  - [Q: How can I organize my code for a large bot?](#q-how-can-i-organize-my-code-for-a-large-bot)
- [6. Links](#6-links)

---

## 1. Installation

**Telegrator** is distributed as a NuGet package. You can install it using the .NET CLI, the NuGet Package Manager Console, or by managing NuGet packages in Visual Studio.

### Prerequisites
- .NET >= 5.0 `or` .NET Core >= 2.0 `or` Framework >= 4.6.1 (.NET Standart 2.0 compatible)
- A Telegram Bot Token from [@BotFather](https://t.me/BotFather).

### .NET CLI
```shell
dotnet add package Telegrator
```

### Package Manager Console
```shell
Install-Package Telegrator
```

### Hosting Integrations
- .NET Core >= 8.0 
- `Telegrator.Hosting`: For console/background services
- `Telegrator.Hosting.Web`: For ASP.NET Core/Webhook (WIP)

---

## 2. Framework Mechanics Overview

### 2.1. Basic Concepts

Telegrator is built around several core ideas:

- **Aspect-Oriented Handlers**: Each handler is a focused, reusable class that reacts to a specific type of update (message, command, callback, etc.).
- **Mediator Pattern**: All updates are routed through a central `UpdateRouter`, which dispatches them to the appropriate handlers based on filters and priorities.
- **Filters as Attributes**: Handler classes are decorated with filter attributes that declaratively specify when the handler should run.
- **State Management**: Built-in mechanisms for managing user/chat state without external storage.
- **Concurrency Control**: Fine-grained control over how many handlers run in parallel, both globally and per-handler.

### 2.2. Practice: Minimal Bot

Here's how to create a minimal bot that replies to any private message containing "hello":

```csharp
using Telegrator;
using Telegrator.Handlers;
using Telegrator.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

[MessageHandler]
[ChatType(ChatType.Private)]
[TextContains("hello", StringComparison.InvariantCultureIgnoreCase)]
public class HelloHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello! Nice to meet you!", cancellationToken: cancellation);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
        bot.Handlers.AddHandler<HelloHandler>();
        bot.StartReceiving();
        Console.ReadLine();
    }
}
```

> **How is it working?**
> 1. **`[MessageHandler]`**: This attribute marks `HelloHandler` as a handler for `Message` updates.
> 2. **`[ChatType(ChatType.Private)]`**: This filter ensures the handler only runs for private chat messages.
> 3. **`[TextContains("hello")]`**: This filter checks if the message contains "hello" (case-insensitive).
> 4. **`TelegratorClient`**: The main bot client that manages the connection to Telegram and the update processing pipeline.
> 5. **`bot.Handlers.AddHandler<HelloHandler>()`**: Registers the handler with the bot.
> 6. **`bot.StartReceiving()`**: Starts the long-polling loop to fetch updates from Telegram.
> 7. **`Reply(...)`**: A helper method that sends a reply to the original message.

### 2.3. Working with Filters

Filters are the gatekeepers of your bot logic. They are applied as attributes to handler classes and determine when a handler should be executed.

**Common Filters:**
- `[CommandAllias("start")]` — Only for the `/start` command
- `[TextContains("hello")]` — Message contains "hello"
- `[ChatType(ChatType.Private)]` — Only private chats
- `[FromUserId(123456789)]` — Only from a specific user
- `[HasReply]` — Only if the message is a reply

**Combining Filters:**
You can combine filters using logical modifiers:
- Multiple filters on a handler, by default, are combined with logical AND
- `Modifiers = FilterModifier.OrNext` - will combine this and next filter with OR logic
- `Modifiers = FilterModifier.Not` - Inverts the filter
- This flags can be combined using bit OR (`Modifiers = FilterModifier.Not | FilterModifier.OrNext`)

**Example:**
```csharp
[MessageHandler]
[ChatType(ChatType.Private)]
[TextContains("hello", Modifiers = FilterModifier.Not)]
public class NotHelloHandler : MessageHandler
{
    // Runs for private messages that do NOT contain "hello"
}

[MessageHandler]
[TextContains("bot", Modifiers = FilterModifier.OrNext)]
[Mentioned()]
public class NotHelloHandler : MessageHandler
{
    // Runs for messages that contains "bot" or if bot was mentioned using @
}
```

> **How is it working?**
> 1. **Multiple Filters**: The handler has two filters that work together with logical AND.
> 2. **`[ChatType(ChatType.Private)]`**: Ensures only private chat messages are processed.
> 3. **`[TextContains("hello", Modifiers = FilterModifier.Not)]`**: The `Not` modifier inverts the filter, so it matches messages that do NOT contain "hello".
> 4. **Combined Logic**: The handler will only run for private messages that don't contain "hello".

### 2.4. State Management

Telegrator provides built-in state management for multi-step conversations (wizards, forms, quizzes) without a database.

> [!NOTE]
> Each type of `StateKeeper`'s (EnumStateKeeper, NumericStateKeeper) is shared beetwen **EVERY** handler in project.

**Types of State:**
- **NumericState**: Integer-based steps
- **StringState**: Named steps
- **EnumState**: Enum-based scenarios

**How to Use:**
1. Define your state (enum/int/string)
2. Use a state filter attribute on your handler:
   - `[EnumState<MyEnum>(MyEnum.Step1)]`
   - `[NumericState(1)]`
3. Change state inside the handler using extension methods:
   - `container.ForwardEnumState<MyEnum>()`
   - `container.ForwardNumericState()`
   - `container.DeleteEnumState<MyEnum>()`

**Example:**
```csharp
public enum QuizState
{
    Start = SpecialState.NoState, Q1, Q2
}

[CommandHandler]
[CommandAllias("quiz")]
[EnumState<QuizState>(QuizState.Start)]
public class StartQuizHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.ForwardEnumState<QuizState>();
        await Reply("Quiz started! Question 1: What is the capital of France?");
    }
}

[MessageHandler]
[EnumState<QuizState>(QuizState.Q1)]
public class Q1Handler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input.Text.Trim().Equals("Paris", StringComparison.InvariantCultureIgnoreCase))
            await Reply("Correct!");
        else
            await Reply("Incorrect. The answer is Paris.");

        container.ForwardEnumState<QuizState>();
        await Reply("Question 2: What is 2 + 2?");
    }
}
```

> **How is it working?**
> 1. **Enum State Definition**: `QuizState` enum defines the conversation flow with `Start = SpecialState.NoState` indicating no initial state.
> 2. **State Filter**: `[EnumState<QuizState>(QuizState.Start)]` ensures the handler only runs when the user is in the "Start" state.
> 3. **State Transition**: `container.ForwardEnumState<QuizState>()` moves the user to the next state (Q1).
> 4. **Next Handler**: The `Q1Handler` will only run when the user is in state `QuizState.Q1`.
> 5. **State Management**: Each handler manages its own state transition, creating a clear conversation flow.

### 2.5. Concurrency & Awaiting

**Concurrency Control:**
- Limit the number of concurrent executions globally using `MaximumParallelWorkingHandlers` in `TelegramBotOptions`

**Awaiting Other Updates:**
- Use `AwaitingProvider` to wait for a user's next update (message or callback) inside a handler:

```csharp
[CommandHandler]
[CommandAllias("ask")]
public class AskHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");
        var nextMessage = await container.AwaitMessage().BySenderId(cancellation);
        await Reply($"Hello, {nextMessage.Text}!");
    }
}
```

> **How is it working?**
> 1. **Awaiting Provider**: `container.AwaitMessage()` creates a temporary handler that waits for the next message.
> 2. **Sender Filter**: `.BySenderId(cancellation)` ensures only messages from the same user are captured.
> 3. **Async Flow**: The handler pauses execution until the user responds, then continues with the conversation.
> 4. **Context Preservation**: The original handler context is maintained during the awaiting process.

### 2.6. Extensibility

You can extend Telegrator by creating custom filters, attributes, and state keepers.

**Custom Filter Attribute Example:**
```csharp
using Telegram.Bot.Types;
using Telgrator.Attributes;
using Telegrator.Handlers;

public class AdminOnlyAttribute() : FilterAnnotation<Message>
{ 
    private readonly List<long> _adminIds = [];

    public void AddAdmin(long id) => _adminIds.Add(id);
    public void RemoveAdmin(long id) => _adminIds.Remove(id);
    
    public override bool CanPass(FilterExecutionContext<Message> context)
        => _adminIds.Contains(context.Input.From?.Id);
}

[MessageHandler]
[AdminOnly]
public class AdminHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, admin!");
    }
}

// ...
AdminOnlyAttribute.AddAdmin(123456789);
bot.StartReceiving();
```

> **How is it working?**
> 1. **Custom Filter**: `AdminOnlyAttribute` inherits from `FilterAnnotation<Message>` to create a reusable filter attribute.
> 2. **Filter Logic**: `CanPass()` method checks if the message sender's ID matches the admin ID.
> 4. **Usage**: The filter is applied as an attribute `[AdminOnly]` to restrict access users that not registered as admins.

### 2.7. Integration

Telegrator works in console, hosted applications, --and ASP.NET Core (webhook)-- (WIP) projects.

**Console App Example:**
```csharp
var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
bot.Handlers.CollectHandlersDomainWide();
bot.StartReceiving();
```

**Hosting Example:**
```csharp
using Telegrator.Hosting;

var builder = TelegramBotHost.CreateBuilder();
builder.Handlers.AddHandler<StartHandler>();
var host = builder.Build();
await host.StartAsync();
```

> **How is it working?**
> 1. **Console Integration**: `TelegratorClient` provides a simple way to create bots in console applications.
> 2. **Domain-Wide Collection**: `CollectHandlersDomainWide()` automatically discovers and registers all handlers in the current assembly.
> 3. **ASP.NET Core Integration**: `TelegramBotHost.CreateBuilder()` provides a builder pattern for hosting bots in ASP.NET Core applications.
> 4. **Dependency Injection**: Handlers and their dependencies are automatically registered with the DI container.

---

## 3. Step-by-Step Tutorials

### 3.1. Minimal Bot Creation
See [Practice: Minimal Bot](#practice-minimal-bot).

### 3.2. Command Filtering
Message is considered command if is start with '/' and has not null or empty name.
Instead of using the `MessageHandler` for command (such as `/start`) you should use `CommandHandler`.

```csharp
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Use /help to see available commands.");
    }
}

[CommandHandler]
[CommandAllias("help")]
public class HelpHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Available commands:\n/start - Start the bot\n/help - Show this help");
    }
}

[MessageHandler]
[TextStartsWith("/", Modifiers = FilterModifier.Not)]
public class EchoHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"You said: \"{Input.Text}\"");
    }
}
```

> **How is it working?**
> 1. **Command Handlers**: `[CommandHandler]` and `[CommandAllias]` work together to handle specific commands like `/start` and `/help`.
> 2. **Echo Handler**: `[TextStartsWith("/", Modifiers = FilterModifier.Not)]` catches all messages that don't start with "/" (non-commands).
> 3. **Handler Separation**: Each command has its own dedicated handler, making the code modular and maintainable.
> 4. **Filter Modifiers**: The `Not` modifier inverts the filter logic to exclude command messages.

### 3.3. State Management Wizard
```csharp
[CommandHandler]
[CommandAllias("wizard")]
[NumericState(SpecialState.NoState)]
public class StartWizardHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.CreateNumericState(); // This code is not necesary, as "Forward" method can automatically creates state if needed, but its recomended to use
        container.ForwardNumericState();
        await Reply("What is your name?");
    }
}

[MessageHandler]
[NumericState(1)]
public class NameHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.ForwardNumericState();
        await Reply($"Nice to meet you, {Input.Text}! How old are you?");
    }
}

[MessageHandler]
[NumericState(2)]
public class AgeHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (int.TryParse(Input.Text, out int age))
        {
            await Reply($"Thank you! You are {age} years old. Wizard completed!");
            container.DeleteNumericState();
        }
        else
        {
            await Reply("Please enter a valid age (number).");
        }
    }
}
```

> **How is it working?**
> 1. **Numeric State**: `[NumericState(SpecialState.NoState)]` starts the wizard when no state exists.
> 2. **State Creation**: `container.CreateNumericState()` initializes the numeric state for the user.
> 3. **State Progression**: `container.ForwardNumericState()` moves to the next step (1, then 2).
> 4. **State Cleanup**: `container.DeleteNumericState()` removes the state when the wizard completes.
> 5. **Input Validation**: The age handler validates numeric input and provides feedback.

### 3.4. Awaiting CallbackQuery
```csharp
[CommandHandler]
[CommandAllias("menu")]
public class MenuHandler : CommandHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Option 1", "option1"),
            InlineKeyboardButton.WithCallbackData("Option 2", "option2")
        });

        await Reply("Choose an option:", replyMarkup: keyboard, cancellationToken: cancellation);
    }
}

[CallbackQueryHandler]
[CallbackData("option1")]
public class Option1Handler : CallbackQueryHandler
{
    public override async Task Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 1!", cancellationToken: cancellation);
        await EditMessageText("You selected Option 1!");
    }
}

[CallbackQueryHandler]
[CallbackData("option2")]
public class Option2Handler : CallbackQueryHandler
{
    public override async Task Execute(IAbstractHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 2!", cancellationToken: cancellation);
        await EditMessageText("You selected Option 2!");
    }
}
```

> **How is it working?**
> 1. **Inline Keyboard**: `InlineKeyboardMarkup` creates interactive buttons with `CallbackData` identifiers.
> 2. **CallbackQuery Handlers**: `[CallbackQueryHandler]` and `[CallbackData]` work together to handle button clicks.
> 3. **Response Methods**: `AnswerCallbackQuery()` provides immediate feedback, while `EditMessageText()` updates the message.
> 4. **Handler Separation**: Each button option has its own dedicated handler for clean code organization.

### 3.5. Adding a Custom Filter
```csharp
public class PremiumUserAttribute : UpdateFilterAttribute<Message>
{
    public override Message? GetFilterringTarget(Update update) => update.Message;
    public override bool CanPass(FilterExecutionContext<Message> context) => context.Input.From?.IsPremium == true;
}

[MessageHandler]
[PremiumUser]
public class PremiumFeatureHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This feature is only available for premium users!");
    }
}
```

> **How is it working?**
> 1. **Custom Filter**: `PremiumUserAttribute` inherits from `UpdateFilterAttribute<Message>` to create a reusable filter.
> 2. **Premium Check**: `context.Input.From?.IsPremium == true` checks if the user has Telegram Premium.
> 3. **Target Extraction**: `GetFilterringTarget()` extracts the `Message` from the `Update` object.
> 4. **Usage**: The filter is applied as `[PremiumUser]` to restrict features to premium users only.

---

## 4. Advanced Topics

### 4.1. Handler Priority
By default, handlers are processed in the order they are added. However, you can control the execution order using the `Priority` property in the handler attribute. A greater number means higher priority.

```csharp
[MessageHandler(Priority = 1)] // Runs before default priority (0)
public class HighPriorityHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs first!");
    }
}

[MessageHandler(Priority = 0)] // Default priority
public class NormalPriorityHandler : MessageHandler
{
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs second!");
    }
}
```

> **How is it working?**
> 1. **Priority System**: The `Priority` property in handler attributes controls execution order.
> 2. **Higher Priority First**: Handlers with higher priority numbers (1) run before those with lower numbers (0).
> 3. **Default Priority**: When not specified, handlers have priority 0.
> 4. **Execution Order**: This ensures critical handlers (like admin commands) run before general handlers.

### 4.2. Dependency Injection (DI)
Telegrator is designed to work seamlessly with DI containers (e.g., ASP.NET Core). Handlers and their dependencies are automatically registered.

```csharp
[MessageHandler]
public class MyHandler : MessageHandler
{
    private readonly IMyService _myService;
    private readonly ILogger<MyHandler> _logger;
    public MyHandler(IMyService myService, ILogger<MyHandler> logger)
    {
        _myService = myService;
        _logger = logger;
    }
    public override async Task Execute(IAbstractHandlerContainer<Message> container, CancellationToken cancellation)
    {
        _logger.LogInformation("MyHandler executed!");
        var result = _myService.DoSomething();
        await Reply(result);
    }
}
```

> **How is it working?**
> 1. **Constructor Injection**: Dependencies (`IMyService`, `ILogger`) are automatically injected by the DI container.
> 2. **Service Registration**: When using `Telegrator.Hosting`, services are automatically registered with the DI container.
> 3. **Handler Instantiation**: Telegrator creates handler instances through the DI container, resolving all dependencies.
> 4. **Logging Integration**: Built-in logging support allows for comprehensive debugging and monitoring.

### 4.3. Custom State Keepers
You can implement your own state keeper by inheriting from `StateKeeperBase<TKey, TState>`. This allows for advanced scenarios (e.g., per-message state, custom key resolution).

### 4.4. Automatic Handler Discovery
Telegrator provides automatic discovery and registration of handlers across your entire application domain using the `CollectHandlersDomainWide()` method.

**How it works:**
- Scans all loaded assemblies in the current domain
- Automatically discovers classes decorated with handler attributes
- Registers them with the bot without manual registration
- 

**Example:**
```csharp
var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
bot.Handlers.CollectHandlersDomainWide(); // Automatically finds and registers all handlers
bot.StartReceiving();
```

**Benefits:**
- No need to manually register each handler
- Reduces boilerplate code
- Ensures all handlers are discovered automatically
- Perfect for large applications with many handlers

> **How is it working?**
> 1. **Domain Scanning**: `CollectHandlersDomainWide()` scans all assemblies loaded in the current AppDomain.
> 2. **Reflection Discovery**: Uses reflection to find all classes decorated with handler attributes.
> 3. **Automatic Registration**: Each discovered handler is automatically registered with the `HandlersCollection`.
> 4. **Handler Types**: Supports all handler types: `MessageHandler`, `CommandHandler`, `CallbackQueryHandler`, etc.

### 4.5. Hosting Integration
Telegrator provides seamless integration with .NET's generic host through the `Telegrator.Hosting` package, making it easy to build production-ready bot applications.

**Installation:**
```shell
dotnet add package Telegrator.Hosting
```

**Dependencies:**
- `Microsoft.Extensions.Hosting` - .NET Generic Host
- `Microsoft.Extensions.DependencyInjection` - Dependency Injection
- `Microsoft.Extensions.Configuration` - Configuration management
- `Microsoft.Extensions.Logging` - Logging infrastructure

**Core Components:**
- `TelegramBotHost` - The main hosted service that manages the bot lifecycle
- `TelegramBotHostBuilder` - Builder pattern for configuring the bot host
- `TelegramBotOptions` - Configuration options for the bot

**Basic Example:**
```csharp
var builder = TelegramBotHost.CreateBuilder();

// Configure services
builder.Services.AddSingleton<IMyService, MyService>();

// Configure handlers
builder.Handlers.CollectHandlersDomainWide();

// Building host
var host = builder.Build();
await host.Run();
```

> **How is it working?**
> 1. **Generic Host Integration**: `TelegramBotHost` implements `IHost` and integrates with .NET's generic host.
> 2. **Lifecycle Management**: The host manages the bot's startup, shutdown, and graceful termination.
> 3. **Dependency Injection**: All handlers and services are automatically registered with the DI container.
> 4. **Configuration**: Supports standard .NET configuration patterns (appsettings.json, environment variables, etc.).
> 5. **Logging**: Integrates with .NET's logging infrastructure for comprehensive monitoring.
> 6. **Health Checks**: Can be integrated with .NET's health check system for production monitoring.

### 4.6. Error Handling and Logging
You can subscribe to error events or set a custom exception handler:

```csharp
bot.UpdateRouter.ExceptionHandler = new DefaultRouterExceptionHandler((client, exception, source, cancellationToken) =>
{
    Console.WriteLine($"An error occurred: {exception.Message}");
    return Task.CompletedTask;
});
```

> **How is it working?**
> 1. **Exception Handler**: `ExceptionHandler` property allows you to set a custom exception handler for the entire bot.
> 2. **Error Context**: The handler receives the bot client, exception, source information, and cancellation token.
> 3. **Global Error Handling**: This provides a centralized way to handle all exceptions that occur during update processing.
> 4. **Logging Integration**: Perfect place to log errors or send notifications to administrators.

### 4.7. Performance Optimization
- Use appropriate concurrency limits for resource-intensive operations
- Avoid thread-blocking operations in handlers
- Use state management for multi-step processes
- Use `AwaitingProvider` for complex conversation flows

### 4.8. Best Practices
- Organize handlers, filters, and state keepers in separate folders
- Use feature modules for large bots
- Prefer declarative filters over manual `if` statements
- Keep handlers focused and single-responsibility

---

## 5. FAQ & Best Practices

### 5.1. Q: My handler is not being triggered. What should I do?
- Check handler registration (use `bot.Handlers.AddHandler<MyHandler>()` or domain-wide collection)
- Check filter attributes and update types
- Enable debug logging

### 5.2. Q: How can I access the `ITelegramBotClient` or the original `Update` object inside a handler?
- Use `Client`, `Update`, and `Input` properties in your handler

### 5.3. Q: How do I handle errors?
- Set a custom exception handler or subscribe to error events

### 5.4. Q: How can I organize my code for a large bot?
- Use folders, feature modules, and namespaces
- Keep handlers focused and modular

---

## 6. Links

- [API Reference](./TelegramReactive_Api.md)
- [Main Repository](https://github.com/Rikitav/Telegrator)
- [Wiki & Examples](https://github.com/Rikitav/Telegrator/wiki/)
- [NuGet Package](https://www.nuget.org/packages/Telegrator)
- [Issues & Discussions](https://github.com/Rikitav/Telegrator/issues)

---

> **Feel free to contribute, ask questions, or open issues!** 