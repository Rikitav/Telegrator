---
title: "WideBot Configuration"
description: "Complete guide to configuring Telegrator.Hosting.WideBot via appsettings.json, code, and environment variables."
---

# WideBot Configuration

`Telegrator.Hosting.WideBot` connects your bot through **MTProto** (the protocol used by the official Telegram apps) instead of the standard Bot API. This requires additional credentials (`ApiId`, `ApiHash`) and a database connection for WTelegramBot's internal session storage.

---

## Configuration Sections

The WideBot addon reads from the **`WideBotOptions`** configuration section.

### Primary and Fallback Section Names

| Options Type | Primary Section | Fallback Section |
|--------------|-----------------|------------------|
| `WideBotOptions` | `"WideBotOptions"` | `"WideBot"` |

If neither section exists in configuration, `WithWide()` throws a `MissingMemberException` unless you have already registered `IOptions<WideBotOptions>` manually.

---

## `WideBotOptions` Reference

| Property | Type | Required | Default | Description |
|----------|------|----------|---------|-------------|
| `ApiId` | `int` | **Yes** | — | Your App ID from [my.telegram.org](https://my.telegram.org). |
| `ApiHash` | `string` | **Yes** | — | Your App Hash from [my.telegram.org](https://my.telegram.org). |
| `DropPendingUpdates` | `bool` | No | `false` | If `true`, drops all pending updates on startup. |
| `MTProxy` | `string?` | No | `null` | MTProto proxy string. Example: `"https://1.2.3.4:443/secret"`. |
| `SqlCommands` | `WTelegram.SqlCommands` | No | `Detect` | Controls how WTelegramBot detects SQL commands. **Not parsed from JSON.** Set only in code. |

### `appsettings.json` Example

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

Or using the fallback section name:

```json
{
  "WideBot": {
    "ApiId": 123456,
    "ApiHash": "YOUR_API_HASH_FROM_MY_TELEGRAM_ORG"
  }
}
```

> [!WARNING]
> **Never commit `ApiId` and `ApiHash` to version control.** Use User Secrets in development and environment variables / secret managers in production.

---

## Getting `ApiId` and `ApiHash`

1. Go to [my.telegram.org](https://my.telegram.org).
2. Log in with your phone number.
3. Navigate to **API Development Tools**.
4. Create a new application (or use an existing one).
5. Copy the **App api_id** and **App api_hash** into your configuration.

These credentials are **tied to your Telegram account**. Keep them secret.

---

## Configuration Methods

### 1. `appsettings.json` (Recommended for Non-Secret Options)

Store non-sensitive options in `appsettings.json`:

```json
{
  "WideBotOptions": {
    "DropPendingUpdates": true,
    "MTProxy": null
  }
}
```

Then pass `ApiId` and `ApiHash` via User Secrets or environment variables:

```bash
dotnet user-secrets set "WideBotOptions:ApiId" "123456"
dotnet user-secrets set "WideBotOptions:ApiHash" "YOUR_API_HASH"
```

In `Program.cs`:

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator()
    .WithWide(provider => new SqliteConnection($"Data Source={database}"))
    .Handlers.CollectHandlers();
```

### 2. `ConfigureWideBot` Extension

Register options imperatively via `IServiceCollection`:

```csharp
builder.Services.ConfigureWideBot(new WideBotOptions
{
    ApiId = 123456,
    ApiHash = "YOUR_API_HASH",
    DropPendingUpdates = true,
    MTProxy = "https://1.2.3.4:443/secret"
});

builder.AddTelegrator()
    .WithWide(provider => new SqliteConnection($"Data Source={database}"))
    .Handlers.CollectHandlers();
```

> [!NOTE]
> `ConfigureWideBot` replaces any previously registered `IOptions<WideBotOptions>`. Call it **before** `.WithWide()` or the replacement will override your manual config.

### 3. `ConfigureWideTelegram` Extension

If you need full control over the underlying `WTelegramBotClientOptions`, use `ConfigureWideTelegram`:

```csharp
builder.Services.ConfigureWideTelegram(new WTelegramBotClientOptions(
    token: "123456:ABC-DEF...",
    apiId: 123456,
    apiHash: "YOUR_API_HASH"
)
{
    MTProxy = "https://1.2.3.4:443/secret",
    SqlCommands = WTelegram.SqlCommands.Explicit
});
```

> [!CAUTION]
> `ConfigureWideTelegram` bypasses the auto-merging of `TelegratorOptions` (token, retry, test env) into `WTelegramBotClientOptions`. You must set the token manually.

### 4. Environment Variables

Use double-underscore (`__`) as the section separator:

```bash
# Linux / macOS / Docker
export WideBotOptions__ApiId="123456"
export WideBotOptions__ApiHash="YOUR_API_HASH"
export WideBotOptions__DropPendingUpdates="true"
export WideBotOptions__MTProxy="https://1.2.3.4:443/secret"

# Windows PowerShell
$env:WideBotOptions__ApiId="123456"
$env:WideBotOptions__ApiHash="YOUR_API_HASH"
```

In a `docker-compose.yml`:

```yaml
services:
  bot:
    environment:
      - WideBotOptions__ApiId=${WIDE_API_ID}
      - WideBotOptions__ApiHash=${WIDE_API_HASH}
      - WideBotOptions__DropPendingUpdates=true
```

### 5. User Secrets (Development)

```bash
dotnet user-secrets init
dotnet user-secrets set "WideBotOptions:ApiId" "123456"
dotnet user-secrets set "WideBotOptions:ApiHash" "YOUR_API_HASH"
```

---

## Database Connection

WTelegramBot requires a database connection for internal session storage. The `WithWide()` method accepts a `Func<IServiceProvider, DbConnection>` factory.

### SQLite (Recommended for Development)

```csharp
builder.AddTelegrator()
    .WithWide(provider => new SqliteConnection("Data Source=wtgb.db"))
    .Handlers.CollectHandlers();
```

### SQLite with Path

```csharp
string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
string database = Path.Combine(appData, "Telegrator", "wtgb.db");

builder.AddTelegrator()
    .WithWide(provider => new SqliteConnection($"Data Source={database}"))
    .Handlers.CollectHandlers();
```

### PostgreSQL

```csharp
builder.AddTelegrator()
    .WithWide(provider => new NpgsqlConnection(
        provider.GetRequiredService<IConfiguration>().GetConnectionString("WTelegram")))
    .Handlers.CollectHandlers();
```

> [!NOTE]
> The connection is disposed automatically when the application shuts down.

---

## TelegratorOptions for WideBot

WideBot bots still require `TelegratorOptions` (token, retry settings, etc.). These are configured exactly the same way as polling bots. See the [Hosting Configuration](../hosting/configuration.md) guide for details.

Quick example:

```json
{
  "Telegrator": {
    "Token": "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11",
    "RetryThreshold": 60,
    "RetryCount": 3,
    "MaximumParallelWorkingHandlers": 20
  },
  "WideBotOptions": {
    "ApiId": 123456,
    "ApiHash": "YOUR_API_HASH",
    "DropPendingUpdates": true
  }
}
```

---

## Validation & Common Errors

| Error | Cause | Fix |
|-------|-------|-----|
| `MissingMemberException`: Auto configuration enabled, yet no options of type 'WideBotOptions'... | `WideBotOptions`/`WideBot` section missing and no `ConfigureWideBot` call. | Add the section to `appsettings.json` or call `ConfigureWideBot`. |
| `MissingMemberException`: ApiId or ApiHash is required. | `ApiId` is 0 or `ApiHash` is null/empty. | Set both values in configuration. |
| `InvalidOperationException`: `HostedUpdateReceiver` found in services... | Both `.WithPolling()` and `.WithWide()` were called. | Remove `.WithPolling()` — WideBot replaces it. |
| `InvalidOperationException`: `HostedUpdateWebhooker` found in services... | Both `.WithWeb()` and `.WithWide()` were called. | Remove `.WithWeb()` — WideBot replaces it. |
| Session file not found / authentication fails | First run requires interactive login (phone code). | Check WTelegramBot logs for the login prompt. |

---

## MTProto Proxy

If your network requires a proxy to reach Telegram servers, set the `MTProxy` option:

```json
{
  "WideBotOptions": {
    "MTProxy": "https://1.2.3.4:443/secret"
  }
}
```

Or in code:

```csharp
builder.Services.ConfigureWideBot(new WideBotOptions
{
    ApiId = 123456,
    ApiHash = "YOUR_API_HASH",
    MTProxy = "https://1.2.3.4:443/secret"
});
```

---

## Next Steps
- [WideBot Introduction](introduction.md) — what is MTProto and why use it.
- [Handling Updates](handling-updates.md) — differences between Bot API and MTProto updates.
- [Advanced Usage](advanced-usage.md) — custom TL layers, extended types.
