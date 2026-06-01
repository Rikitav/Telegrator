---
title: "Quick Start"
description: "Create your first bot in minutes."
---

# Quick Start

This section will get you up and running with Telegrator quickly. You'll learn the basics and create your first bot in minutes.

## Your First Bot

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

        // Option A: Manual registration
        bot.Handlers.AddHandler<HelloHandler>();

        // Option B: Generative discovery (Recommended for Native AOT)
        bot.Handlers.CollectHandlers();

        bot.StartReceiving();
        Console.ReadLine();
    }
}
```

> [!TIP]
> **Native AOT & Trimming**: Telegrator now fully supports Native AOT. The `CollectHandlers()` method is source-generated at compile-time, ensuring no reflection is used for discovery and no code is trimmed incorrectly.

> **How is it working?**
> 1. **`[MessageHandler]`**: Marks the class as a handler for message updates
> 2. **`[ChatType(ChatType.Private)]`**: Only processes private chat messages
> 3. **`[TextContains("hello")]`**: Only processes messages containing "hello" (case-insensitive)
> 4. **`TelegratorClient`**: Main bot client that manages Telegram connection
> 5. **`bot.Handlers.AddHandler<HelloHandler>()`**: Registers the handler
> 6. **`bot.StartReceiving()`**: Starts the long-polling loop
> 7. **`Reply(...)`**: Sends a reply to the original message

## Basic Handler Types

Telegrator provides several handler types for different update types:

### MessageHandler
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

### CommandHandler
Handles bot commands (messages starting with `/`):

```csharp
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Welcome! Use /help to see available commands.");
        return Ok;
    }
}
```

### CallbackQueryHandler
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

### AnyUpdateHandler
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

## Simple Filters

Filters determine when your handlers should run. Here are the most common ones:

### Text Filters
```csharp
[TextContains("hello")]           // Message contains "hello"
[TextStartsWith("/")]             // Message starts with "/"
[TextStartsWith("/", Modifiers = FilterModifier.Not)]  // Message does NOT start with "/"
```

### User Filters
```csharp
[FromUserId(123456789)]           // Only from specific user ID
[FromUser("John")]                // Only from user with specific name
[FromUsername("john_doe")]        // Only from user with specific username
```

### Chat Filters
```csharp
[ChatType(ChatType.Private)]      // Only private chats
[ChatType(ChatType.Group)]        // Only group chats
[Mentioned]                       // Only if bot was mentioned with @
```

### Command Filters
```csharp
[CommandAllias("start")]          // Only for /start command
[CommandAllias("help")]           // Only for /help command
```

### Combining Filters
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
