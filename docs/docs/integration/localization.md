---
title: "Localization"
description: "Build multi-language bots with built-in localization support."
---

# Localization

Telegrator supports multi-language bots through the `Telegrator.Localized` package. It integrates with the standard .NET `IStringLocalizer` and provides easy access to localized strings within your handlers.

## Installation

```shell
dotnet add package Telegrator.Localized
```

## Basic Usage

To use localization, your handler should implement the `ILocalizedHandler<T>` interface or you can use the `ResponseLocalized` extension method.

### 1. Define Language Resources
Create standard .NET resource files (`.resx`) for your languages (e.g., `Messages.en.resx`, `Messages.ru.resx`).

### 2. Configure Localization
Register .NET localization services in your host builder:

```csharp
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
```

### 3. Localized Response

Use the `ResponseLocalized` extension method to send a message using a key from your resource file:

```csharp
[CommandHandler]
[CommandAllias("start")]
public class StartHandler : MessageHandler, ILocalizedHandler<Message>
{
    public IStringLocalizer LocalizationProvider { get; }

    public StartHandler(IStringLocalizer<StartHandler> localizer)
    {
        LocalizationProvider = localizer;
    }

    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Automatically fetches the string based on user's language code from Update
        await this.ResponseLocalized("welcome_message");
        return Ok;
    }
}
```

> [!TIP]
> Telegrator automatically extracts the language code from the user's Telegram profile (`User.LanguageCode`) and uses it to resolve the correct string from your resource files.
