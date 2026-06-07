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

In an ASP.NET Core application, use `.WithWeb()` during configuration and `UseTelegrator()` after building:

```csharp
using Telegrator.Hosting.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddTelegrator() // 1. Add Telegrator core
    .WithWeb()            // 2. Configure webhook receiving
    .Handlers.CollectHandlers(); // 3. Register handlers using source generator

var app = builder.Build();

// 4. Initialize Telegrator (webhook endpoint is mapped automatically)
app.UseTelegrator();

// 5. Optional: Remap webhook to a specific URL (e.g. behind a proxy)
// app.RemapWebhook("https://your-domain.com/api/telegrator/update");

await app.RunAsync();
```

> [!NOTE]
> The webhook endpoint is registered automatically via an `IStartupFilter` inside `.WithWeb()`. You do **not** need to call any extra endpoint mapping method after `builder.Build()`.

## Configuration

For a complete reference of every option, precedence rules, environment variables, and common errors, see the [Webhook Configuration](configuration.md) guide.

Quick example via `appsettings.json`:

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
  },
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "MySuperSecretToken123",
    "MaxConnections": 50,
    "DropPendingUpdates": true
  }
}
```

Or configure in code:

```csharp
builder.Services.ConfigureWebhooker(new WebhookerOptions
{
    WebhookUri = "https://mybot.example.com/api/telegrator/update",
    SecretToken = "MySecret",
    MaxConnections = 100
});

builder.AddTelegrator()
    .WithWeb()
    .Handlers.CollectHandlers();
```

## Advantages
- **Resources**: Doesn't keep an open connection idle.
- **Latency**: Updates reach the bot almost instantly.
- **Scalability**: Can be easily balanced behind Nginx or Cloud Load Balancers.

## Requirements
- **HTTPS**: Telegram only sends webhooks to secure URLs.
- **Valid SSL**: Self-signed certificates are only allowed if you manually upload your public key to Telegram.
- **Public URL**: Your server must be accessible from the internet (use `ngrok` for local development).

## Next Steps
- Learn about [Webhook Configuration](configuration.md) — every option explained.
- Read about [Webhook Optimization](optimization.md) — performance tuning.
- Read about [Webhook Security](security.md) — secret tokens and reverse proxies.
