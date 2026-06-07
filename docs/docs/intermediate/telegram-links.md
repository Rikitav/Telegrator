# Telegram Link Generation

Telegrator provides a set of extension methods that make it easy to build Telegram-specific URLs and deep links directly from framework types such as `User`, `Chat`, and `Message`. This avoids hard-coding URL templates throughout your handlers and keeps link generation consistent.

## User Links

- **`GetUserLink()`**
  - Return Type: `string`
  - Description: Returns a `tg://user?id=...` deep link that opens the user's profile inside Telegram.
- **`GetPublicLink()`**
  - Return Type: `string?`
  - Description: Returns `https://t.me/username` when the user has a username; otherwise `null`.
- **`GetDeepLink()`**
  - Return Type: `string`
  - Description: Alias for `GetUserLink()`.

```csharp
User user = new User { Id = 123456, Username = "durov" };

string deep = user.GetUserLink();        // tg://user?id=123456
string? pub = user.GetPublicLink();      // https://t.me/durov
```

> Passing a `null` `User` to any of these methods throws `ArgumentNullException`.

## Chat Links

- **`GetPublicLink()`**
  - Return Type: `string?`
  - Description: Returns `https://t.me/username` when the chat has a username; otherwise `null`.

```csharp
Chat chat = new Chat { Id = 1, Username = "mychannel" };
string? link = chat.GetPublicLink();     // https://t.me/mychannel
```

> Passing a `null` `Chat` throws `ArgumentNullException`.

## Message Links

- **`GetMessageLink()`**
  - Return Type: `string?`
  - Description: Returns `https://t.me/username/messageId` when the containing chat has a username; otherwise `null`.

```csharp
Message message = new Message
{
    MessageId = 42,
    Chat = new Chat { Id = 1, Username = "channel" }
};

string? link = message.GetMessageLink(); // https://t.me/channel/42
```

> Passing a `null` `Message` throws `ArgumentNullException`.

## String Helpers

These extensions work on plain `string` values and normalize the input automatically (trim whitespace, strip leading `@` or `+`).

- **`ToTelegramPublicUrl()`**: Converts a username into `https://t.me/username`.
- **`ToTelegramInviteUrl()`**: Converts an invite hash into `https://t.me/+hash`.

```csharp
"@durov".ToTelegramPublicUrl();          // https://t.me/durov
"+AbCdEf".ToTelegramInviteUrl();         // https://t.me/+AbCdEf
```

> Empty or whitespace-only input throws `ArgumentException`.

## Share URLs

- **`GetShareUrl(url, text?)`**: Builds a `https://t.me/share/url?url=...` link with optional pre-filled text. Both parameters are URL-encoded automatically.

```csharp
TelegramLinkExtensions.GetShareUrl("https://example.com", "Check this");
// https://t.me/share/url?url=https%3A%2F%2Fexample.com&text=Check%20this%20out
```

## Deep Links

Static helpers that return well-known Telegram deep links.

- **`GetSettingsDeepLink()`**
  - Returns: `tg://settings`
  - Description: Opens Telegram settings.
- **`GetAddStickersDeepLink(setName)`**
  - Returns: `tg://addstickers?set=...`
  - Description: Opens the sticker pack installation screen.
- **`GetResolveDeepLink(username)`**
  - Returns: `tg://resolve?domain=...`
  - Description: Opens a profile by username.
- **`GetJoinDeepLink(inviteHash)`**
  - Returns: `tg://join?invite=...`
  - Description: Invites the user to a group or channel.
- **`GetMessageDeepLink(text)`**
  - Returns: `tg://msg?text=...`
  - Description: Pre-fills the message text field.

```csharp
TelegramLinkExtensions.GetSettingsDeepLink();                        // tg://settings
TelegramLinkExtensions.GetAddStickersDeepLink("my_pack");            // tg://addstickers?set=my_pack
TelegramLinkExtensions.GetResolveDeepLink("durov");                  // tg://resolve?domain=durov
TelegramLinkExtensions.GetJoinDeepLink("+AbCdEf");                  // tg://join?invite=AbCdEf
TelegramLinkExtensions.GetMessageDeepLink("Hello world");            // tg://msg?text=Hello%20world
```

> Empty or whitespace-only arguments throw `ArgumentException`.

## When to Use Which

- **Inline mention / button that opens a user profile**: `user.GetUserLink()`
- **Share a public channel or user profile outside Telegram**: `user.GetPublicLink()` or `chat.GetPublicLink()`
- **Link to a specific message in a public channel**: `message.GetMessageLink()`
- **Share button that pre-fills a URL and text**: `GetShareUrl(url, text)`
- **Invite user to a private group**: `GetJoinDeepLink(hash)`
- **Pre-fill a message before sending**: `GetMessageDeepLink(text)`
