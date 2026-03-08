# Getting Started with Telegrator

---

This guide will walk you through the core concepts and advanced features of **Telegrator** — a modern, aspect-oriented, mediator-based framework for building powerful and maintainable Telegram bots in C#.

- [1. Installation](#1-installation)
- [2. Quick Start](#2-quick-start)
  - [2.1. Your First Bot](#21-your-first-bot)
  - [2.2. Basic Handler Types](#22-basic-handler-types)
  - [2.3. Simple Filters](#23-simple-filters)
- [3. Core Concepts](#3-core-concepts)
  - [3.1. Handler System](#31-handler-system)
  - [3.2. Filter System](#32-filter-system)
  - [3.3. State Management](#33-state-management)
  - [3.4. Update Routing](#34-update-routing)
- [4. Intermediate Topics](#4-intermediate-topics)
  - [4.1. Advanced Filters](#41-advanced-filters)
  - [4.2. Awaiting Mechanism](#42-awaiting-mechanism)
  - [4.3. Branching Handlers](#43-branching-handlers)
  - [4.4. Advanced Hosting Integration](#44-advanced-hosting-integration)
- [5. Advanced Topics](#5-advanced-topics)
  - [5.1. Aspects & Cross-Cutting Concerns](#51-aspects--cross-cutting-concerns)
  - [5.2. Custom Extensions](#52-custom-extensions)
  - [5.3. Performance Optimization](#53-performance-optimization)
  - [5.4. Error Handling](#54-error-handling)
- [6. Integration & Deployment](#6-integration--deployment)
  - [6.1. Console Applications](#61-console-applications)
  - [6.2. ASP.NET Core Hosting](#62-aspnet-core-hosting)
  - [6.3. Webhook Deployment](#63-webhook-deployment)
  - [6.4. Configuration Management](#64-configuration-management)
- [7. Best Practices & Patterns](#7-best-practices--patterns)
  - [7.1. Project Organization](#71-project-organization)
  - [7.2. Testing Strategies](#72-testing-strategies)
  - [7.3. Common Patterns](#73-common-patterns)
  - [7.4. Performance Tips](#74-performance-tips)
  - [7.5. Logging System](#75-logging-system)
- [8. FAQ & Troubleshooting](#8-faq--troubleshooting)
  - [8.1. Common Issues](#81-common-issues)
  - [8.2. Debugging Guide](#82-debugging-guide)
- [9. Links](#9-links)

---

## 1. Installation

**Telegrator** is distributed as a NuGet package. You can install it using the .NET CLI, the NuGet Package Manager Console, or by managing NuGet packages in Visual Studio.

### Prerequisites
- .NET >= 5.0 `or` .NET Core >= 2.0 `or` Framework >= 4.6.1 (.NET Standard 2.1 compatible)
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
- .NET Core >= 10.0 
- `Telegrator.Hosting`: For console/background services
- `Telegrator.Hosting.Web`: For ASP.NET Core/Webhook

```shell
# For console/background services
dotnet add package Telegrator.Hosting

# For webhook hosting
dotnet add package Telegrator.Hosting.Web
```

---

## 2. Quick Start

This section will get you up and running with Telegrator quickly. You'll learn the basics and create your first bot in minutes.

### 2.1. Your First Bot

Let's create a simple bot that replies to any private message containing "hello":

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
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello! Nice to meet you!", cancellationToken: cancellation);
        return Ok;
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
> 1. **`[MessageHandler]`**: Marks the class as a handler for message updates
> 2. **`[ChatType(ChatType.Private)]`**: Only processes private chat messages
> 3. **`[TextContains("hello")]`**: Only processes messages containing "hello" (case-insensitive)
> 4. **`TelegratorClient`**: Main bot client that manages Telegram connection
> 5. **`bot.Handlers.AddHandler<HelloHandler>()`**: Registers the handler
> 6. **`bot.StartReceiving()`**: Starts the long-polling loop
> 7. **`Reply(...)`**: Sends a reply to the original message

### 2.2. Basic Handler Types

Telegrator provides several handler types for different update types:

#### MessageHandler
Handles text messages and media:

```csharp
[MessageHandler]
[TextContains("hello")]
public class GreetingHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello there!");
        return Ok;
    }
}
```

#### CommandHandler
Handles bot commands (messages starting with `/`):

```csharp
[CommandHandler]
[CommandAlias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Use /help to see available commands.");
        return Ok;
    }
}
```

#### CallbackQueryHandler
Handles button clicks and inline keyboard interactions:

```csharp
[CallbackQueryHandler]
[TextStartsWith("action_")]
public class ActionHandler : CallbackQueryHandler
{
    public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("Action completed!");
        return Ok;
    }
}
```

#### AnyUpdateHandler
Handles any type of update:

```csharp
[AnyUpdateHandler]
public class LoggingHandler : AnyUpdateHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Update> container, CancellationToken cancellation)
    {
        Console.WriteLine($"Received update: {container.HandlingUpdate.Type}");
        return Ok;
    }
}
```

### 2.3. Simple Filters

Filters determine when your handlers should run. Here are the most common ones:

#### Text Filters
```csharp
[TextContains("hello")]           // Message contains "hello"
[TextStartsWith("/")]             // Message starts with "/"
[TextStartsWith("/", Modifiers = FilterModifier.Not)]  // Message does NOT start with "/"
```

#### User Filters
```csharp
[FromUserId(123456789)]           // Only from specific user ID
[FromUser("John")]                // Only from user with specific name
[FromUsername("john_doe")]        // Only from user with specific username
```

#### Chat Filters
```csharp
[ChatType(ChatType.Private)]      // Only private chats
[ChatType(ChatType.Group)]        // Only group chats
[Mentioned]                       // Only if bot was mentioned with @
```

#### Command Filters
```csharp
[CommandAlias("start")]          // Only for /start command
[CommandAlias("help")]           // Only for /help command
```

#### Combining Filters
Filters are combined with AND logic by default. You can use modifiers:

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
public class BotMentionHandler : MessageHandler
{
    // Runs for messages that contain "bot" OR if bot was mentioned
}
```

> **Filter Modifiers:**
> - `FilterModifier.Not` - Inverts the filter
> - `FilterModifier.OrNext` - Combines with next filter using OR logic
> - Can be combined: `Modifiers = FilterModifier.Not | FilterModifier.OrNext`

---

## 3. Core Concepts

This section covers the fundamental concepts and architecture of Telegrator.

### 3.1. Handler System

Telegrator is built around several core ideas:

- **Aspect-Oriented Handlers**: Each handler is a focused, reusable class that reacts to a specific type of update (message, command, callback, etc.).
- **Aspect-Oriented Programming**: Built-in support for pre and post-execution processing through aspects, enabling separation of cross-cutting concerns.
- **Mediator Pattern**: All updates are routed through a central `UpdateRouter`, which dispatches them to the appropriate handlers based on filters and priorities.
- **Filters as Attributes**: Handler classes are decorated with filter attributes that declaratively specify when the handler should run.
- **State Management**: Built-in mechanisms for managing user/chat state without external storage.
- **Concurrency Control**: Fine-grained control over how many handlers run in parallel, both globally and per-handler.

#### Handler Lifecycle
1. **Registration**: Handlers are registered with the bot during startup
2. **Discovery**: The framework automatically discovers handlers using reflection
3. **Filtering**: Updates are filtered to determine which handlers should run
4. **Execution**: Selected handlers are executed in order of priority
5. **Cleanup**: Resources are cleaned up after execution

#### Handler Priority & Importance
- **Importance**: Internal priority based on handler type (CommandHandler > MessageHandler > AnyUpdateHandler)
- **Priority**: User-defined global priority for execution order
- **Combined Order**: Importance first, then Priority

#### Implicit Handlers from Methods

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
[CommandAlias("menu")]
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
[CommandAlias("register")]
[State<UserState>(UserState.Start)]
private static async Task<Result> StartRegistration(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    StateStorage.GetStateMachine<UserState>().BySenderId().Advance();
    await container.Reply("Please enter your name:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle name input
[MessageHandler]
[State<UserState>(UserState.WaitingForName)]
private static async Task<Result> HandleName(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    var name = container.Input.Text;
    StateStorage.GetStateMachine<UserState>().BySenderId().Advance();
    await container.Reply($"Hello {name}! Please enter your age:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle age input
[MessageHandler]
[State<UserState>(UserState.WaitingForAge)]
private static async Task<Result> HandleAge(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    if (int.TryParse(container.Input.Text, out int age))
    {
        StateStorage.GetStateMachine<UserState>().BySenderId().Reset();
        await container.Reply($"Registration complete! Name: {name}, Age: {age}", cancellationToken: cancellationToken);
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

### 3.2. Filter System

Filters are the gatekeepers of your bot logic. They determine when handlers should be executed.

#### Filter Types
- **Text Filters**: `TextContains`, `TextStartsWith`, `RegexFilter`
- **User Filters**: `FromUserId`, `FromUser`, `FromUsername`
- **Chat Filters**: `ChatType`, `Mentioned`, `HasReply`
- **Command Filters**: `CommandAlias`
- **State Filters**: `EnumState`, `NumericState`, `StringState`

#### Filter Composition
- **AND Logic**: Multiple filters are combined with AND by default
- **OR Logic**: Use `FilterModifier.OrNext` to combine with OR
- **NOT Logic**: Use `FilterModifier.Not` to invert a filter
- **Combined Modifiers**: Use bitwise OR to combine modifiers

#### Custom Filters
You can create custom filters by inheriting from `FilterAnnotation<T>`:

```csharp
public class AdminOnlyAttribute : FilterAnnotation<Message>
{ 
    private readonly List<long> _adminIds = [];

    public void AddAdmin(long id)
        => _adminIds.Add(id);
    
    public override bool CanPass(FilterExecutionContext<Message> context)
        => _adminIds.Contains(context.Input.From?.Id);
}
```

#### Hosting Integration - Access to DI Container

When using Telegrator.Hosting, filters can access the DI container and configuration through `HostedTelegramBotInfo`:

```csharp
public class DatabaseUserFilterAttribute : FilterAnnotation<Message>
{
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        using (var scope = botInfo.Services.CreateScope())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var dbContext = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

            var telegramId = context.Input.From?.Id;
            if (telegramId == null)
                return false;

            var user = dbContext.Users.FirstOrDefault(u => u.TelegramId == telegramId);
            return user?.IsActive == true;
        }
    }
}

// Usage in handler
[MessageHandler]
[DatabaseUserFilter]
public class ActiveUserHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, active user!");
        return Ok;
    }
}
```

**Configuration-based Filter:**
```csharp
public class ConfigurableFilterAttribute : FilterAnnotation<Message>
{
    private readonly string _configKey;
    
    public ConfigurableFilterAttribute(string configKey)
    {
        _configKey = configKey;
    }
    
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        var configuration = botInfo.Services.GetRequiredService<IConfiguration>();
        var allowedUsers = configuration.GetSection(_configKey).Get<List<long>>() ?? [];
        
        return allowedUsers.Contains(context.Input.From?.Id ?? 0);
    }
}

// Usage
[MessageHandler]
[ConfigurableFilter("AllowedUsers")]
public class RestrictedHandler : MessageHandler
{
    // Handler implementation
}
```

**Key Points:**
- **Hosting Only**: This feature is only available when using `Telegrator.Hosting`
- **Type Casting**: Cast `context.BotInfo` to `HostedTelegramBotInfo`
- **Service Access**: Use `botInfo.Services.GetRequiredService<T>()` to access DI services
- **Configuration Access**: Use `botInfo.Services.GetRequiredService<IConfiguration>()` for settings
- **Null Safety**: Always check if the cast is successful before using services

### 3.3. State Management

Telegrator provides built-in state management for multi-step conversations (wizards, forms, quizzes) with or without a database.

> [!NOTE]
> Each type of `StateKeeper`'s keys and states are shared between **EVERY** handler in project.

**How to Use:**
1. Define your state (enum/int/string)
2. Use a state filter attribute on your handler:
   - `[State<MyEnum>(MyEnum.Step1)]`
3. Change state inside the handler using extension methods:
   - `StateStorage.GetStateMachine<MyEnum>().BySenderId().Current()`
   - `StateStorage.GetStateMachine<MyEnum>().BySenderId().Advance()`
   - `StateStorage.GetStateMachine<MyEnum>().BySenderId().Retreat()`
   - `StateStorage.GetStateMachine<MyEnum>().BySenderId().Reset()`

**Example:**
```csharp
public enum QuizState
{
    Start, Q1, Q2
}

[CommandHandler]
[CommandAlias("quiz")]
[State<QuizState>(QuizState.Start)]
public class StartQuizHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        StateStorage.GetStateMachine<QuizState>().BySenderId().Advance();
        await Reply("Quiz started! Question 1: What is the capital of France?");
        return Ok;
    }
}

[MessageHandler]
[State<QuizState>(QuizState.Q1)]
public class Q1Handler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input.Text.Trim().Equals("Paris", StringComparison.InvariantCultureIgnoreCase))
            await Reply("Correct!");
        else
            await Reply("Incorrect. The answer is Paris.");

        StateStorage.GetStateMachine<QuizState>().BySenderId().Advance();
        await Reply("Question 2: What is 2 + 2?");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Enum State Definition**: `QuizState` enum defines the conversation flow with `Start = SpecialState.NoState` indicating no initial state.
> 2. **State Filter**: `[State<QuizState>(QuizState.Start)]` ensures the handler only runs when the user is in the "Start" state.
> 3. **State Transition**: `StateStorage.GetStateMachine<QuizState>().BySenderId().Advance()` moves the user to the next state (Q1).
> 4. **Next Handler**: The `Q1Handler` will only run when the user is in state `QuizState.Q1`.
> 5. **State Management**: Each handler manages its own state transition, creating a clear conversation flow.

### 3.4. Update Routing

The `UpdateRouter` is the central component that manages how updates flow through your bot.

#### How Updates Are Processed
1. **Reception**: Updates are received from Telegram via long-polling or webhook
2. **Filtering**: Each registered handler is checked against the update using its filters
3. **Selection**: Handlers that pass all filters are selected for execution
4. **Prioritization**: Selected handlers are sorted by Importance and Priority
5. **Execution**: Handlers are executed in order, with aspects applied

#### Router Configuration
```csharp
var options = new TelegratorOptions
{
    MaximumParallelWorkingHandlers = 10,
    ExclusiveAwaitingHandlerRouting = true,
    ExceptIntersectingCommandAliases = true
};

var bot = new TelegratorClient("<BOT_TOKEN>", options);
```

#### Error Handling
The router includes built-in error handling:
- **Exception Handler**: Global exception handler for all errors
- **Handler Errors**: Individual handler errors are caught and logged
- **Recovery**: The router continues processing other handlers even if one fails

#### Performance Considerations
- **Parallel Execution**: Multiple handlers can run simultaneously
- **Concurrency Limits**: Control the number of parallel executions
- **Resource Management**: Automatic cleanup of resources after execution

---

## 4. Intermediate Topics

This section covers intermediate concepts that build upon the core concepts.

### 4.1. Advanced Filters

You can create custom filters by inheriting from `FilterAnnotation<T>`:

```csharp
using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Handlers;

public class AdminOnlyAttribute : FilterAnnotation<Message>
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
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, admin!");
        return Ok;
    }
}

// Usage
AdminOnlyAttribute.AddAdmin(123456789);
bot.StartReceiving();
```

> **How is it working?**
> 1. **Custom Filter**: `AdminOnlyAttribute` inherits from `FilterAnnotation<Message>` to create a reusable filter attribute.
> 2. **Filter Logic**: `CanPass()` method checks if the message sender's ID matches the admin ID.
> 3. **Usage**: The filter is applied as an attribute `[AdminOnly]` to restrict access to users that are not registered as admins.

### 4.2. Awaiting Mechanism

Use `AwaitingProvider` to wait for a user's next update (message or callback) inside a handler:

```csharp
[CommandHandler]
[CommandAlias("ask")]
public class AskHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");
        var nextMessage = await container.AwaitMessage().BySenderId(cancellation);
        await Reply($"Hello, {nextMessage.Text}!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Awaiting Provider**: `container.AwaitMessage()` creates a temporary handler that waits for the next message.
> 2. **Sender Filter**: `.BySenderId(cancellation)` ensures only messages from the same user are captured.
> 3. **Async Flow**: The handler pauses execution until the user responds, then continues with the conversation.
> 4. **Context Preservation**: The original handler context is maintained during the awaiting process.

### 4.3. Branching Handlers

For complex scenarios where a single handler needs to handle multiple different update types or conditions, you can use `BranchingUpdateHandler` to create handlers with multiple entry points.

**Example:**
```csharp
[MessageHandler]
public class ComplexHandler : BranchingMessageHandler
{
    [TextContains("hello")]
    public async Task<Result> HandleGreeting(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello there!");
        return Ok;
    }

    [TextContains("help")]
    public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("How can I help you?");
        return Ok;
    }

    [FromUser("John")]
    public async Task<Result> HandleAdmin(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Admin command received!");
        return Ok;
    }
}
```

**Branching Command Handler:**
```csharp
[CommandHandler]
public class SettingsHandler : BranchingCommandHandler
{
    [CommandAlias("settings")]
    public async Task<Result> ShowSettings(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Settings menu:");
        // Show settings options
        return Ok;
    }

    [CommandAlias("settings", "language")]
    public async Task<Result> SetLanguage(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var language = Arguments.FirstOrDefault();
        await Reply($"Language set to: {language}");
        return Ok;
    }

    [CommandAlias("settings", "theme")]
    public async Task<Result> SetTheme(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var theme = Arguments.FirstOrDefault();
        await Reply($"Theme set to: {theme}");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Multiple Entry Points**: Each method with filters becomes a separate handler entry point.
> 2. **Individual Filtering**: Each method can have its own set of filters and conditions.
> 3. **Shared Context**: All methods share the same handler instance and context.
> 4. **Automatic Registration**: Each method is automatically registered as a separate handler.
> 5. **Command Arguments**: In `BranchingCommandHandler`, you can access command arguments via the `Arguments` property.
> 6. **Flexible Routing**: Perfect for complex bots with many related commands or message patterns.

You can extend Telegrator by creating custom filters, attributes, and state keepers.

### 4.4. Advanced Hosting Integration

When using Telegrator.Hosting, you can create powerful filters that integrate with your application's services and configuration.

#### Database-Integrated Filters

```csharp
public class PremiumUserFilterAttribute : FilterAnnotation<Message>
{
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        var dbContext = botInfo.Services.GetRequiredService<ApplicationDbContext>();
        var user = dbContext.Users
            .FirstOrDefault(u => u.TelegramId == context.Input.From?.Id);
            
        return user?.SubscriptionLevel == SubscriptionLevel.Premium;
    }
}

// Usage
[MessageHandler]
[PremiumUserFilter]
public class PremiumFeatureHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome to premium features!");
        return Ok;
    }
}
```

#### Configuration-Driven Filters

```csharp
public class EnvironmentFilterAttribute : FilterAnnotation<Message>
{
    private readonly string _environment;
    
    public EnvironmentFilterAttribute(string environment)
    {
        _environment = environment;
    }
    
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        var configuration = botInfo.Services.GetRequiredService<IConfiguration>();
        var currentEnv = configuration["Environment"] ?? "Production";
        
        return currentEnv.Equals(_environment, StringComparison.OrdinalIgnoreCase);
    }
}

// Usage
[MessageHandler]
[EnvironmentFilter("Development")]
public class DevOnlyHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This feature is only available in development!");
        return Ok;
    }
}
```

#### Multi-Service Integration

```csharp
public class RateLimitFilterAttribute : FilterAnnotation<Message>
{
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        var cache = botInfo.Services.GetRequiredService<IDistributedCache>();
        var configuration = botInfo.Services.GetRequiredService<IConfiguration>();
        var userId = context.Input.From?.Id.ToString();
        
        if (string.IsNullOrEmpty(userId))
            return false;

        var cacheKey = $"rate_limit:{userId}";
        var currentCount = cache.GetString(cacheKey);
        
        if (int.TryParse(currentCount, out int count) && count >= 10)
            return false;

        cache.SetString(cacheKey, (count + 1).ToString(), 
            new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) });
            
        return true;
    }
}

// Usage
[MessageHandler]
[RateLimitFilter]
public class RateLimitedHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message processed!");
        return Ok;
    }
}
```

#### 2.6.1. Custom Filter Attributes

**Custom Filter Attribute Example:**
```csharp
using Telegram.Bot.Types;
using Telegrator.Attributes;
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
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, admin!");
        return Ok;
    }
}

// ...
AdminOnlyAttribute.AddAdmin(123456789);
bot.StartReceiving();
```

> **How is it working?**
> 1. **Custom Filter**: `AdminOnlyAttribute` inherits from `FilterAnnotation<Message>` to create a reusable filter attribute.
> 2. **Filter Logic**: `CanPass()` method checks if the message sender's ID matches the admin ID.
> 3. **Usage**: The filter is applied as an attribute `[AdminOnly]` to restrict access to users that are not registered as admins.

#### 2.6.2. Branching Handlers

For complex scenarios where a single handler needs to handle multiple different update types or conditions, you can use `BranchingUpdateHandler` to create handlers with multiple entry points.

**Example:**
```csharp
[MessageHandler]
public class ComplexHandler : BranchingMessageHandler
{
    [TextContains("hello")]
    public async Task<Result> HandleGreeting(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello there!");
        return Ok;
    }

    [TextContains("help")]
    public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("How can I help you?");
        return Ok;
    }

    [FromUser("John")]
    public async Task<Result> HandleAdmin(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Admin command received!");
        return Ok;
    }
}
```

**Branching Command Handler:**
```csharp
[CommandHandler]
public class SettingsHandler : BranchingCommandHandler
{
    [CommandAlias("settings")]
    public async Task<Result> ShowSettings(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Settings menu:");
        // Show settings options
        return Ok;
    }

    [CommandAlias("settings", "language")]
    public async Task<Result> SetLanguage(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var language = Arguments.FirstOrDefault();
        await Reply($"Language set to: {language}");
        return Ok;
    }

    [CommandAlias("settings", "theme")]
    public async Task<Result> SetTheme(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var theme = Arguments.FirstOrDefault();
        await Reply($"Theme set to: {theme}");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Multiple Entry Points**: Each method with filters becomes a separate handler entry point.
> 2. **Individual Filtering**: Each method can have its own set of filters and conditions.
> 3. **Shared Context**: All methods share the same handler instance and context.
> 4. **Automatic Registration**: Each method is automatically registered as a separate handler.
> 5. **Command Arguments**: In `BranchingCommandHandler`, you can access command arguments via the `Arguments` property.
> 6. **Flexible Routing**: Perfect for complex bots with many related commands or message patterns.

#### 2.6.3. Custom Aspects

You can create custom aspects by implementing `IPreProcessor` or `IPostProcessor` interfaces to add cross-cutting concerns to your handlers.

**Creating a Custom Pre-Processor:**
```csharp
using Telegrator.Aspects;

public class RateLimitProcessor : IPreProcessor
{
    private readonly Dictionary<long, DateTime> _lastExecution = new();
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5);

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var userId = container.HandlingUpdate.Message?.From?.Id;
        if (userId == null) return Ok;

        if (_lastExecution.TryGetValue(userId.Value, out var lastExec))
        {
            if (DateTime.Now - lastExec < _cooldown)
            {
                return Result.Fault(); // Stop execution - rate limit exceeded
            }
        }

        _lastExecution[userId.Value] = DateTime.Now;
        return Ok;
    }
}
```

**Creating a Custom Post-Processor:**
```csharp
using Telegrator.Aspects;

public class MetricsProcessor : IPostProcessor
{
    private int _totalExecutions = 0;
    private readonly object _lock = new();

    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        lock (_lock)
        {
            _totalExecutions++;
        }
        
        Console.WriteLine($"Total handler executions: {_totalExecutions}");
        return Ok;
    }
}
```

**Applying Custom Aspects:**
```csharp
[MessageHandler]
[BeforeExecution<RateLimitProcessor>]
[AfterExecution<MetricsProcessor>]
public class RateLimitedHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message processed!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Custom Processors**: Implement `IPreProcessor` or `IPostProcessor` to create reusable aspects
> 2. **Flow Control**: Return `Result.Fault()` from pre-processors to stop handler execution
> 3. **State Management**: Processors can maintain their own state for rate limiting, metrics, etc.
> 4. **Reusability**: Custom aspects can be applied to multiple handlers via attributes
> 5. **Separation of Concerns**: Business logic remains separate from cross-cutting concerns

### 2.7. Integration

Telegrator works in console, hosted applications, and ASP.NET Core (webhook) projects.

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

**Web Hosting Example:**
```csharp
using Telegrator.Hosting.Web;

var webOptions = new TelegramBotWebOptions();

var webHost = TelegramBotWebHost.CreateBuilder(webOptions);
webHost.Handlers.AddHandler<StartHandler>();
var host = webHost.Build();
await host.StartAsync();
```

**Note:** For web hosting, you need to configure both `TelegratorOptions` (for bot token) and `WebhookerOptions` (for webhook settings) in your `appsettings.json` file.

> **How is it working?**
> 1. **Console Integration**: `TelegratorClient` provides a simple way to create bots in console applications.
> 2. **Domain-Wide Collection**: `CollectHandlersDomainWide()` automatically discovers and registers all handlers in the current assembly.
> 3. **ASP.NET Core Integration**: `TelegramBotHost.CreateBuilder()` provides a builder pattern for hosting bots in ASP.NET Core applications.
> 4. **Webhook Integration**: `TelegramBotWebHost.CreateBuilder()` provides webhook hosting for production deployments.
> 5. **Dependency Injection**: Handlers and their dependencies are automatically registered with the DI container.

### 2.8. Aspects & Cross-Cutting Concerns

Telegrator provides a powerful aspect-oriented programming (AOP) system for handling cross-cutting concerns like logging, validation, authorization, and error handling. This system allows you to separate business logic from infrastructure concerns.

**Key Concepts:**
- **Pre-Execution Aspects**: Code that runs before handler execution
- **Post-Execution Aspects**: Code that runs after handler execution
- **Self-Processing**: Handler implements interfaces directly
- **Typed Processing**: External processor classes via attributes

**Common Use Cases:**
- Input validation
- Logging and monitoring
- Authorization and access control
- Error handling and recovery
- Performance metrics collection
- Audit trails

#### Self-Processing Example

```csharp
using Telegrator.Aspects;

[MessageHandler]
public class LoggingHandler : MessageHandler, IPreProcessor, IPostProcessor
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message processed successfully!");
        return Ok;
    }

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var user = container.HandlingUpdate.Message?.From;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User {user?.Id} ({user?.Username}) sent: {container.HandlingUpdate.Message?.Text}");
        return Ok;
    }

    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Message processing completed");
        return Ok;
    }
}
```

#### Typed Processing Example

```csharp
using Telegrator.Aspects;

// Validation processor
public class MessageValidationProcessor : IPreProcessor
{
    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var message = container.HandlingUpdate.Message;
        
        if (message?.Text == null)
            return Result.Fault(); // Stop execution
            
        if (message.Text.Length > 1000)
            return Result.Fault(); // Message too long
            
        return Ok;
    }
}

// Logging processor
public class LoggingProcessor : IPostProcessor
{
    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"Handler execution completed for update {container.HandlingUpdate.Id}");
        return Ok;
    }
}

// Handler with external processors
[MessageHandler]
[BeforeExecution<MessageValidationProcessor>]
[AfterExecution<LoggingProcessor>]
public class ValidatedHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Valid message received and processed!");
        return Ok;
    }
}
```

#### Combined Approach Example

```csharp
using Telegrator.Aspects;

[MessageHandler]
[BeforeExecution<AuthorizationProcessor>]
public class SecureHandler : MessageHandler, IPostProcessor
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Secure operation completed!");
        return Ok;
    }

    // Custom post-processing
    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"Secure operation completed for user {container.HandlingUpdate.Message?.From?.Id}");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Self-Processing**: Handlers implement `IPreProcessor` and/or `IPostProcessor` interfaces directly
> 2. **Typed Processing**: External processor classes are applied via `[BeforeExecution<T>]` and `[AfterExecution<T>]` attributes
> 3. **Execution Order**: Pre-execution aspects run first, then handler main logic, then post-execution aspects
> 4. **Flow Control**: Return `Result.Fault()` from pre-execution to stop handler execution
> 5. **Separation of Concerns**: Business logic is separated from cross-cutting concerns like logging and validation

---

## 3. Step-by-Step Tutorials

### 3.1. Minimal Bot Creation
See [Practice: Minimal Bot](#practice-minimal-bot).

### 3.2. Command Filtering
Message is considered command if is start with '/' and has not null or empty name.
Instead of using the `MessageHandler` for command (such as `/start`) you should use `CommandHandler`.

```csharp
[CommandHandler]
[CommandAlias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Use /help to see available commands.");
        return Ok;
    }
}

[CommandHandler]
[CommandAlias("help")]
public class HelpHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Available commands:\n/start - Start the bot\n/help - Show this help");
        return Ok;
    }
}

[MessageHandler]
[TextStartsWith("/", Modifiers = FilterModifier.Not)]
public class EchoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"You said: \"{Input.Text}\"");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Command Handlers**: `[CommandHandler]` and `[CommandAlias]` work together to handle specific commands like `/start` and `/help`.
> 2. **Echo Handler**: `[TextStartsWith("/", Modifiers = FilterModifier.Not)]` catches all messages that don't start with "/" (non-commands).
> 3. **Handler Separation**: Each command has its own dedicated handler, making the code modular and maintainable.
> 4. **Filter Modifiers**: The `Not` modifier inverts the filter logic to exclude command messages.

### 3.3. State Management Wizard
```csharp
[CommandHandler]
[CommandAlias("wizard")]
[NumericState(SpecialState.NoState)]
public class StartWizardHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.CreateNumericState(); // This code is not necessary, as "Forward" method can automatically creates state if needed, but its recomended to use
        container.ForwardNumericState();
        await Reply("What is your name?");
        return Ok;
    }
}

[MessageHandler]
[NumericState(1)]
public class NameHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.ForwardNumericState();
        await Reply($"Nice to meet you, {Input.Text}! How old are you?");
        return Ok;
    }
}

[MessageHandler]
[NumericState(2)]
public class AgeHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
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
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Numeric State**: `[NumericState(SpecialState.NoState)]` starts the wizard when no state exists.
> 2. **State Creation**: `container.CreateNumericState()` initializes the numeric state for the user.
> 3. **State Progression**: `container.ForwardNumericState()` moves to the next step (1, then 2).
> 4. **State Cleanup**: `container.DeleteNumericState()` removes the state when the wizard completes.
> 5. **Input Validation**: The age handler validates numeric input and provides feedback.

### 3.4. CallbackQuery Handling
```csharp
[CommandHandler]
[CommandAlias("menu")]
public class MenuHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Option 1", "option1"),
            InlineKeyboardButton.WithCallbackData("Option 2", "option2")
        });

        await Reply("Choose an option:", replyMarkup: keyboard, cancellationToken: cancellation);
        return Ok;
    }
}

[CallbackQueryHandler]
[CallbackData("option1")]
public class Option1Handler : CallbackQueryHandler
{
    public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 1!", cancellationToken: cancellation);
        await EditMessage("You selected Option 1!");
        return Ok;
    }
}

[CallbackQueryHandler]
[CallbackData("option2")]
public class Option2Handler : CallbackQueryHandler
{
    public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 2!", cancellationToken: cancellation);
        await EditMessage("You selected Option 2!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Inline Keyboard**: `InlineKeyboardMarkup` creates interactive buttons with `CallbackData` identifiers.
> 2. **CallbackQuery Handlers**: `[CallbackQueryHandler]` and `[CallbackData]` work together to handle button clicks.
> 3. **Response Methods**: `AnswerCallbackQuery()` provides immediate feedback, while `EditMessage()` updates the message.
> 4. **Handler Separation**: Each button option has its own dedicated handler for clean code organization.

### 3.5. Implicit Handlers from Methods

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
[CommandAlias("menu")]
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
    Start = SpecialState.NoState,
    WaitingForName,
    WaitingForAge
}

// Start conversation
[CommandHandler]
[CommandAlias("register")]
[State<UserState>(UserState.Start)]
private static async Task<Result> StartRegistration(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    StateStorage.GetStateMachine<UserState>().BySenderId().Advance();
    await container.Reply("Please enter your name:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle name input
[MessageHandler]
[State<UserState>(UserState.WaitingForName)]
private static async Task<Result> HandleName(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    var name = container.Input.Text;
    StateStorage.GetStateMachine<UserState>().BySenderId().Advance();
    await container.Reply($"Hello {name}! Please enter your age:", cancellationToken: cancellationToken);
    return Ok;
}

// Handle age input
[MessageHandler]
[State<UserState>(UserState.WaitingForAge)]
private static async Task<Result> HandleAge(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    if (int.TryParse(container.Input.Text, out int age))
    {
        container.DeleteEnumState<UserState>();
        await container.Reply($"Registration complete! Name: {name}, Age: {age}", cancellationToken: cancellationToken);
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

---

## 4. Advanced Topics

### 4.1. Handler Importance & Priority

Telegrator provides two different mechanisms for controlling handler execution order: `Importance` and `Priority`. These serve different purposes in the framework's execution model.

#### Importance (Internal Type Priority)
`Importance` is an internal parameter used to control priority between different handler types that process the same update type. It's automatically set by the framework based on the handler type.

**Built-in Importance Values:**
- `CommandHandler`: Importance = 1 (highest priority for message updates)
- `MessageHandler`: Importance = 0 (default priority for message updates)
- `CallbackQueryHandler`: Importance = 0 (default for callback updates)
- `AnyUpdateHandler`: Importance = -1 (lowest priority, catches all updates)

**Example of Importance in Action:**
```csharp
[CommandHandler] // Importance = 1 (automatically set)
[CommandAlias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Command handler executed first!");
        return Ok;
    }
}

[MessageHandler] // Importance = 0 (automatically set)
[TextContains("hello")]
public class HelloHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message handler executed second!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Automatic Importance**: The framework automatically sets importance based on handler type.
> 2. **Command Priority**: Commands are processed before regular messages due to higher importance.
> 3. **Type-based Ordering**: This ensures critical handlers (like commands) run before general handlers.
> 4. **Framework Control**: Importance is managed internally and shouldn't be manually overridden.

#### Priority (Global Execution Control)
`Priority` is a user-controlled parameter that regulates the execution order among all registered handlers in the application, regardless of their type.

**Priority Usage:**
```csharp
[MessageHandler(Priority = 10)] // High priority among all handlers
public class HighPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with high priority!");
        return Ok;
    }
}

[MessageHandler(Priority = 0)] // Default priority
public class NormalPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with normal priority!");
        return Ok;
    }
}

[MessageHandler(Priority = -10)] // Low priority
public class LowPriorityHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("This handler runs with low priority!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Global Priority**: `Priority` controls execution order across all handler types.
> 2. **Higher Priority First**: Handlers with higher priority numbers run before those with lower numbers.
> 3. **User Control**: Priority is manually set by developers for custom execution ordering.
> 4. **Default Priority**: When not specified, handlers have priority 0.
> 5. **Cross-Type Ordering**: Priority works across different handler types (MessageHandler, CommandHandler, etc.).

#### Combined Execution Order
The final execution order is determined by both Importance and Priority:

1. **First**: Handlers are sorted by `Importance` (type-based priority)
2. **Second**: Within the same importance level, handlers are sorted by `Priority` (user-defined priority)

**Example:**
```csharp
[CommandHandler(Priority = 5)] // Importance = 1, Priority = 5
[CommandAlias("admin")]
public class AdminCommandHandler : CommandHandler
{
    // Executes first (highest importance + high priority)
}

[CommandHandler(Priority = 0)] // Importance = 1, Priority = 0  
[CommandAlias("start")]
public class StartCommandHandler : CommandHandler
{
    // Executes second (highest importance + normal priority)
}

[MessageHandler(Priority = 10)] // Importance = 0, Priority = 10
[TextContains("urgent")]
public class UrgentMessageHandler : MessageHandler
{
    // Executes third (normal importance + high priority)
}

[MessageHandler(Priority = 0)] // Importance = 0, Priority = 0
[TextContains("hello")]
public class HelloMessageHandler : MessageHandler
{
    // Executes last (normal importance + normal priority)
}
```

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
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        _logger.LogInformation("MyHandler executed!");
        var result = _myService.DoSomething();
        await Reply(result);
        return Ok;
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

**Note:** For automatic handler discovery, you can use `bot.Handlers.CollectHandlersDomainWide()` instead of manually adding each handler with `AddHandler<T>()`. This automatically discovers and registers all handlers in the current assembly.
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

### 4.6. Web Hosting (Webhook)
Telegrator provides webhook hosting through the `Telegrator.Hosting.Web` package for production deployments.

**Installation:**
```shell
dotnet add package Telegrator.Hosting.Web
```

**Dependencies:**
- `Microsoft.AspNetCore.App` - ASP.NET Core
- `Microsoft.Extensions.Hosting` - .NET Generic Host
- `Microsoft.Extensions.DependencyInjection` - Dependency Injection

**Core Components:**
- `TelegramBotWebHost` - The main web hosted service for webhook handling
- `TelegramBotWebHostBuilder` - Builder pattern for configuring the web host
- `WebhookerOptions` - Configuration options for webhook settings

**Configuration Requirements:**
- `TelegratorOptions` must be configured as it contains the bot token
- `WebhookerOptions` must be configured through external sources (appsettings.json) for webhook settings

**Basic Example:**
```csharp
var webOptions = new TelegramBotWebOptions();

var builder = TelegramBotWebHost.CreateBuilder(webOptions);

// Configure services
builder.Services.AddSingleton<IMyService, MyService>();

// Configure handlers
builder.Handlers.CollectHandlersDomainWide();

// Building host
var host = builder.Build();
await host.StartAsync();
```

**Configuration via appsettings.json:**
```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN"
  },
  
  "WebhookerOptions": {
    "WebhookUri": "https://your-domain.com/webhook",
    "SecretToken": "your-secret-token",
    "MaxConnections": 40,
    "DropPendingUpdates": true
  },
  
  "HostOptions": {
    "ShutdownTimeout": 10,
    "BackgroundServiceExceptionBehavior": "StopHost"
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Telegrator": "Debug"
    }
  }
}
```

> **How is it working?**
> 1. **Webhook Integration**: `TelegramBotWebHost` handles incoming webhook requests from Telegram.
> 2. **Security**: Supports secret token validation for secure webhook handling.
> 3. **Scalability**: Webhook hosting is more efficient for high-traffic bots compared to long-polling.
> 4. **Production Ready**: Includes health checks, logging, and monitoring capabilities.
> 5. **SSL Required**: Webhook hosting requires HTTPS for production use.

### 4.7. Configuration Management
The hosting integration provides comprehensive configuration management through standard .NET configuration patterns.

**Required Configuration:**
The bot token must be configured either in `appsettings.json` or through environment variables:

```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN"
  }
}
```

**Environment Variables:**
You can also use environment variables for sensitive configuration:
```shell
export TelegramBotClientOptions__Token="YOUR_BOT_TOKEN"
```

**Advanced Configuration:**
```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN",
    "BaseUrl": "https://api.telegram.org"
  },
  
  "HostOptions": {
    "ShutdownTimeout": 10,
    "BackgroundServiceExceptionBehavior": "StopHost"
  },

  "ReceiverOptions": {
    "DropPendingUpdates": true,
    "Limit": 10,
    "Timeout": 30
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

> **How is it working?**
> 1. **Automatic Configuration Binding**: The hosting integration automatically binds configuration sections to their respective options classes.
> 2. **Environment Variable Support**: Configuration can be overridden using environment variables with the `__` separator.
> 3. **Hierarchical Configuration**: Supports multiple configuration sources (appsettings.json, environment variables, command line, etc.).
> 4. **Type Safety**: Configuration is strongly typed and validated at startup.
> 5. **Production Ready**: Sensitive data like bot tokens can be securely managed through environment variables or secret management systems.

### 4.8. Error Handling and Logging
You can subscribe to error events or set a custom exception handler:

```csharp
bot.UpdateRouter.ExceptionHandler = new DefaultRouterExceptionHandler((client, exception, source, cancellationToken) =>
{
    Console.WriteLine($"An error occurred: {exception.Message}");
    return Task.CompletedTask;
});
```

**Custom Exception Handler:**
```csharp
public class CustomExceptionHandler : IRouterExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleException(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error occurred in {Source}", source);
        
        // You can implement custom error handling logic here
        // For example, send error notifications to administrators
        
        return Task.CompletedTask;
    }
}
```

> **How is it working?**
> 1. **Exception Handler**: `ExceptionHandler` property allows you to set a custom exception handler for the entire bot.
> 2. **Error Context**: The handler receives the bot client, exception, source information, and cancellation token.
> 3. **Global Error Handling**: This provides a centralized way to handle all exceptions that occur during update processing.
> 4. **Logging Integration**: Perfect place to log errors or send notifications to administrators.

### 4.9. Performance Optimization
- Use appropriate concurrency limits for resource-intensive operations
- Avoid thread-blocking operations in handlers
- Use state management for multi-step processes
- Use `AwaitingProvider` for complex conversation flows
- Consider webhook hosting for high-traffic bots

### 4.10. Best Practices
- Organize handlers, filters, and state keepers in separate folders
- Use feature modules for large bots
- Prefer declarative filters over manual `if` statements
- Keep handlers focused and single-responsibility
- Use dependency injection for better testability
- Implement proper error handling and logging
- Use webhook hosting for production deployments

**Aspect-Oriented Programming Best Practices:**
- **Separation of Concerns**: Keep aspects focused on a single responsibility (logging, validation, authorization, etc.)
- **Reusability**: Create generic aspects that can be applied to multiple handlers
- **Performance**: Avoid heavy operations in aspects that run frequently
- **Error Handling**: Always handle exceptions in aspects gracefully
- **Testing**: Test aspects independently from handlers
- **Documentation**: Document the purpose and behavior of custom aspects
- **State Management**: Use thread-safe collections for aspects that maintain state

**Common Aspect Patterns:**
- **Validation Aspect**: Check input data before processing
- **Logging Aspect**: Record execution details for debugging
- **Authorization Aspect**: Verify user permissions
- **Metrics Aspect**: Collect performance and usage data
- **Audit Aspect**: Track user actions for compliance
- **Rate Limiting Aspect**: Prevent abuse by limiting request frequency

---

## 5. Advanced Topics

This section covers advanced concepts and techniques for building sophisticated bots.

### 5.1. Aspects & Cross-Cutting Concerns

Telegrator provides a powerful aspect-oriented programming (AOP) system for handling cross-cutting concerns like logging, validation, authorization, and error handling.

#### Self-Processing Example

```csharp
using Telegrator.Aspects;

[MessageHandler]
public class LoggingHandler : MessageHandler, IPreProcessor, IPostProcessor
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Message processed successfully!");
        return Ok;
    }

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var user = container.HandlingUpdate.Message?.From;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] User {user?.Id} ({user?.Username}) sent: {container.HandlingUpdate.Message?.Text}");
        return Ok;
    }

    public async Task<Result> AfterExecution(IHandlerContainer container)
    {
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Message processing completed");
        return Ok;
    }
}
```

#### Typed Processing Example

```csharp
using Telegrator.Aspects;

// Validation processor
public class MessageValidationProcessor : IPreProcessor
{
    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var message = container.HandlingUpdate.Message;
        
        if (message?.Text == null)
            return Result.Fault(); // Stop execution
            
        if (message.Text.Length > 1000)
            return Result.Fault(); // Message too long
            
        return Ok;
    }
}

// Handler with external processors
[MessageHandler]
[BeforeExecution<MessageValidationProcessor>]
public class ValidatedHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Valid message received and processed!");
        return Ok;
    }
}
```

### 5.2. Custom Extensions

You can extend Telegrator by creating custom filters, aspects, and state keepers.

#### Custom Filters
```csharp
public class RateLimitFilter : FilterAnnotation<Message>
{
    private readonly Dictionary<long, DateTime> _lastExecution = new();
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5);

    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        var userId = context.Input.From?.Id;
        if (userId == null) return true;

        if (_lastExecution.TryGetValue(userId.Value, out var lastExec))
        {
            if (DateTime.Now - lastExec < _cooldown)
                return false; // Rate limit exceeded
        }

        _lastExecution[userId.Value] = DateTime.Now;
        return true;
    }
}
```

#### Custom State Keepers
```csharp
public class CustomStateKeeper : StateKeeperBase<string, string>
{
    protected override string GetKey(IHandlerContainer container)
        => container.HandlingUpdate.Message?.From?.Id.ToString() ?? "unknown";

    protected override string GetValue(IHandlerContainer container)
        => container.HandlingUpdate.Message?.Text ?? "";

    protected override void SetValue(IHandlerContainer container, string value)
    {
        // Custom state storage logic
    }
}
```

### 5.3. Performance Optimization

Optimize your bot for high performance:

#### Concurrency Settings
```csharp
var options = new TelegratorOptions
{
    MaximumParallelWorkingHandlers = 20,  // Adjust based on server capabilities
    ExclusiveAwaitingHandlerRouting = true,
    ExceptIntersectingCommandAliases = true
};
```

#### Memory Management
- Use `using` statements for disposable resources
- Implement proper cleanup in custom aspects
- Monitor memory usage in long-running bots

#### Caching Strategies
```csharp
public class CachingAspect : IPreProcessor
{
    private readonly Dictionary<string, object> _cache = new();

    public async Task<Result> BeforeExecution(IHandlerContainer container)
    {
        var key = container.HandlingUpdate.Message?.Text;
        if (_cache.TryGetValue(key, out var cached))
        {
            // Use cached result
            return Ok;
        }
        return Ok;
    }
}
```

### 5.4. Error Handling

Implement robust error handling for your bot:

#### Global Exception Handler
```csharp
var bot = new TelegratorClient("<BOT_TOKEN>");
bot.ExceptionHandler = new CustomExceptionHandler();

public class CustomExceptionHandler : IRouterExceptionHandler
{
    public Task HandleException(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Error in {source}: {exception.Message}");
        return Task.CompletedTask;
    }
}
```

#### Handler-Level Error Handling
```csharp
[MessageHandler]
public class SafeHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        try
        {
            // Risky operation
            await Reply("Operation completed!");
            return Ok;
        }
        catch (Exception ex)
        {
            await Reply("Sorry, something went wrong.");
            return Result.Fault();
        }
    }
}
```

---

## 6. Integration & Deployment

### 6.1. Console Applications

Simple console bot setup:

```csharp
class Program
{
    static void Main(string[] args)
    {
        var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
        bot.Handlers.CollectHandlersDomainWide();
        bot.StartReceiving();
        Console.ReadLine();
    }
}
```

### 6.2. ASP.NET Core Hosting

Host your bot in ASP.NET Core applications:

```csharp
using Telegrator.Hosting;

var builder = TelegramBotHost.CreateBuilder();
builder.Handlers.AddHandler<StartHandler>();
var host = builder.Build();
await host.StartAsync();
```

### 6.3. Webhook Deployment

Deploy your bot using webhooks for production:

```csharp
using Telegrator.Hosting.Web;

var webOptions = new TelegramBotWebOptions();
var webHost = TelegramBotWebHost.CreateBuilder(webOptions);
webHost.Handlers.AddHandler<StartHandler>();
var host = webHost.Build();
await host.StartAsync();
```

**Configuration (appsettings.json):**
```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN"
  },

  "WebhookerOptions": {
    "WebhookUri": "https://your-domain.com/webhook",
    "SecretToken": "your-secret-token",
    "MaxConnections": 40,
    "DropPendingUpdates": true
  }
}
```

### 6.4. Configuration Management

Manage your bot configuration:

```csharp
// From appsettings.json
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var botOptions = configuration.GetSection("TelegratorOptions").Get<TelegratorOptions>();
var bot = new TelegratorClient("<BOT_TOKEN>", botOptions);
```

---

## 7. Best Practices & Patterns

### 7.1. Project Organization

Organize your bot code effectively:

```
MyBot/
├── Handlers/
│   ├── Commands/
│   │   ├── StartHandler.cs
│   │   └── HelpHandler.cs
│   ├── Messages/
│   │   ├── EchoHandler.cs
│   │   └── GreetingHandler.cs
│   └── Callbacks/
│       └── ButtonHandler.cs
├── Filters/
│   ├── AdminFilter.cs
│   └── RateLimitFilter.cs
├── Aspects/
│   ├── LoggingAspect.cs
│   └── ValidationAspect.cs
├── State/
│   └── QuizState.cs
└── Program.cs
```

### 7.2. Testing Strategies

Test your bot components:

```csharp
[Test]
public async Task StartHandler_ShouldReplyWithWelcome()
{
    // Arrange
    var handler = new StartHandler();
    var container = CreateMockContainer();
    
    // Act
    var result = await handler.Execute(container, CancellationToken.None);
    
    // Assert
    Assert.That(result.Positive, Is.True);
}
```

### 7.3. Common Patterns

#### Command Pattern
```csharp
[CommandHandler]
[CommandAlias("settings")]
public class SettingsHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { InlineKeyboardButton.WithCallbackData("Language", "settings_lang") },
            new[] { InlineKeyboardButton.WithCallbackData("Theme", "settings_theme") }
        });
        
        await Reply("Choose a setting:", replyMarkup: keyboard);
        return Ok;
    }
}
```

#### Wizard Pattern
```csharp
[CommandHandler]
[CommandAlias("wizard")]
[StringState("no_state")]
public class StartWizardHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.ForwardStringState();
        await Reply("Step 1: What is your name?");
        return Ok;
    }
}
```

### 7.4. Performance Tips

- **Use webhooks** for production bots
- **Implement caching** for frequently accessed data
- **Limit concurrent executions** based on server capabilities
- **Use async/await** properly throughout your code
- **Monitor memory usage** in long-running bots
- **Implement proper error handling** to prevent crashes

### 7.5. Logging System

Telegrator provides a centralized logging system called "TelegratorLogging" that allows integration with various logging frameworks while maintaining zero dependencies in the core library.

#### Overview

The logging system consists of:
- **TelegratorLogging** - Centralized static logging system
- **ITelegratorLogger** - Core logging interface
- **NullLogger** - No-op logger
- **ConsoleLogger** - Simple console output
- **MicrosoftLoggingAdapter** - Integration with Microsoft.Extensions.Logging

#### Basic Usage

**Console Logging:**
```csharp
using Telegrator.Logging;

// Add console adapter
TelegratorLogging.AddAdapter(new ConsoleLoggerAdapter(LogLevel.Debug, includeTimestamp: true));

// Use logging
TelegratorLogging.LogInformation("Bot started");
TelegratorLogging.LogError("Something went wrong", exception);
```

**Custom Logger Adapter:**
```csharp
public class CustomLogger : ITelegratorLogger
{
    public void Log(LogLevel level, string message, Exception? exception = null)
    {
        // Your logging implementation
        Console.WriteLine($"[{level}] {message}");
        if (exception != null)
            Console.WriteLine($"Exception: {exception.Message}");
    }
}

// Add custom adapter
TelegratorLogging.AddAdapter(new CustomLogger());
```

#### Hosting Integration

**With Microsoft.Extensions.Logging:**
```csharp
using Telegrator.Hosting.Logging;

var loggerFactory = LoggerFactory.Create(builder => 
{
    builder.AddConsole();
    builder.AddDebug();
});

// Add Microsoft.Extensions.Logging adapter
ILogger<Program> logger = loggerFactory.CreateLogger<Program>();
MicrosoftLoggingAdapter adapter = new MicrosoftLoggingAdapter(logger);
TelegratorLogging.AddAdapter(adapter);

var bot = new TelegratorClient("<BOT_TOKEN>");
```

#### Log Levels

- **Trace** - Most detailed logging
- **Debug** - Detailed debugging information  
- **Information** - General information
- **Warning** - Warning messages
- **Error** - Error messages

#### Simple Logging

All logging methods support simple message logging:

```csharp
// In your handlers or aspects
TelegratorLogging.LogInformation("Handler executed");
TelegratorLogging.LogError("Something went wrong", exception);
TelegratorLogging.LogWarning("User exceeded rate limit");
```

#### Performance Considerations

- **TelegratorLogging** has minimal overhead with thread-safe adapter management
- **NullLogger** has zero overhead
- **ConsoleLogger** is lightweight for development
- **MicrosoftLoggingAdapter** delegates to the underlying framework
- Simple interface reduces complexity and improves performance

#### Best Practices

1. **Configure adapters early** in your application startup
2. **Use ConsoleLogger for development** and debugging
3. **Use MicrosoftLoggingAdapter for ASP.NET Core** applications
4. **Include relevant context** in log messages
5. **Set appropriate log levels** based on your needs
6. **Multiple adapters** can be registered for different outputs

---

## 8. FAQ & Troubleshooting

### 8.1. Common Issues

### 8.1. Q: My handler is not being triggered. What should I do?
- Check handler registration (use `bot.Handlers.AddHandler<MyHandler>()` or domain-wide collection)
- Check filter attributes and update types
- Enable debug logging
- Verify that the handler class inherits from the correct base class

### 8.2. Q: How can I access the `ITelegramBotClient` or the original `Update` object inside a handler?
- Use `Client`, `Update`, and `Input` properties in your handlers container

### 8.3. Q: How do I handle errors?
- Set a custom exception handler or subscribe to error events
- Use try-catch blocks in individual handlers for specific error handling
- Implement proper logging for debugging

### 8.4. Q: How can I organize my code for a large bot?
- Use folders, feature modules, and namespaces
- Keep handlers focused and modular
- Use dependency injection for better separation of concerns
- Implement proper state management for complex flows

### 8.5. Q: What's the difference between `Reply()` and `Responce()` methods?
- `Reply()` sends a reply to the original message (with reply markup)
- `Responce()` sends a new message to the chat (without reply markup)
- Both methods are available in `MessageHandler` and `CallbackQueryHandler`

**Note:** `Responce()` has a typo in the name but is intentionally kept for backward compatibility. Both methods serve different purposes in message handling.

### 8.6. Q: How do I implement webhook hosting?
- Use `Telegrator.Hosting.Web` package
- Configure `TelegramBotWebOptions` with your bot token and webhook URL
- Ensure your server has HTTPS enabled
- Set up proper SSL certificates for production use

### 8.7. Q: How can I add logging or validation to all handlers?
**A:** Use the aspect system with `IPreProcessor` and `IPostProcessor` interfaces:

- **Self-processing**: Implement interfaces directly in your handler
- **Typed processing**: Create external processor classes and apply with attributes
- **Combined approach**: Use both methods together

This allows you to implement cross-cutting concerns without modifying handler business logic.

### 8.8. Q: Can I stop handler execution from an aspect?
**A:** Yes! Return `Result.Fault()` from a pre-execution processor to stop handler execution. Return `Ok` to continue.

### 8.9. Q: What's the execution order of aspects?
**A:** Aspects execute in the following order:
1. Pre-execution aspects (self-processing first, then typed)
2. Handler main execution
3. Post-execution aspects (self-processing first, then typed)

### 8.10. Q: How do I create reusable aspects for multiple handlers?
**A:** Create external processor classes that implement `IPreProcessor` or `IPostProcessor`, then apply them using `[BeforeExecution<T>]` or `[AfterExecution<T>]` attributes. This allows you to share the same aspect logic across multiple handlers.

### 8.11. Q: Can I create handlers from methods instead of full classes?
**A:** Yes! You can create implicit handlers from methods using the same attributes and patterns as regular handlers:

```csharp
[MessageHandler, TextEquals("Hello", StringComparison.InvariantCultureIgnoreCase)]
private static async Task<Result> HelloWorld(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    await container.Reply("Hello, World!", cancellationToken: cancellationToken);
    return Ok;
}

// Register the method as a handler
builder.Handlers.AddMethod<Message>(HelloWorld);
```

**Key Points:**
- **Method Signature**: Must return `Task<Result>` and accept `IHandlerContainer<T>` and `CancellationToken`
- **Attributes**: Apply the same filter attributes as regular handlers
- **Container Methods**: Use extension methods like `container.Reply()`, `container.Response()`, etc.
- **Registration**: Use `AddMethod<T>()` to register methods as handlers
- **Benefits**: Quick prototyping, simple handlers, and code reuse

This approach is perfect for simple handlers or when you want to avoid creating full handler classes.

### 8.12. Q: Can I access DI container and configuration in filters?
**A:** Yes! When using Telegrator.Hosting, you can access the DI container and configuration in custom filters by casting `context.BotInfo` to `HostedTelegramBotInfo`:

```csharp
public class DatabaseUserFilterAttribute : FilterAnnotation<Message>
{
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        // Cast to HostedTelegramBotInfo to access services
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        // Access DI container
        var dbContext = botInfo.Services.GetRequiredService<UsersDbContext>();
        var configuration = botInfo.Services.GetRequiredService<IConfiguration>();
        
        // Use services in filter logic
        var user = dbContext.Users.FirstOrDefault(u => u.TelegramId == context.Input.From?.Id);
        return user?.IsActive == true;
    }
}
```

**Key Points:**
- **Hosting Only**: This feature is only available when using `Telegrator.Hosting`
- **Type Casting**: Cast `context.BotInfo` to `HostedTelegramBotInfo`
- **Service Access**: Use `botInfo.Services.GetRequiredService<T>()` to access DI services
- **Configuration Access**: Use `botInfo.Services.GetRequiredService<IConfiguration>()` for settings
- **Null Safety**: Always check if the cast is successful before using services

This allows you to create powerful filters that integrate with your application's database, configuration, and other services.

### 8.8. Q: Can I stop handler execution from an aspect?
**A:** Yes! Return `Result.Fault()` from a pre-execution processor to stop handler execution. Return `Ok` to continue.

### 8.9. Q: What's the execution order of aspects?
**A:** Aspects execute in the following order:
1. Pre-execution aspects (self-processing first, then typed)
2. Handler main execution
3. Post-execution aspects (self-processing first, then typed)

### 8.10. Q: How do I create reusable aspects for multiple handlers?
**A:** Create external processor classes that implement `IPreProcessor` or `IPostProcessor`, then apply them using `[BeforeExecution<T>]` or `[AfterExecution<T>]` attributes. This allows you to share the same aspect logic across multiple handlers.

### 8.11. Q: Can I create handlers from methods instead of full classes?
**A:** Yes! You can create implicit handlers from methods using the same attributes and patterns as regular handlers:

```csharp
[MessageHandler, TextEquals("Hello", StringComparison.InvariantCultureIgnoreCase)]
private static async Task<Result> HelloWorld(IHandlerContainer<Message> container, CancellationToken cancellationToken)
{
    await container.Reply("Hello, World!", cancellationToken: cancellationToken);
    return Ok;
}

// Register the method as a handler
builder.Handlers.AddMethod<Message>(HelloWorld);
```

**Key Points:**
- **Method Signature**: Must return `Task<Result>` and accept `IHandlerContainer<T>` and `CancellationToken`
- **Attributes**: Apply the same filter attributes as regular handlers
- **Container Methods**: Use extension methods like `container.Reply()`, `container.Response()`, etc.
- **Registration**: Use `AddMethod<T>()` to register methods as handlers
- **Benefits**: Quick prototyping, simple handlers, and code reuse

This approach is perfect for simple handlers or when you want to avoid creating full handler classes.

### 8.12. Q: Can I access DI container and configuration in filters?
**A:** Yes! When using Telegrator.Hosting, you can access the DI container and configuration in custom filters by casting `context.BotInfo` to `HostedTelegramBotInfo`:

```csharp
public class DatabaseUserFilterAttribute : FilterAnnotation<Message>
{
    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        // Cast to HostedTelegramBotInfo to access services
        if (context.BotInfo is not HostedTelegramBotInfo botInfo)
            return false;

        // Access DI container
        var dbContext = botInfo.Services.GetRequiredService<UsersDbContext>();
        var configuration = botInfo.Services.GetRequiredService<IConfiguration>();
        
        // Use services in filter logic
        var user = dbContext.Users.FirstOrDefault(u => u.TelegramId == context.Input.From?.Id);
        return user?.IsActive == true;
    }
}
```

**Key Points:**
- **Hosting Only**: This feature is only available when using `Telegrator.Hosting`
- **Type Casting**: Cast `context.BotInfo` to `HostedTelegramBotInfo`
- **Service Access**: Use `botInfo.Services.GetRequiredService<T>()` to access DI services
- **Configuration Access**: Use `botInfo.Services.GetRequiredService<IConfiguration>()` for settings
- **Null Safety**: Always check if the cast is successful before using services

This allows you to create powerful filters that integrate with your application's database, configuration, and other services.

### 8.2. Debugging Guide

#### Enable Debug Logging

For detailed information about the logging system, see [section 7.5 - Logging System](#75-logging-system).

**Quick Start:**
```csharp
using Telegrator.Logging;

// Add console adapter for debugging
TelegratorLogging.AddConsoleAdapter(LogLevel.Debug, includeTimestamp: true);

// Use logging
TelegratorLogging.LogInformation("Bot started");
TelegratorLogging.LogError("Something went wrong", exception);
```

#### Common Debugging Steps
1. **Check Handler Registration**: Verify handlers are properly registered
2. **Verify Filters**: Ensure filters are correctly configured
3. **Test Individual Handlers**: Test handlers in isolation
4. **Monitor Logs**: Check application logs for errors
5. **Use Breakpoints**: Set breakpoints in handler methods

#### Performance Monitoring
- Monitor handler execution times
- Check memory usage patterns
- Track concurrent execution counts
- Monitor error rates and types

#### Simple Logging
```csharp
// In your handlers or aspects
TelegratorLogging.LogInformation("Handler executed");
TelegratorLogging.LogError("Something went wrong", exception);
TelegratorLogging.LogWarning("User exceeded rate limit");
```

---

## 9. Links

- [Main Repository](https://github.com/Rikitav/Telegrator)
- [Wiki & Examples](https://github.com/Rikitav/Telegrator/wiki/)
- [NuGet Package](https://www.nuget.org/packages/Telegrator)
- [Issues & Discussions](https://github.com/Rikitav/Telegrator/issues)

---

> **Feel free to contribute, ask questions, or open issues!**

В главных ролях :
> Сишарпилло Крокодилло,
> Дыкий Сишарп,
> Шарпенко Михаил Дотнетович

Кастинг и Тестирование :
> не проводилось