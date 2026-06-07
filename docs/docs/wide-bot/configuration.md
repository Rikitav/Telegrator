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

<table>
<thead>
<tr><th>Options Type</th><th>Primary Section</th><th>Fallback Section</th></tr>
</thead>
<tbody>
<tr><td>`WideBotOptions`</td><td>`"WideBotOptions"`</td><td>`"WideBot"`</td></tr>
</tbody>
</table>

If neither section exists in configuration, `WithWide()` throws a `MissingMemberException` unless you have already registered `IOptions<WideBotOptions>` manually.

---

## `WideBotOptions` Reference

<table>
<thead>
<tr><th>Property</th><th>Type</th><th>Required</th><th>Default</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td>`ApiId`</td><td>`int`</td><td>**Yes**</td><td>—</td><td>Your App ID from [my.telegram.org](https://my.telegram.org).</td></tr>
<tr><td>`ApiHash`</td><td>`string`</td><td>**Yes**</td><td>—</td><td>Your App Hash from [my.telegram.org](https://my.telegram.org).</td></tr>
<tr><td>`DropPendingUpdates`</td><td>`bool`</td><td>No</td><td>`false`</td><td>If `true`, drops all pending updates on startup.</td></tr>
<tr><td>`MTProxy`</td><td>`string?`</td><td>No</td><td>`null`</td><td>MTProto proxy string. Example: `"https://1.2.3.4:443/secret"`.</td></tr>
<tr><td>`SqlCommands`</td><td>`WTelegram.SqlCommands`</td><td>No</td><td>`Detect`</td><td>Controls how WTelegramBot detects SQL commands. **Not parsed from JSON.** Set only in code.</td></tr>
</tbody>
</table>

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

<table>
<thead>
<tr><th>Error</th><th>Cause</th><th>Fix</th></tr>
</thead>
<tbody>
<tr><td>`MissingMemberException`: Auto configuration enabled, yet no options of type 'WideBotOptions'...</td><td>`WideBotOptions`/`WideBot` section missing and no `ConfigureWideBot` call.</td><td>Add the section to `appsettings.json` or call `ConfigureWideBot`.</td></tr>
<tr><td>`MissingMemberException`: ApiId or ApiHash is required.</td><td>`ApiId` is 0 or `ApiHash` is null/empty.</td><td>Set both values in configuration.</td></tr>
<tr><td>`InvalidOperationException`: `HostedUpdateReceiver` found in services...</td><td>Both `.WithPolling()` and `.WithWide()` were called.</td><td>Remove `.WithPolling()` — WideBot replaces it.</td></tr>
<tr><td>`InvalidOperationException`: `HostedUpdateWebhooker` found in services...</td><td>Both `.WithWeb()` and `.WithWide()` were called.</td><td>Remove `.WithWeb()` — WideBot replaces it.</td></tr>
<tr><td>Session file not found / authentication fails</td><td>First run requires interactive login (phone code).</td><td>Check WTelegramBot logs for the login prompt.</td></tr>
</tbody>
</table>

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
