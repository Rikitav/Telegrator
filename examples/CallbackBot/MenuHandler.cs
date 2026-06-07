/*
 * Copyright (c) 2026 Rikitav Tim4ik
 */

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Annotations;
using Telegrator.Attributes;
using Telegrator.Core.Handlers;
using Telegrator.Handlers;

namespace Telegrator.Examples;

/// <summary>
/// Demonstrates inline keyboard usage with the awaiting mechanism.
/// Sends a menu and waits for the user to press a callback button.
/// </summary>
[MessageHandler]
[MightAwait(UpdateType.CallbackQuery)]
public class MenuHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input?.Text is not { } text)
            return Ok;

        if (!text.Equals("/menu", StringComparison.OrdinalIgnoreCase))
        {
            await Reply("Send /menu to open the inline keyboard.", cancellationToken: cancellation);
            return Ok;
        }

        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup([
            [InlineKeyboardButton.WithCallbackData("Option 1", "opt1")],
            [InlineKeyboardButton.WithCallbackData("Option 2", "opt2")],
            [InlineKeyboardButton.WithCallbackData("Cancel", "cancel")]
        ]);

        await Reply("Please choose an option:", replyMarkup: keyboard, cancellationToken: cancellation);

        CallbackQuery? query = await AwaitingProvider.AwaitCallbackQuery(HandlingUpdate).BySenderId(cancellation);
        if (query?.Data is not { } data)
        {
            await Reply("No response received.", cancellationToken: cancellation);
            return Ok;
        }

        string response = data switch
        {
            "opt1" => "You selected Option 1!",
            "opt2" => "You selected Option 2!",
            "cancel" => "Menu cancelled.",
            _ => "Unknown option."
        };

        await Reply(response, cancellationToken: cancellation);
        return Ok;
    }
}
