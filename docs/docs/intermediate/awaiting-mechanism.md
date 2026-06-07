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

<table>
<thead>
<tr><th>Method</th><th>Return Type</th><th>Update Type</th></tr>
</thead>
<tbody>
<tr><td>`AwaitAny`</td><td>`Update`</td><td>`Unknown`</td></tr>
<tr><td>`AwaitMessage`</td><td>`Message`</td><td>`Message`</td></tr>
<tr><td>`AwaitEditedMessage`</td><td>`Message`</td><td>`EditedMessage`</td></tr>
<tr><td>`AwaitChannelPost`</td><td>`Message`</td><td>`ChannelPost`</td></tr>
<tr><td>`AwaitEditedChannelPost`</td><td>`Message`</td><td>`EditedChannelPost`</td></tr>
<tr><td>`AwaitBusinessMessage`</td><td>`Message`</td><td>`BusinessMessage`</td></tr>
<tr><td>`AwaitEditedBusinessMessage`</td><td>`Message`</td><td>`EditedBusinessMessage`</td></tr>
<tr><td>`AwaitDeletedBusinessMessages`</td><td>`BusinessMessagesDeleted`</td><td>`DeletedBusinessMessages`</td></tr>
<tr><td>`AwaitBusinessConnection`</td><td>`BusinessConnection`</td><td>`BusinessConnection`</td></tr>
<tr><td>`AwaitMessageReaction`</td><td>`MessageReactionUpdated`</td><td>`MessageReaction`</td></tr>
<tr><td>`AwaitMessageReactionCount`</td><td>`MessageReactionCountUpdated`</td><td>`MessageReactionCount`</td></tr>
<tr><td>`AwaitInlineQuery`</td><td>`InlineQuery`</td><td>`InlineQuery`</td></tr>
<tr><td>`AwaitChosenInlineResult`</td><td>`ChosenInlineResult`</td><td>`ChosenInlineResult`</td></tr>
<tr><td>`AwaitCallbackQuery`</td><td>`CallbackQuery`</td><td>`CallbackQuery`</td></tr>
<tr><td>`AwaitShippingQuery`</td><td>`ShippingQuery`</td><td>`ShippingQuery`</td></tr>
<tr><td>`AwaitPreCheckoutQuery`</td><td>`PreCheckoutQuery`</td><td>`PreCheckoutQuery`</td></tr>
<tr><td>`AwaitPurchasedPaidMedia`</td><td>`PaidMediaPurchased`</td><td>`PurchasedPaidMedia`</td></tr>
<tr><td>`AwaitPoll`</td><td>`Poll`</td><td>`Poll`</td></tr>
<tr><td>`AwaitPollAnswer`</td><td>`PollAnswer`</td><td>`PollAnswer`</td></tr>
<tr><td>`AwaitMyChatMember`</td><td>`ChatMemberUpdated`</td><td>`MyChatMember`</td></tr>
<tr><td>`AwaitChatMember`</td><td>`ChatMemberUpdated`</td><td>`ChatMember`</td></tr>
<tr><td>`AwaitChatJoinRequest`</td><td>`ChatJoinRequest`</td><td>`ChatJoinRequest`</td></tr>
<tr><td>`AwaitChatBoost`</td><td>`ChatBoostUpdated`</td><td>`ChatBoost`</td></tr>
<tr><td>`AwaitRemovedChatBoost`</td><td>`ChatBoostRemoved`</td><td>`RemovedChatBoost`</td></tr>
<tr><td>`AwaitManagedBot`</td><td>`ManagedBotUpdated`</td><td>`ManagedBot`</td></tr>
<tr><td>`AwaitGuestMessage`</td><td>`Message`</td><td>`GuestMessage`</td></tr>
<tr><td>`CancellAllCallbacks`</td><td>`CallbackQuery`</td><td>`CallbackQuery` (auto-deleting)</td></tr>
</tbody>
</table>

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
