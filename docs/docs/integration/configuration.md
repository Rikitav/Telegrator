---
title: "Configuration Management"
description: "Configuration files and appsettings bindings."
---

# Configuration Management

The hosting integration provides comprehensive configuration management through standard .NET configuration patterns.

## Required Configuration

The bot token must be configured either in `appsettings.json` or through environment variables:

```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN"
  }
}
```

## Environment Variables
You can also use environment variables for sensitive configuration:
```shell
export TelegramBotClientOptions__Token="YOUR_BOT_TOKEN"
```

## Advanced Configuration
```json
{
  "TelegratorOptions": {
    "Token": "YOUR_BOT_TOKEN",
    "BaseUrl": "https://api.telegram.org"
  },
  
  "WebhookerOptions": {
    "WebhookUri": "https://your-domain.com/webhook",
    "SecretToken": "your-secret-token",
    "MaxConnections": 40,
    "DropPendingUpdates": true
  },

  "HostOptions": {
    "ShutdownTimeout": 10,
    "BackgroundServiceExceptionBehavior": "StopHost"
  },

  "ReceiverOptions": {
    "DropPendingUpdates": true,
    "Limit": 10,
    "Timeout": 30
  },

  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Telegrator": "Debug"
    }
  }
}
```

> **How is it working?**
> 1. **Automatic Configuration Binding**: The hosting integration automatically binds configuration sections to their respective options classes.
> 2. **Environment Variable Support**: Configuration can be overridden using environment variables with the `__` separator.
> 3. **Hierarchical Configuration**: Supports multiple configuration sources (appsettings.json, environment variables, command line, etc.).
> 4. **Type Safety**: Configuration is strongly typed and validated at startup.
> 5. **Production Ready**: Sensitive data like bot tokens can be securely managed through environment variables or secret management systems.
