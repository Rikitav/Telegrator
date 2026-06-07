---
title: "Configuration"
description: "Cross-cutting configuration guide for all Telegrator deployment scenarios."
---

# Configuration

Telegrator uses standard .NET Configuration. This means you can use `appsettings.json`, environment variables, command-line arguments, Azure Key Vault, and any other `IConfigurationProvider`.

## Configuration Sections Overview

| Section | Options Type | Used By | Required For |
|---------|--------------|---------|--------------|
| `"Telegrator"` or `"TelegratorOptions"` | `TelegratorOptions` | All hosting modes | Always |
| `"Receiver"` or `"ReceiverOptions"` | `ReceiverOptions` | Long-polling (`.WithPolling()`) | Polling bots |
| `"WebhookerOptions"` or `"Webhooker"` | `WebhookerOptions` | Webhooks (`.WithWeb()`) | Webhook bots |

## `TelegratorOptions` (Always Required)

Controls the bot token, retry behavior, routing, and concurrency.

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    "BaseUrl": null,
    "UseTestEnvironment": false,
    "RetryThreshold": 60,
    "RetryCount": 3,
    "MaximumParallelWorkingHandlers": 20,
    "ExclusiveAwaitingHandlerRouting": false,
    "ExceptIntersectingCommandAliases": true
  }
}
```

See the [Hosting Configuration](../hosting/configuration.md) guide for the full reference.

## `ReceiverOptions` (Polling Only)

Controls long-polling behavior.

```json
{
  "Receiver": {
    "Offset": 0,
    "Limit": 100,
    "DropPendingUpdates": true,
    "AllowedUpdates": ["Message", "CallbackQuery"]
  }
}
```

See the [Hosting Configuration](../hosting/configuration.md) guide for the full reference.

## `WebhookerOptions` (Webhooks Only)

Controls webhook URL, secret token, and connection limits.

```json
{
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "MySecretToken",
    "MaxConnections": 40,
    "DropPendingUpdates": false
  }
}
```

See the [Webhook Configuration](../web-hosting/configuration.md) guide for the full reference.

## `WideBotOptions` (MTProto / WideBot Only)

Controls MTProto credentials, proxy, and session behavior.

```json
{
  "WideBotOptions": {
    "ApiId": 123456,
    "ApiHash": "YOUR_API_HASH_FROM_MY_TELEGRAM_ORG",
    "DropPendingUpdates": true,
    "MTProxy": null
  }
}
```

See the [WideBot Configuration](../wide-bot/configuration.md) guide for the full reference.

## Configuration Precedence

.NET Configuration uses the following precedence (last wins):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (Development only)
4. Environment Variables
5. Command-Line Arguments

Example: if `appsettings.json` sets `Token = "A"` and an environment variable sets `Telegrator__Token = "B"`, the environment variable wins.

## Docker & Cloud Deployment

Use environment variables for containerized deployments:

```bash
# Required for all modes
Telegrator__Token=123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11

# Polling-specific
Receiver__DropPendingUpdates=true
Receiver__Limit=100

# Webhook-specific
WebhookerOptions__WebhookUri=https://mybot.example.com/api/telegrator/update
WebhookerOptions__SecretToken=MySecret
```

In `docker-compose.yml`:

```yaml
services:
  bot:
    image: mybot:latest
    environment:
      - Telegrator__Token=${BOT_TOKEN}
      - Telegrator__MaximumParallelWorkingHandlers=20
      # Polling-specific
      - Receiver__DropPendingUpdates=true
      # Webhook-specific
      - WebhookerOptions__WebhookUri=${WEBHOOK_URL}
      - WebhookerOptions__SecretToken=${WEBHOOK_SECRET}
      # WideBot-specific
      - WideBotOptions__ApiId=${WIDE_API_ID}
      - WideBotOptions__ApiHash=${WIDE_API_HASH}
```

## Kubernetes

Store sensitive values in a Secret and reference them in the Deployment:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: bot-secrets
type: Opaque
stringData:
  token: "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11"
  wide-api-hash: "YOUR_API_HASH"
---
apiVersion: apps/v1
kind: Deployment
metadata:
  name: bot
spec:
  template:
    spec:
      containers:
        - name: bot
          image: mybot:latest
          env:
            - name: Telegrator__Token
              valueFrom:
                secretKeyRef:
                  name: bot-secrets
                  key: token
            - name: Telegrator__MaximumParallelWorkingHandlers
              value: "20"
            # WideBot-specific
            - name: WideBotOptions__ApiId
              value: "123456"
            - name: WideBotOptions__ApiHash
              valueFrom:
                secretKeyRef:
                  name: bot-secrets
                  key: wide-api-hash
```

## Azure App Service / Container Apps

In Azure Portal → Configuration → Application Settings, add:

| Name | Value |
|------|-------|
| `Telegrator__Token` | `123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11` |
| `Telegrator__MaximumParallelWorkingHandlers` | `20` |
| `WebhookerOptions__WebhookUri` | `https://mybot.azurewebsites.net/api/telegrator/update` |

Azure automatically surfaces these as environment variables.

## Multiple Bots in One App

Telegrator's Hosting system is designed for **one bot per process** by default. For multiple bots, create separate `TelegratorClient` instances manually or use separate Generic Hosts.

If you must host multiple bots in one ASP.NET Core app, manually create `TelegratorClient` instances and manage their lifetimes yourself:

```csharp
var bot1 = new TelegratorClient(token1, new TelegratorOptions { ... });
var bot2 = new TelegratorClient(token2, new TelegratorOptions { ... });
```

## Next Steps
- [Hosting Configuration](../hosting/configuration.md) — detailed polling configuration.
- [Webhook Configuration](../web-hosting/configuration.md) — detailed webhook configuration.
- [Deployment](deployment.md) — deployment strategies for Docker, Kubernetes, and cloud.
