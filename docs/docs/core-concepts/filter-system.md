---
title: "Filter System"
description: "Understand the gatekeepers of your bot logic."
---

# Filter System

Filters are the gatekeepers of your bot logic. They determine when handlers should be executed.

## Filter Types
- **Text Filters**: `TextContains`, `TextStartsWith`, `TextEndsWith`, `TextEquals`, `MessageRegex`, `HasText`
- **User Filters**: `FromUserId`, `FromUser`, `FromUsername`, `FromBot`, `FromPremiumUser`, `NotFromBot`
- **Chat Filters**: `ChatType`, `ChatId`, `ChatTitle`, `ChatUsername`, `ChatName`, `ChatIsForum`, `Mentioned`
- **Message Content**: `DiceThrowed`, `IsAutomaticForward`, `IsFromOffline`, `IsServiceMessage`, `MessageHasEntity`
- **Reply System**: `HasReply`, `MeReplied`, `FromReplyChain`
- **Command Filters**: `CommandAllias`, `ArgumentCount`, `ArgumentStartsWith`, `ArgumentContains`, etc.
- **Environment**: `IsDebugEnvironment`, `IsReleaseEnvironment`, `EnvironmentVariable`
- **State Filters**: `State<TKey, TValue>` (generic state checking)

## Filter Composition
- **AND Logic**: Multiple filters are combined with AND by default
- **OR Logic**: Use `FilterModifier.OrNext` to combine with OR
- **NOT Logic**: Use `FilterModifier.Not` to invert a filter
- **Combined Modifiers**: Use bitwise OR to combine modifiers

## Custom Filters
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

## Hosting Integration - Access to DI Container

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
