---
title: "Integration Testing"
description: "Test your bot end-to-end without hitting the real Telegram API using Telegrator.Testing."
---

# Integration Testing with Telegrator.Testing

The `Telegrator.Testing` package provides a lightweight, in-memory testing infrastructure for Telegrator bots. It mocks the underlying `ITelegramBotClient` and lets you push synthetic updates through the real routing pipeline — including filters, aspects, and state management — without connecting to Telegram.

## Installation

```shell
dotnet add package Telegrator.Testing
```

The package ships with two complementary APIs:

- **`TestTelegratorClient`**
  - Target: `netstandard2.0` &amp; `net10.0`
  - Use Case: Stand-alone testing without the .NET Generic Host.
- **`TelegratorTestServer`**
  - Target: `net10.0` only
  - Use Case: Integration testing inside the full hosting pipeline (DI, logging, configuration).

Both APIs expose a **mocked `ITelegramBotClient`** (`ClientMock`) so you can verify that your handlers sent messages, edited keyboards, or performed any other Bot API call.

---

## Stand-Alone Testing with `TestTelegratorClient`

Use this approach when you want to test handlers in isolation or in a custom harness without spinning up `IHost`.

### 1. Create the Client

```csharp
using Telegrator.Testing;
using Telegram.Bot.Types;

var testClient = new TestTelegratorClient(
    telegratorOptions: new TelegratorOptions(),
    botUser: new User { Id = 1, IsBot = true, FirstName = "TestBot", Username = "test_bot" }
);
```

### 2. Register Handlers

You can register handlers exactly as you would in production:

```csharp
testClient.Handlers.AddHandler<MyCommandHandler>();
// or
testClient.Handlers.CollectHandlers();
```

### 3. Start the Test Router

```csharp
testClient.StartTestReceiving();
```

This builds the internal `UpdateRouter`, `HandlersProvider`, `AwaitingProvider`, and `DefaultStateStorage`.

### 4. Emit Updates

Push a raw `Update` into the pipeline:

```csharp
var update = new Update
{
    Message = new Message
    {
        MessageId = 1,
        Text = "/start",
        Chat = new Chat { Id = 123456, Type = ChatType.Private },
        From = new User { Id = 123456, FirstName = "Alice" }
    }
};

await testClient.EmitUpdateAsync(update);
```

Or use the convenience helper for messages:

```csharp
await testClient.EmitMessageAsync(new Message
{
    MessageId = 2,
    Text = "hello",
    Chat = new Chat { Id = 123456, Type = ChatType.Private },
    From = new User { Id = 123456, FirstName = "Alice" }
});
```

### 5. Verify Bot API Calls

Because the client is a Moq mock, you can verify any interaction:

```csharp
testClient.ClientMock.Verify(
    c => c.SendMessage(
        It.Is<ChatId>(id => id.Identifier == 123456),
        "Welcome!",
        It.IsAny<int?>(),
        It.IsAny<ParseMode>(),
        It.IsAny<IEnumerable<MessageEntity>>(),
        It.IsAny<bool>(),
        It.IsAny<bool>(),
        It.IsAny<bool>(),
        It.IsAny<int?>(),
        It.IsAny<bool>(),
        It.IsAny<IReplyMarkup>(),
        It.IsAny<CancellationToken>()
    ),
    Times.Once
);
```

---

## Hosted Integration Testing with `TelegratorTestServer`

When your bot relies on the .NET Generic Host — for dependency injection, `ILogger`, `IConfiguration`, or hosted services — use `TelegratorTestServer` to test the **full stack** without network I/O.

### 1. Configure the Host for Testing

Replace the standard `WithPolling()` or `WithWebhook()` call with `WithTestServer()`:

```csharp
using Telegrator.Hosting;
using Telegrator.Testing.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.AddTelegrator(options: new TelegratorOptions())
    .WithTestServer()          // <-- replaces WithPolling / WithWebhook
    .Handlers.CollectHandlers();

var host = builder.Build();
host.UseTelegrator();
```

`WithTestServer` does three things under the hood:
1. Replaces the real `ITelegramBotClient` with a Moq mock.
2. Stubs `GetMe` so the bot info is available immediately.
3. Removes all `IHostedService` receivers (long-polling & webhooks) so the host does not try to open network connections.

### 2. Retrieve the Test Server

```csharp
var testServer = host.GetTestServer();
```

You can resolve the server either before or after starting the host, depending on your test framework:

```csharp
await host.StartAsync();
var testServer = host.GetTestServer();
```

### 3. Emit Updates Through the Hosted Pipeline

The API is identical to the stand-alone client:

```csharp
var update = new Update
{
    Message = new Message
    {
        MessageId = 1,
        Text = "/help",
        Chat = new Chat { Id = 999, Type = ChatType.Private },
        From = new User { Id = 999, FirstName = "Bob" }
    }
};

await testServer.EmitUpdateAsync(update);
```

### 4. Verify DI-Resolved Behaviors

Because the full host is running, scoped services (e.g. `DbContext`) are created and disposed per update just like in production. You can assert side effects in the database or verify that the mock client was called:

```csharp
testServer.ClientMock.Verify(
    c => c.SendTextMessageAsync(
        It.Is<ChatId>(id => id.Identifier == 999),
        It.Is<string>(s => s.Contains("Help")),
        It.IsAny<int?>(),
        It.IsAny<ParseMode>(),
        It.IsAny<IEnumerable<MessageEntity>>(),
        It.IsAny<bool>(),
        It.IsAny<bool>(),
        It.IsAny<bool>(),
        It.IsAny<int?>(),
        It.IsAny<bool>(),
        It.IsAny<IReplyMarkup>(),
        It.IsAny<CancellationToken>()
    ),
    Times.Once
);
```

---

## Testing Stateful Flows

Both APIs route updates through the real `UpdateRouter`, so state machines work out of the box.

```csharp
// Step 1: user sends /wizard
await testServer.EmitMessageAsync(new Message
{
    Text = "/wizard",
    Chat = new Chat { Id = 42, Type = ChatType.Private },
    From = new User { Id = 42, FirstName = "Alice" }
});

// Step 2: user replies with her name
await testServer.EmitMessageAsync(new Message
{
    Text = "Alice",
    Chat = new Chat { Id = 42, Type = ChatType.Private },
    From = new User { Id = 42, FirstName = "Alice" }
});

// Verify both bot responses
testServer.ClientMock.Verify(
    c => c.SendTextMessageAsync(
        It.Is<ChatId>(id => id.Identifier == 42),
        It.IsAny<string>(),
        It.IsAny<int?>(), It.IsAny<ParseMode>(), It.IsAny<IEnumerable<MessageEntity>>(),
        It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int?>(),
        It.IsAny<bool>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()
    ),
    Times.Exactly(2)
);
```

---

## Best Practices

- **Reset state between tests** — `TestTelegratorClient` creates a fresh `DefaultStateStorage` on `StartTestReceiving`, but if you reuse the same instance, create a new one per test.
- **Prefer `EmitMessageAsync`** for message-based tests; it wraps the message in an `Update` automatically.
- **Verify on `ClientMock`** rather than checking return values, because handlers communicate with Telegram via side effects (sending messages, editing keyboards, etc.).
- **Use `host.StartAsync()` / `host.StopAsync()`** in hosted tests to ensure scoped services are disposed correctly.
- **Keep tests synchronous where possible** — `EmitUpdateAsync` awaits handler completion, so you can assert immediately after the call.

---

## Summary

- **Unit-test a single handler**: Instantiate handler directly + mock `IHandlerContainer&lt;T&gt;`
- **Test routing, filters &amp; state without DI**: `TestTelegratorClient`
- **Test full hosting pipeline (DI, logging, scoped services)**: `TelegratorTestServer` via `WithTestServer()`

The `Telegrator.Testing` package bridges the gap between isolated unit tests and slow, flaky production API tests, giving you fast, deterministic confidence in your bot's behavior.
