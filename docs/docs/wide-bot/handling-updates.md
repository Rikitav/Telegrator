---
title: "Handling MTProto Updates"
description: "How to process raw TL updates in WideBot handlers."
---

# Handling MTProto Updates

In WideBot, the `Update` object you receive in your handlers is not the same as the standard Telegram Bot API `Update`. It is a wrapped `TL.Update`.

## Update Compatibility

Telegrator tries to normalize basic updates. However, for advanced MTProto features, you need to work with raw types.

```csharp
[UpdateHandler]
public class RawUpdateHandler : AnyUpdateHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Update> container, CancellationToken cancellation)
    {
        // Access MTProto raw update
        TL.Update rawUpdate = container.HandlingUpdate.AsWUpdate().TLUpdate;
        
        if (rawUpdate is TL.UpdateNewMessage unm)
        {
            // Handle raw MTProto message
        }
        
        return Ok;
    }
}
```

## Update & Client Properties

Inside WideBot handlers, you have specialized properties to access MTProto-specific data:

```csharp
[MessageHandler]
public class WideInfoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // 1. Specialized Update Properties
        // Get WTelegram.Types.Update
        var wideUpdate = this.WideUpdate; 
        
        // Get raw TL.Update
        var tlUpdate = this.TLUpdate;

        // 2. Specialized Client
        // Access MTProto client via property or cast
        WTelegramBotClient wideClient = this.WClient; 
        
        // Use MTProto-specific methods
        ChatFullInfo userDetails = await wideClient.GetChat("@RikitavTimur");
        
        // 3. Raw TL Data Extraction
        // Use .TLInfo() to get the underlying TL object
        TL.Users_UserFull full = (TL.Users_UserFull)userDetails.TLInfo()!;
        TL.UserFull fullUser = full.full_user;

        await Reply($"User ID: {fullUser.id}");
        return Ok;
    }
}
```

- **`WClient` / `AsWClient()`**: Provides access to the `WTelegramBotClient` which contains methods like `GetChat`, `SetMyProfilePhoto`, etc.
- **`TLInfo()`**: An extension method allowing you to extract raw TL schema objects from Telegrator's high-level wrappers.
- **Update Properties**: `WideUpdate` and `TLUpdate` give you immediate access to the protocol-level objects received from Telegram.

## Advanced Interaction (Awaiting)

WideBot handlers can pause execution to wait for specific updates (like callback queries or messages) using the `AwaitingProvider`. It is recommended to use the `[MightAwait]` attribute to help the dispatcher understand potential waiting flows.

```csharp
[CommandHandler, CommandAllias("character"), MightAwait(UpdateType.CallbackQuery)]
public class CharacterSelectorHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Choose your character:", replyMarkup: MyKeyboard());

        // Wait for ANY update from the SAME sender
        Update update = await AwaitingProvider.AwaitAny(HandlingUpdate).BySenderId(cancellation);

        if (update.CallbackQuery is { Data: "hero_selected" } query)
        {
            await Client.AnswerCallbackQuery(query.Id, "Hero selected!");
        }

        return Ok;
    }
}
```

## Key Differences from Bot API
1. **IDs**: MTProto uses `long` for IDs, but they might differ in structure from Bot API IDs.
2. **Peers**: Instead of a simple ChatId, MTProto uses `InputPeer` types (User, Chat, Channel).
3. **Entities**: You often need to resolve entities (users/chats) because MTProto updates only contain IDs.

## Limitations
- Filter support for MTProto is currently more limited than for the standard Bot API.
- State management works similarly but uses MTProto peer IDs as keys.
