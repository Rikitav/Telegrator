---
title: "Organization & Testing"
description: "Best practices for architecting and thoroughly testing your bots."
---

# Best Practices & Patterns

## Project Organization

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

## Testing Strategies

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

## Common Patterns

### Command Pattern
```csharp
[CommandHandler]
[CommandAllias("settings")]
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

### Wizard Pattern
```csharp
[CommandHandler]
[CommandAllias("wizard")]
[State<SenderIdResolver, int>(0)]
public class StartWizardHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await StateStorage.GetStateMachine<int>(HandlingUpdate).BySenderId().Advance();
        await Reply("Step 1: What is your name?");
        return Ok;
    }
}
```

## Aspect-Oriented Programming Best Practices
- **Separation of Concerns**: Keep aspects focused on a single responsibility (logging, validation, authorization, etc.)
- **Reusability**: Create generic aspects that can be applied to multiple handlers
- **Performance**: Avoid heavy operations in aspects that run frequently
- **Error Handling**: Always handle exceptions in aspects gracefully
- **Testing**: Test aspects independently from handlers
- **Documentation**: Document the purpose and behavior of custom aspects
- **State Management**: Use thread-safe collections for aspects that maintain state

## Quick Tips
- Organize handlers, filters, and state keepers in separate folders
- Use feature modules for large bots
- Prefer declarative filters over manual `if` statements
- Keep handlers focused and single-responsibility
- Use dependency injection for better testability
