---
title: "CallbackQuery Handling"
description: "React to inline keyboard buttons."
---

# CallbackQuery Handling

Reacting to inline keyboards is incredibly straightforward with `CallbackQueryHandler`.

```csharp
[CommandHandler]
[CommandAllias("menu")]
public class MenuHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Option 1", "option1"),
            InlineKeyboardButton.WithCallbackData("Option 2", "option2")
        });

        await Reply("Choose an option:", replyMarkup: keyboard, cancellationToken: cancellation);
        return Ok;
    }
}

[CallbackQueryHandler]
[CallbackData("option1")]
public class Option1Handler : CallbackQueryHandler
{
    public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 1!", cancellationToken: cancellation);
        await EditMessage("You selected Option 1!");
        return Ok;
    }
}

[CallbackQueryHandler]
[CallbackData("option2")]
public class Option2Handler : CallbackQueryHandler
{
    public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
    {
        await AnswerCallbackQuery("You selected Option 2!", cancellationToken: cancellation);
        await EditMessage("You selected Option 2!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Inline Keyboard**: `InlineKeyboardMarkup` creates interactive buttons with `CallbackData` identifiers.
> 2. **CallbackQuery Handlers**: `[CallbackQueryHandler]` and `[CallbackData]` work together to handle button clicks.
> 3. **Response Methods**: `AnswerCallbackQuery()` provides immediate feedback, while `EditMessage()` updates the message.
> 4. **Handler Separation**: Each button option has its own dedicated handler for clean code organization.
