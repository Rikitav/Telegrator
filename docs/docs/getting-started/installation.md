---
title: "Installation"
description: "How to install Telegrator and prerequisites."
---

# Installation

**Telegrator** is distributed as a NuGet package. You can install it using the .NET CLI, the NuGet Package Manager Console, or by managing NuGet packages in Visual Studio.

## Prerequisites
- .NET >= 5.0 `or` .NET Core >= 2.0 `or` Framework >= 4.6.1 (.NET Standard 2.1 compatible)
- A Telegram Bot Token from [@BotFather](https://t.me/BotFather).

## .NET CLI
```shell
dotnet add package Telegrator
```

## Package Manager Console
```shell
Install-Package Telegrator
```

## Hosting & Extensions
- `Telegrator.Hosting`: Integration with .NET Generic Host.
- `Telegrator.Hosting.Web`: Webhook support for ASP.NET Core.
- `Telegrator.Hosting.WideBot`: MTProto support via WTelegramBot.
- `Telegrator.RedisStateStorage`: Persistent state storage using Redis.

```shell
# For console/background services
dotnet add package Telegrator.Hosting

# For webhook hosting
dotnet add package Telegrator.Hosting.Web

# For MTProto support
dotnet add package Telegrator.Hosting.WideBot

# For Redis state storage
dotnet add package Telegrator.RedisStateStorage
```
