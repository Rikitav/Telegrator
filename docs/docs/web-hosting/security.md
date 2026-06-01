---
title: "Security & Validation"
description: "Protecting your webhook endpoint from unauthorized requests."
---

# Webhook Security

Because your webhook endpoint is a public URL, anyone could technically send malicious data to it. Telegrator provides several mechanisms to verify that requests are coming from Telegram.

## Secret Token Verification

Telegram allows you to set a `secret_token` when registering a webhook. Telegram will then include this token in the `X-Telegram-Bot-Api-Secret-Token` header of every request.

Telegrator automatically validates this token if you configure it:

```csharp
builder.Services.Configure<TelegramBotWebOptions>(options => {
    options.SecretToken = "YourSuperSecretToken123!";
});
```

If the header is missing or doesn't match, Telegrator will return a `403 Forbidden` response and ignore the update.

## IP Filtering

While `secret_token` is usually enough, some enterprises require IP-based filtering. Telegrator doesn't include IP whitelists by default (as Telegram IPs can change), but you can easily add middleware before `UseTelegratorWeb`:

```csharp
app.Use(async (context, next) => {
    var ip = context.Connection.RemoteIpAddress;
    // Validate IP against Telegram's CIDR ranges
    await next();
});

app.UseTelegratorWeb();
```

## Handling Re-deliveries

If your handler takes too long (timeout) or returns an error, Telegram will retry the update. 
- **Telegrator behavior**: Immediately returns `200 OK` to Telegram and queues the update for processing in the background, minimizing re-deliveries.

## Restrictions
- **Maximum update size**: 2MB (Telegram's limit).
- **Endpoint sharing**: Do not share the same webhook URL for multiple different bots.
