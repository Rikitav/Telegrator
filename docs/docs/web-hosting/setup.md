---
title: "Webhook Setup"
description: "Configuring Webhooks for ASP.NET Core applications."
---

# Webhook Fundamentals

`Telegrator.Hosting.Web` allows your bot to receive updates via Webhooks instead of long-polling. This is highly recommended for production environments as it's more efficient and reactive.

## Installation

```shell
dotnet add package Telegrator.Hosting.Web
```

## Basic Usage

In an ASP.NET Core application, use `AddTelegratorWeb` and `UseTelegratorWeb`:

```csharp
using Telegrator.Hosting.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelegrator() // 1. Add Telegrator
    .WithWeb(provider => new SqliteConnection($"Data Source={database}")) // 2. Add update receiving method
    .Handlers.CollectHandlers(); // 3. Register handlers using source generator

var app = builder.Build();

// 4. Map the webhook endpoint and initialize
app.UseTelegrator();

// 5. Optional: Remap webhook to specific URL (e.g. for behind a proxy or cloud host)
// app.RemapWebhook("https://your-domain.com/api/telegrator/update");

await app.RunAsync();
```

> [!CAUTION]
> **Obsolete Method**: `AddTelegratorWeb()` and `UseTelegratorWeb()` are now replaced by the unified fluent API. Use `.WithWeb()` and `UseTelegrator()` instead.

## Configuration Options

You can customize the webhook behavior using `TelegramBotWebOptions`:

```csharp
builder.Services.Configure<TelegramBotWebOptions>(options => {
    options.EndpointPath = "/my/secret/path";
    options.SecretToken = "A_Secure_Token";
});
```

## Advantages
- **Resources**: Doesn't keep an open connection idle.
- **Latency**: Updates reach the bot almost instantly.
- **Scalability**: Can be easily balanced behind Nginx or Cloud Load Balancers.

## Requirements
- **HTTPS**: Telegram only sends webhooks to secure URLs.
- **Valid SSL**: Self-signed certificates are only allowed if you manually upload your public key to Telegram.
- **Public URL**: Your server must be accessible from the internet (use `ngrok` for local development).
