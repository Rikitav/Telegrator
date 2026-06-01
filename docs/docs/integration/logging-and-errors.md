---
title: "Logging & Errors"
description: "Logging interfaces and global exception handling."
---

# Error Handling and Logging

Telegrator provides a centralized logging system called "TelegratorLogging" that allows integration with various logging frameworks while maintaining zero dependencies in the core library.

## Logging System

The logging system consists of:
- **TelegratorLogging** - Centralized static logging system
- **ITelegratorLogger** - Core logging interface
- **NullLogger** - No-op logger
- **ConsoleLogger** - Simple console output
- **MicrosoftLoggingAdapter** - Integration with Microsoft.Extensions.Logging

### Basic Usage

**Console Logging:**
```csharp
using Telegrator.Logging;

// Add console adapter
TelegratorLogging.AddAdapter(new ConsoleLogger(LogLevel.Debug, includeTimestamp: true));

// Use logging
TelegratorLogging.LogInformation("Bot started");
TelegratorLogging.LogError("Something went wrong", exception);
```

**Hosting Integration:**
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
```

## Global Error Handling

You can subscribe to error events or set a custom exception handler:

```csharp
var bot = new TelegratorClient("<BOT_TOKEN>");
bot.ExceptionHandler = new CustomExceptionHandler();

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
        return Task.CompletedTask;
    }
}
```

## Handler-Level Error Handling
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
