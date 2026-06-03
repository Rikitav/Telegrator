---
title: "Awaiting Mechanism"
description: "Pause the handler directly to await the user's next update."
---

# Awaiting Mechanism

Use `AwaitingProvider` to wait for a user's next update (message or callback) inside a handler. This allows for a linear flow in complex multi-step interactions.

```csharp
[CommandHandler]
[CommandAllias("ask")]
public class AskHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");
        
        // Pause execution until the same user sends another message
        Update update = await AwaitingProvider.AwaitAny(HandlingUpdate).BySenderId(cancellation);
        Message nextMessage = update.Message;
        
        await Reply($"Hello, {nextMessage.Text}!");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Awaiting Provider**: `container.AwaitMessage()` creates a temporary "waiting" handler.
> 2. **Correlation**: `.BySenderId(cancellation)` ensures the bot only captures the message from the same person in the current chat.
> 3. **Non-blocking**: The internal router handles this using `TaskCompletionSource`, so the thread is freed while waiting.
> 4. **Cleanup**: The temporary handler is automatically removed once the message is received or the token is cancelled.

## Convenience Methods

Telegrator provides strongly-typed convenience methods for every `UpdateType`. You can call them on either `IAwaitingProvider` (passing the current `Update`) or on `IHandlerContainer` directly.

### Via `IHandlerContainer` (Recommended)
```csharp
// Inside a handler
Message msg = await container.AwaitMessage().ByChatId(cancellation);
CallbackQuery cb = await container.AwaitCallbackQuery().BySenderId(cancellation);
ChatJoinRequest req = await container.AwaitChatJoinRequest().ByChatId(cancellation);
```

### Via `IAwaitingProvider`
```csharp
// Inside a handler (equivalent)
Message msg = await AwaitingProvider.AwaitMessage(HandlingUpdate).ByChatId(cancellation);
```

### Supported Await Types

| Method | Return Type | Update Type |
|--------|-------------|-------------|
| `AwaitAny` | `Update` | `Unknown` |
| `AwaitMessage` | `Message` | `Message` |
| `AwaitEditedMessage` | `Message` | `EditedMessage` |
| `AwaitChannelPost` | `Message` | `ChannelPost` |
| `AwaitEditedChannelPost` | `Message` | `EditedChannelPost` |
| `AwaitBusinessMessage` | `Message` | `BusinessMessage` |
| `AwaitEditedBusinessMessage` | `Message` | `EditedBusinessMessage` |
| `AwaitDeletedBusinessMessages` | `BusinessMessagesDeleted` | `DeletedBusinessMessages` |
| `AwaitBusinessConnection` | `BusinessConnection` | `BusinessConnection` |
| `AwaitMessageReaction` | `MessageReactionUpdated` | `MessageReaction` |
| `AwaitMessageReactionCount` | `MessageReactionCountUpdated` | `MessageReactionCount` |
| `AwaitInlineQuery` | `InlineQuery` | `InlineQuery` |
| `AwaitChosenInlineResult` | `ChosenInlineResult` | `ChosenInlineResult` |
| `AwaitCallbackQuery` | `CallbackQuery` | `CallbackQuery` |
| `AwaitShippingQuery` | `ShippingQuery` | `ShippingQuery` |
| `AwaitPreCheckoutQuery` | `PreCheckoutQuery` | `PreCheckoutQuery` |
| `AwaitPurchasedPaidMedia` | `PaidMediaPurchased` | `PurchasedPaidMedia` |
| `AwaitPoll` | `Poll` | `Poll` |
| `AwaitPollAnswer` | `PollAnswer` | `PollAnswer` |
| `AwaitMyChatMember` | `ChatMemberUpdated` | `MyChatMember` |
| `AwaitChatMember` | `ChatMemberUpdated` | `ChatMember` |
| `AwaitChatJoinRequest` | `ChatJoinRequest` | `ChatJoinRequest` |
| `AwaitChatBoost` | `ChatBoostUpdated` | `ChatBoost` |
| `AwaitRemovedChatBoost` | `ChatBoostRemoved` | `RemovedChatBoost` |
| `AwaitManagedBot` | `ManagedBotUpdated` | `ManagedBot` |
| `AwaitGuestMessage` | `Message` | `GuestMessage` |
| `CancellAllCallbacks` | `CallbackQuery` | `CallbackQuery` (auto-deleting) |

For update types not covered above, use the generic `AwaitUpdate<TUpdate>(UpdateType)` method.

## MightAwait Analyzer (TLG201)

When a handler calls any awaiting method, the **MightAwaitAnalyzer** source generator checks whether the handler is decorated with `[MightAwait]`. If not, it emits a **TLG201** warning and **auto-injects** the attribute at compile time.

### Explicit Attribute
```csharp
[MessageHandler]
[MightAwait(UpdateType.Message)]
public class AskHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("What is your name?");
        var next = await container.AwaitMessage().BySenderId(cancellation);
        await Reply($"Hello, {next.Text}!");
        return Ok;
    }
}
```

### Auto-Injection
If you omit `[MightAwait]`, the `HandlersCollectorGenerator` will automatically add it when it detects awaiting calls. This ensures the router always knows which update types the handler might await, enabling correct exclusive routing.
