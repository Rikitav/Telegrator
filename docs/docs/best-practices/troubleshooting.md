---
title: "Troubleshooting & FAQ"
description: "Resolving common issues and managing bugs."
---

# FAQ & Troubleshooting

## Common Issues

### Q: My handler is not being triggered. What should I do?
- Check handler registration (use `bot.Handlers.CollectHandlers()` generative method)
- Check IDE for errors from **DeveloperHelperAnalyzer** (e.g. mismatched attributes and base classes)
- Check filter attributes and update types
- Enable debug logging
- Verify that the handler class inherits from the correct base class

### Q: How can I access the `ITelegramBotClient` or the original `Update` object inside a handler?
- Use `Client`, `Update`, and `Input` properties in your handlers container

### Q: How do I handle errors?
- Set a custom exception handler or subscribe to error events
- Use try-catch blocks in individual handlers for specific error handling
- Implement proper logging for debugging

### Q: How can I organize my code for a large bot?
- Use folders, feature modules, and namespaces
- Keep handlers focused and modular
- Use dependency injection for better separation of concerns
- Implement proper state management for complex flows

### Q: What's the difference between `Reply()` and `Responce()` methods?
- `Reply()` sends a reply to the original message (it automatically sets `ReplyToMessageId`).
- `Responce()` sends a new message to the chat (it does NOT set `ReplyToMessageId`).
- Both methods are available in `MessageHandler` and `CallbackQueryHandler`.

**Note:** `Responce()` has a typo in the name but is intentionally kept for backward compatibility.

### Q: Why are my bot commands not showing up in Telegram?
**A:** You must call `host.SetBotCommands()` (or `bot.SetBotCommands()` for WideBot) after building the host. Telegrator will automatically register all commands defined via `[CommandAllias]` attributes.

### Q: How do I handle multiple bots in one application?
**A:** Telegrator's `Hosting` system is designed for single-bot per process integration by default. For multiple bots, consider using multiple `TelegratorClient` instances manually or creating separate Generic Hosts.

### Q: How do I implement webhook hosting?
- Use `Telegrator.Hosting.Web` package
- Configure `TelegramBotWebOptions` with your bot token and webhook URL
- Ensure your server has HTTPS enabled
- Set up proper SSL certificates for production use

### Q: How can I add logging or validation to all handlers?
**A:** Use the aspect system with `IPreProcessor` and `IPostProcessor` interfaces:
- **Self-processing**: Implement interfaces directly in your handler
- **Typed processing**: Create external processor classes and apply with attributes
- **Combined approach**: Use both methods together

### Q: Can I stop handler execution from an aspect?
**A:** Yes! Return `Result.Fault()` from a pre-execution processor to stop handler execution. Return `Ok` to continue.

### Q: What's the execution order of aspects?
**A:** Aspects execute in the following order:
1. Pre-execution aspects (self-processing first, then typed)
2. Handler main execution
3. Post-execution aspects (self-processing first, then typed)

### Q: How do I create reusable aspects for multiple handlers?
**A:** Create external processor classes that implement `IPreProcessor` or `IPostProcessor`, then apply them using `[BeforeExecution<T>]` or `[AfterExecution<T>]` attributes. This allows you to share the same aspect logic across multiple handlers.

### Q: Can I create handlers from methods instead of full classes?
**A:** Yes! You can create implicit handlers from methods using the same attributes and patterns as regular handlers.

### Q: Can I access DI container and configuration in filters?
**A:** Yes! When using Telegrator.Hosting, you can access the DI container and configuration in custom filters by casting `context.BotInfo` to `HostedTelegramBotInfo`.

## Debugging Guide

### Enable Debug Logging

**Quick Start:**
```csharp
using Telegrator.Logging;

// Add console adapter for debugging
TelegratorLogging.AddAdapter(new ConsoleLogger(LogLevel.Debug, includeTimestamp: true));

// Use logging
TelegratorLogging.LogInformation("Bot started");
TelegratorLogging.LogError("Something went wrong", exception);
```

### Common Debugging Steps
1. **Check Handler Registration**: Verify handlers are properly registered
2. **Verify Filters**: Ensure filters are correctly configured
3. **Test Individual Handlers**: Test handlers in isolation
4. **Monitor Logs**: Check application logs for errors
5. **Use Breakpoints**: Set breakpoints in handler methods

### Performance Monitoring
- Monitor handler execution times
- Check memory usage patterns
- Track concurrent execution counts
- Monitor error rates and types
