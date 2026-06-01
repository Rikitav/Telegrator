---
title: "Advanced Hosting Integration"
description: "Create powerful filters that integrate with DI services and app settings."
---

## WideBot (MTProto) Support
If your bot needs [MTProto support](https://github.com/Rikitav/Telegrator/blob/master/src/Telegrator.Hosting.WideBot/README.md), use the `Telegrator.Hosting.WideBot` package. It replaces the default long-polling/webhook receiver with a powerful MTProto client.

```csharp
using Telegrator.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Add WideTelegrator instead of default
builder.AddWideTelegrator();

// Configure Wide Options
builder.Services.ConfigureWideTelegram(new WTelegramBotClientOptions(
    token: "YOUR_TOKEN",
    apiId: 12345,
    apiHash: "YOUR_HASH"
));

var host = builder.Build();

// Use WideTelegrator initialization
host.UseWideTelegrator();

await host.RunAsync();
```

> [!TIP]
> WideBot handlers can access `WTelegramBotClient` and `TL.Update` using cast helpers:
> `WTelegramBotClient wideClient = container.Client.AsWClient();`

## Database-Integrated Filters

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

## Configuration-Driven Filters

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

## Multi-Service Integration

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
