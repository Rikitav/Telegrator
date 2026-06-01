---
title: "Localization Setup"
description: "Initializing multi-language support in Telegrator."
---

# Localization Setup

The `Telegrator.Localized` package allows you to build bots that talk to users in their own language. It uses the standard .NET `IStringLocalizer` ecosystem.

## Installation

```shell
dotnet add package Telegrator.Localized
```

## 1. Create Resource Files
Create standard `.resx` files in your project (e.g., in a `Resources` folder):
- `BotMessages.en.resx`: "Welcome" -> "Welcome to our bot!"
- `BotMessages.ru.resx`: "Welcome" -> "Добро пожаловать в нашего бота!"

## 2. Register Services

```csharp
using Telegrator.Localized;

var builder = Host.CreateApplicationBuilder(args);

// Add standard .NET localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add Telegrator with standard setup
builder.AddTelegrator(action: b => {
    b.Handlers.CollectHandlers();
})
.WithPolling();

var host = builder.Build();
host.UseTelegrator();
await host.RunAsync();
```

## 3. Using in Handlers

Implement `ILocalizedHandler<T>` to gain access to the `LocalizationProvider`:

```csharp
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : CommandHandler, ILocalizedHandler<Message>
{
    public IStringLocalizer LocalizationProvider { get; }

    public StartHandler(IStringLocalizer<StartHandler> localizer)
    {
        LocalizationProvider = localizer;
    }

    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // 1. Send localized response immediately
        await this.ResponseLocalized("Welcome");
        
        // 2. Get localized string for custom use (e.g., logging or variable)
        string welcomeText = this.Localize("Welcome");
        
        return Ok;
    }
}
```

## Extension Methods

### `ResponseLocalized(key, args...)`
Automatically resolves the user's language, fetches the string from resources, and sends it as a response to the original message.

### `Localize(key)`
Returns the localized `string` for the given key. This is useful when you need the text for something other than a direct message (e.g., button text, logging, or notifications).

## How It Works
When these methods are called, Telegrator:
1. Looks at `container.Update.Message.From.LanguageCode` (or uses a custom `ICultureResolver`).
2. Sets the current thread's `CultureInfo` to that language.
3. Fetches the string from your resources via `IStringLocalizer`.

## Limitations
- **Fallback**: If the user's language is not supported, it falls back to your application's default culture.
- **Static Content**: Localization only works for messages sent by the bot, not for built-in Telegram UI elements (unless you use BotFather to translate them).
