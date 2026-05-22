# Telegrator.Localized

The `Telegrator.Localized` package provides seamless localization support for your Telegrator bots. It leverages the standard .NET `IStringLocalizer` ecosystem, allowing you to easily build bots that communicate with users in their preferred language.

## Features

- **Standard .NET Localization**: Works out-of-the-box with `.resx` files and standard .NET localization services.
- **Dynamic Culture Resolution**: Automatically detects user language from Telegram updates or allows custom resolution logic (e.g., from a database).
- **Aspect-Oriented**: Uses Telegrator's aspect system (`LocalizedAspect`) to set the thread's culture before handler execution without cluttering your business logic.
- **Easy Templating**: Pass dynamic arguments to your localized strings effortlessly.

## Installation

Install the package via NuGet:

```shell
dotnet add package Telegrator.Localized
```

## Quick Start

### 1. Create Resource Files

Create standard `.resx` files in your project (e.g., inside a `Resources` folder):
- `BotMessages.en.resx`: `Welcome` -> `"Welcome to our bot!"`
- `BotMessages.ru.resx`: `Welcome` -> `"Добро пожаловать в нашего бота!"`

### 2. Register Services

Add localization services and Telegrator to your DI container:

```csharp
using Telegrator.Localized;

var builder = Host.CreateApplicationBuilder(args);

// Add standard .NET localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Register culture resolver and the localized aspect
builder.Services.AddSingleton<ICultureResolver, DefaultCultureResolver>();

// Add Telegrator
builder.AddTelegrator();

var host = builder.Build();
await host.RunAsync();
```

### 3. Using in Handlers

Implement the `ILocalizedHandler<T>` interface (e.g., `ILocalizedMessageHandler`) to gain access to the `LocalizationProvider`.

```csharp
[CommandHandler]
[CommandAllias("start")]
[BeforeExecution<LocalizedAspect>] // Ensure culture is set before execution
public class StartHandler : CommandHandler, ILocalizedMessageHandler
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
        
        // 2. Get localized string for custom use (e.g., buttons, logging)
        string welcomeText = this.Localize("Welcome");
        
        return Ok;
    }
}
```

## Localized Templating

Often you need to include dynamic values (like usernames or counts) in your localized messages.

### Using Arguments in Resources

In your `.resx` file, define strings with standard C# format placeholders:
- `Greeting` -> `"Hello, {0}! Welcome back."`

### Passing Arguments in Code

The extension methods accept optional arguments:

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    string name = container.HandlingUpdate.Message!.From!.FirstName;
    
    // Result: "Hello, John! Welcome back." (in user's language)
    await this.ResponseLocalized("Greeting", name);
    
    return Ok;
}
```

## Culture Resolvers

By default, Telegrator determines the user's language from the `LanguageCode` field in the Telegram update via the `DefaultCultureResolver`. However, you can implement a custom resolver to load language preferences from a database.

### Implementing a Custom Resolver

Implement `ICultureResolver` to override the default behavior:

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
        var userId = container.HandlingUpdate.GetSenderId();
        var preferredLanguage = await _userRepository.GetLanguageAsync(userId);
        
        return new CultureInfo(preferredLanguage ?? "en");
    }
}
```

### Registering the Resolver

Replace the default resolver in your DI container:

```csharp
builder.Services.AddSingleton<ICultureResolver, DatabaseCultureResolver>();
```

## Limitations
- **Fallback**: If the user's language is not supported, it falls back to your application's default culture.
- **Static Content**: Localization only works for messages sent by the bot, not for built-in Telegram UI elements (unless you use BotFather to translate them).
