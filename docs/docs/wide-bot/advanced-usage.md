---
title: "Advanced WideBot Usage"
description: "Media handling and raw TL calls in WideBot."
---

# Advanced WideBot Usage

MTProto allows for much deeper interaction with Telegram's infrastructure, especially regarding media and large updates.

## Uploading Large Files

One of the biggest advantages of WideBot is efficient file uploading. You can stream files directly in chunks:

```csharp
var wClient = container.Client.AsWClient();

// High-level helper or raw call
var uploadedFile = await wClient.UploadFileAsync("large_video.mp4", stream);
await wClient.SendVideoAsync(chatId, uploadedFile);
```

## Custom Update Filtering

Because MTProto sends a lot of service updates (User status changes, typing, etc.), you might want to filter them out globally to save resources. You can do this in `TelegratorOptions`.

## Working with TL.Schema

WTelegramBot (and thus WideBot) generates a set of classes based on the Telegram API schema. You can perform any action allowed by your bot's privileges, including:
- Joining/Leaving channels.
- Moderation (banning/unbanning with custom permissions).
- Managing folder/dialog filters (if the account is not a bot but a user — WideBot can also work with user accounts!).

## Bot vs User Account
While `Telegrator.Hosting.WideBot` is named "Bot", the underlying `WTelegramBotClient` can also log in as a **normal user account**. 

> [!CAUTION]
> Using MTProto for user automation can lead to account bans if you violate Telegram's terms of service. Use responsibly.

## Limitations
- **No official support**: MTProto is an internal Telegram protocol. While documented, official libraries only exist for mobile apps.
- **Breaking Changes**: Telegram updates their TL schema frequently. WideBot usually needs updates to stay compatible.
