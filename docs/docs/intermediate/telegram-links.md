# Telegram Link Generation

Telegrator provides a set of extension methods that make it easy to build Telegram-specific URLs and deep links directly from framework types such as <code>User</code>, <code>Chat</code>, and <code>Message</code>. This avoids hard-coding URL templates throughout your handlers and keeps link generation consistent.

## User Links

<table>
<thead>
<tr><th>Method</th><th>Return Type</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>GetUserLink()</code></td><td><code>string</code></td><td>Returns a <code>tg://user?id=...</code> deep link that opens the user's profile inside Telegram.</td></tr>
<tr><td><code>GetPublicLink()</code></td><td><code>string?</code></td><td>Returns <code>https://t.me/username</code> when the user has a username; otherwise <code>null</code>.</td></tr>
<tr><td><code>GetDeepLink()</code></td><td><code>string</code></td><td>Alias for <code>GetUserLink()</code>.</td></tr>
</tbody>
</table>

```csharp
User user = new User { Id = 123456, Username = "durov" };

string deep = user.GetUserLink();        // tg://user?id=123456
string? pub = user.GetPublicLink();      // https://t.me/durov
```

> Passing a <code>null</code> <code>User</code> to any of these methods throws <code>ArgumentNullException</code>.

## Chat Links

<table>
<thead>
<tr><th>Method</th><th>Return Type</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>GetPublicLink()</code></td><td><code>string?</code></td><td>Returns <code>https://t.me/username</code> when the chat has a username; otherwise <code>null</code>.</td></tr>
</tbody>
</table>

```csharp
Chat chat = new Chat { Id = 1, Username = "mychannel" };
string? link = chat.GetPublicLink();     // https://t.me/mychannel
```

> Passing a <code>null</code> <code>Chat</code> throws <code>ArgumentNullException</code>.

## Message Links

<table>
<thead>
<tr><th>Method</th><th>Return Type</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>GetMessageLink()</code></td><td><code>string?</code></td><td>Returns <code>https://t.me/username/messageId</code> when the containing chat has a username; otherwise <code>null</code>.</td></tr>
</tbody>
</table>

```csharp
Message message = new Message
{
    MessageId = 42,
    Chat = new Chat { Id = 1, Username = "channel" }
};

string? link = message.GetMessageLink(); // https://t.me/channel/42
```

> Passing a <code>null</code> <code>Message</code> throws <code>ArgumentNullException</code>.

## String Helpers

These extensions work on plain <code>string</code> values and normalize the input automatically (trim whitespace, strip leading <code>@</code> or <code>+</code>).

<table>
<thead>
<tr><th>Method</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>ToTelegramPublicUrl()</code></td><td>Converts a username into <code>https://t.me/username</code>.</td></tr>
<tr><td><code>ToTelegramInviteUrl()</code></td><td>Converts an invite hash into <code>https://t.me/+hash</code>.</td></tr>
</tbody>
</table>

```csharp
"@durov".ToTelegramPublicUrl();          // https://t.me/durov
"+AbCdEf".ToTelegramInviteUrl();         // https://t.me/+AbCdEf
```

> Empty or whitespace-only input throws <code>ArgumentException</code>.

## Share URLs

<table>
<thead>
<tr><th>Method</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>GetShareUrl(url, text?)</code></td><td>Builds a <code>https://t.me/share/url?url=...</code> link with optional pre-filled text. Both parameters are URL-encoded automatically.</td></tr>
</tbody>
</table>

```csharp
TelegramLinkExtensions.GetShareUrl("https://example.com", "Check this");
// https://t.me/share/url?url=https%3A%2F%2Fexample.com&text=Check%20this%20out
```

## Deep Links

Static helpers that return well-known Telegram deep links.

<table>
<thead>
<tr><th>Method</th><th>Returns</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td><code>GetSettingsDeepLink()</code></td><td><code>tg://settings</code></td><td>Opens Telegram settings.</td></tr>
<tr><td><code>GetAddStickersDeepLink(setName)</code></td><td><code>tg://addstickers?set=...</code></td><td>Opens the sticker pack installation screen.</td></tr>
<tr><td><code>GetResolveDeepLink(username)</code></td><td><code>tg://resolve?domain=...</code></td><td>Opens a profile by username.</td></tr>
<tr><td><code>GetJoinDeepLink(inviteHash)</code></td><td><code>tg://join?invite=...</code></td><td>Invites the user to a group or channel.</td></tr>
<tr><td><code>GetMessageDeepLink(text)</code></td><td><code>tg://msg?text=...</code></td><td>Pre-fills the message text field.</td></tr>
</tbody>
</table>

```csharp
TelegramLinkExtensions.GetSettingsDeepLink();                        // tg://settings
TelegramLinkExtensions.GetAddStickersDeepLink("my_pack");            // tg://addstickers?set=my_pack
TelegramLinkExtensions.GetResolveDeepLink("durov");                  // tg://resolve?domain=durov
TelegramLinkExtensions.GetJoinDeepLink("+AbCdEf");                  // tg://join?invite=AbCdEf
TelegramLinkExtensions.GetMessageDeepLink("Hello world");            // tg://msg?text=Hello%20world
```

> Empty or whitespace-only arguments throw <code>ArgumentException</code>.

## When to Use Which

<table>
<thead>
<tr><th>Scenario</th><th>Recommended Method</th></tr>
</thead>
<tbody>
<tr><td>Inline mention / button that opens a user profile</td><td><code>user.GetUserLink()</code></td></tr>
<tr><td>Share a public channel or user profile outside Telegram</td><td><code>user.GetPublicLink()</code> or <code>chat.GetPublicLink()</code></td></tr>
<tr><td>Link to a specific message in a public channel</td><td><code>message.GetMessageLink()</code></td></tr>
<tr><td>Share button that pre-fills a URL and text</td><td><code>GetShareUrl(url, text)</code></td></tr>
<tr><td>Invite user to a private group</td><td><code>GetJoinDeepLink(hash)</code></td></tr>
<tr><td>Pre-fill a message before sending</td><td><code>GetMessageDeepLink(text)</code></td></tr>
</tbody>
</table>
