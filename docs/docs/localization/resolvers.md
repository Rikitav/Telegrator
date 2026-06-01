---
title: "Culture Resolvers"
description: "Customizing how the user's language is determined."
---

# Culture Resolvers

By default, Telegrator determines the user's language from the `LanguageCode` field in the Telegram `User` object. However, in many cases, you want users to manually pick a language that gets stored in a database.

## Implementing a Custom Resolver

You can implement `ICultureResolver` to override the default behavior:

```csharp
public class DatabaseCultureResolver : ICultureResolver
{
    private readonly IUserRepository _userRepository;

    public DatabaseCultureResolver(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CultureInfo> ResolveAsync(IHandlerContainer container)
    {
        var userId = container.HandlingUpdate.SenderId();
        var preferredLanguage = await _userRepository.GetLanguageAsync(userId);
        
        return new CultureInfo(preferredLanguage ?? "en");
    }
}
```

## Registering the Resolver

```csharp
builder.Services.AddSingleton<ICultureResolver, DatabaseCultureResolver>();
```

## Benefits of Custom Resolvers
- **Persistence**: Users don't lose their language setting if they change it in their Telegram app.
- **Granularity**: You can have different languages for different chats (e.g., a group chat in Spanish, even if the admin's app is in English).
- **Graceful Fallback**: You can implement complex logic for region-specific dialects.

## Limitations
- **Performance**: If your resolver calls a database, it will be executed for **every** update. Use caching to keep it fast!
- **Context**: The resolver only has access to the current `IHandlerContainer`.
