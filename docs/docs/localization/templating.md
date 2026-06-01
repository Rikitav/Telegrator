---
title: "Localized Templating"
description: "Formatting localized strings with dynamic data."
---

# Localized Templating

Often you need to include dynamic values (like usernames or counts) in your localized messages.

## Using Arguments in Resources

In your `.resx` file, define strings with standard C# format placeholders:
- `Greeting` -> `Hello, {0}! Welcome back.`

## Passing Arguments in Code

The `ResponseLocalized` method accepts optional arguments:

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    string name = container.Update.Message!.From!.FirstName;
    
    // Result: "Hello, John! Welcome back." (in user's language)
    await this.ResponseLocalized("Greeting", name);
    
    return Ok;
}
```

## Complex Objects
You can pass any number of arguments. They are applied via `string.Format` under the hood after the localized string is retrieved.

## Localized Keyboards
You can also use localization for button text:

```csharp
var buttonText = LocalizationProvider["SettingsButton"];
var keyboard = new ReplyKeyboardMarkup(new[] { new KeyboardButton(buttonText) });
```

## Best Practices
1. **Key Naming**: Use clear, descriptive keys (e.g., `Errors_UserNotFound`, `Commands_Start_Welcome`).
2. **Translation Quality**: Avoid machine translation for complex UI flows as it can break formatting or UX.
3. **Empty Values**: Ensure your resources have values for all supported languages to avoid seeing raw keys in production.
