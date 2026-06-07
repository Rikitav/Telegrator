---
title: "Webhook Configuration"
description: "Complete guide to configuring Telegrator.Hosting.Web via appsettings.json, code, and environment variables."
---

# Webhook Configuration

`Telegrator.Hosting.Web` can be configured in three ways (in order of precedence):

1. **Code-first** — passing objects directly in `Program.cs`.
2. **`appsettings.json`** — standard .NET configuration binding.
3. **Environment variables** — for Docker, CI/CD, and cloud deployments.

---

## Configuration Sections

The Webhook addon reads from the **`WebhookerOptions`** configuration section.

### Primary and Fallback Section Names

- **`WebhookerOptions`**
  - Primary Section: `"WebhookerOptions"`
  - Fallback Section: `"Webhooker"`

If neither section exists in configuration, `WithWeb()` throws a `MissingMemberException` unless you have already registered `IOptions<WebhookerOptions>` manually.

---

## `WebhookerOptions` Reference

- **`WebhookUri`**
  - Type: `string?`
  - Default: `null`
  - Description: The public HTTPS URL where Telegram will send updates. **Required.** Example: `"https://mybot.example.com/api/telegrator/update"`
- **`SecretToken`**
  - Type: `string?`
  - Default: `null`
  - Description: Optional secret token. Telegram sends it in the `X-Telegram-Bot-Api-Secret-Token` header. If set, requests without this header are rejected with `401 Unauthorized`.
- **`MaxConnections`**
  - Type: `int`
  - Default: `40`
  - Description: Maximum number of simultaneous HTTPS connections allowed for the webhook. Passed to `setWebhook`.
- **`DropPendingUpdates`**
  - Type: `bool`
  - Default: `false`
  - Description: If `true`, pending updates are dropped when the webhook is set and when the application stops.

### `appsettings.json` Example

```json
{
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "MySuperSecretToken123",
    "MaxConnections": 50,
    "DropPendingUpdates": true
  }
}
```

Or using the fallback section name:

```json
{
  "Webhooker": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "MySuperSecretToken123"
  }
}
```

---

## Configuration Methods

### 1. `appsettings.json` (Recommended for Production)

The framework automatically binds the `"WebhookerOptions"` section when you call `.WithWeb()` without explicit options:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddTelegrator()
    .WithWeb() // Reads WebhookerOptions from appsettings.json automatically
    .Handlers.CollectHandlers();
```

Make sure your `appsettings.json` (or `appsettings.Production.json`) contains the section:

```json
{
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "..."
  }
}
```

### 2. `ConfigureWebhooker` Extension

Register options imperatively via `IServiceCollection`:

```csharp
builder.Services.ConfigureWebhooker(new WebhookerOptions
{
    WebhookUri = "https://mybot.example.com/api/telegrator/update",
    SecretToken = "MySecret",
    MaxConnections = 100,
    DropPendingUpdates = true
});

builder.AddTelegrator()
    .WithWeb()
    .Handlers.CollectHandlers();
```

> [!NOTE]
> `ConfigureWebhooker` replaces any previously registered `IOptions<WebhookerOptions>`. Call it **before** `.WithWeb()` or the replacement will override your manual config.

### 3. Inline in `Program.cs`

There is no direct "inline object" overload for `WithWeb()` (unlike `AddTelegrator(options: ...)`). Use `ConfigureWebhooker` instead.

### 4. Environment Variables

Use double-underscore (`__`) or colon (`:`) as section separators:

```bash
# Linux / macOS / Docker
export WebhookerOptions__WebhookUri="https://mybot.example.com/api/telegrator/update"
export WebhookerOptions__SecretToken="MySecret"
export WebhookerOptions__DropPendingUpdates="true"

# Windows PowerShell
$env:WebhookerOptions__WebhookUri="https://mybot.example.com/api/telegrator/update"
```

In a `docker-compose.yml`:

```yaml
services:
  bot:
    environment:
      - WebhookerOptions__WebhookUri=https://mybot.example.com/api/telegrator/update
      - WebhookerOptions__SecretToken=${WEBHOOK_SECRET}
```

---

## TelegratorOptions for Webhook Bots

Webhook bots still require `TelegratorOptions` (token, retry settings, etc.). These are configured exactly the same way as polling bots. See the [Hosting Configuration](../hosting/configuration.md) guide for details.

Quick example for webhook bots:

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    "RetryThreshold": 60,
    "RetryCount": 3,
    "MaximumParallelWorkingHandlers": 20,
    "ExclusiveAwaitingHandlerRouting": true
  },
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update",
    "SecretToken": "..."
  }
}
```

---

## Validation & Common Errors

- **`MissingMemberException`: Auto configuration enabled, yet no options of type 'WebhookerOptions' was registered.**
  - Cause: `appsettings.json` missing the `WebhookerOptions`/`Webhooker` section, and no `ConfigureWebhooker` call.
  - Fix: Add the section to `appsettings.json` or call `ConfigureWebhooker`.
- **`ArgumentNullException`: Option "WebhookUrl" must be set...**
  - Cause: `WebhookUri` is null or empty.
  - Fix: Set `WebhookUri` in configuration.
- **`InvalidOperationException`: `HostedUpdateReceiver` found in services...**
  - Cause: Both `.WithPolling()` and `.WithWeb()` were called on the same builder.
  - Fix: Remove `.WithPolling()` or use separate builders.
- **`400 Bad Request` on webhook endpoint**
  - Cause: Missing or invalid `X-Telegram-Bot-Api-Secret-Token` header.
  - Fix: Ensure the header matches `WebhookerOptions.SecretToken`.

---

## Behind a Proxy / Reverse Proxy

If your bot sits behind Nginx, Traefik, or a cloud load balancer, the `WebhookUri` must be the **public-facing URL**, not the internal container URL. The framework does not automatically handle `X-Forwarded-*` headers for webhook URL construction.

Example with Nginx:

```nginx
location /api/telegrator/update {
    proxy_pass http://localhost:5000/api/telegrator/update;
    proxy_set_header Host $host;
    proxy_set_header X-Real-IP $remote_addr;
}
```

Your `appsettings.json`:

```json
{
  "WebhookerOptions": {
    "WebhookUri": "https://mybot.example.com/api/telegrator/update"
  }
}
```

To remap the webhook at runtime (e.g. after deployment):

```csharp
app.UseTelegrator();
app.RemapWebhook("https://new-domain.com/api/telegrator/update");
await app.RunAsync();
```

> [!CAUTION]
> `RemapWebhook` sets a new webhook URL in Telegram **and** maps a new endpoint. It does not remove the old endpoint mapping from the ASP.NET Core pipeline. Use it for one-time migrations, not dynamic switching.
