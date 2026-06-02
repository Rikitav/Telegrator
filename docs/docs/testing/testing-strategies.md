---
title: "Testing Strategies"
description: "Unit-test handlers in isolation and integration-test the full routing pipeline."
---

# Testing Strategies

Telegrator provides multiple levels of testing, from isolated unit tests for a single handler to full-stack integration tests that exercise routing, filters, and state machines without connecting to Telegram.

## Unit Testing

The simplest way to test a handler is to instantiate it directly and mock its dependencies. Handlers expose an `Execute(IHandlerContainer<T>, CancellationToken)` method that you can call in a test.

```csharp
[Fact]
public async Task StartHandler_ShouldReplyWithWelcome()
{
    // Arrange
    var handler = new StartHandler();
    var mockContainer = new Mock<IHandlerContainer<Message>>();
    mockContainer.Setup(c => c.Update).Returns(new Message
    {
        Chat = new Chat { Id = 123456 },
        From = new User { Id = 123456, FirstName = "Alice" }
    });

    // Act
    var result = await handler.Execute(mockContainer.Object, CancellationToken.None);

    // Assert
    result.Success.Should().BeTrue();
    result.RouteNext.Should().BeFalse();
}
```

### What to Assert

| Assertion | Meaning |
|-----------|---------|
| `result.Success` | The handler completed without faulting |
| `result.RouteNext` | The router should continue to the next handler |
| `mockClient.Verify(...)` | The handler called the expected Bot API method |

### Mocking the Container

`IHandlerContainer<T>` carries everything the handler needs: the update, the client, extra data, and completed filters. With Moq you can set up only the properties your handler actually touches:

```csharp
var mockContainer = new Mock<IHandlerContainer<Message>>();
mockContainer.Setup(c => c.Update).Returns(message);
mockContainer.Setup(c => c.Client).Returns(mockClient.Object);
```

## Integration Testing

When you need to verify that **filters, routing, and state machines** work together, use the `Telegrator.Testing` package. It mocks `ITelegramBotClient` and pushes synthetic updates through the real `UpdateRouter`.

### Stand-Alone Integration Tests

Use `TestTelegratorClient` when you do not need the .NET Generic Host (no DI, no `ILogger`):

```csharp
var testClient = new TestTelegratorClient();
testClient.Handlers.AddHandler<StartHandler>();
testClient.StartTestReceiving();

await testClient.EmitMessageAsync(new Message
{
    Text = "/start",
    Chat = new Chat { Id = 42, Type = ChatType.Private },
    From = new User { Id = 42, FirstName = "Alice" }
});

testClient.ClientMock.Verify(
    c => c.SendTextMessageAsync(
        It.Is<ChatId>(id => id.Identifier == 42),
        It.Is<string>(s => s.Contains("Welcome")),
        It.IsAny<int?>(), It.IsAny<ParseMode>(), It.IsAny<IEnumerable<MessageEntity>>(),
        It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<int?>(),
        It.IsAny<bool>(), It.IsAny<IReplyMarkup>(), It.IsAny<CancellationToken>()),
    Times.Once);
```

### Hosted Integration Tests

Use `TelegratorTestServer` when your bot relies on Dependency Injection, logging, or scoped services:

```csharp
var builder = Host.CreateApplicationBuilder(args);
builder.AddTelegrator(new TelegratorOptions())
    .WithTestServer()
    .Handlers.CollectHandlers();

var host = builder.Build();
host.UseTelegrator();
await host.StartAsync();

var testServer = host.GetTestServer();
await testServer.EmitMessageAsync(new Message { Text = "/start", ... });
```

See the dedicated <a href="./integration-testing">Integration Testing</a> article for the full API.

## Testing Aspects

Aspects (`IPreProcessor` / `IPostProcessor`) should be tested in isolation just like handlers:

```csharp
[Fact]
public async Task LoggingAspect_ShouldLogHandlerName()
{
    var aspect = new LoggingAspect(mockLogger.Object);
    var container = new Mock<IHandlerContainer>().Object;

    var result = await aspect.BeforeExecution(container, CancellationToken.None);

    result.Success.Should().BeTrue();
    mockLogger.VerifyLogContains("Executing handler");
}
```

## Testing Stateful Flows

State machines span multiple updates, so integration tests are usually required. Use `TestTelegratorClient` or `TelegratorTestServer` to simulate the full conversation:

```csharp
// Step 1
await testServer.EmitMessageAsync(new Message { Text = "/quiz", Chat = chat, From = user });

// Step 2
await testServer.EmitMessageAsync(new Message { Text = "Paris", Chat = chat, From = user });

// Assert both bot replies
testServer.ClientMock.Verify(
    c => c.SendTextMessageAsync(It.IsAny<ChatId>(), It.IsAny<string>(), ...),
    Times.Exactly(2));
```

## Best Practices

- **Prefer unit tests for business logic** — test handler methods directly with mocked containers
- **Use integration tests for routing & state** — verify that filters and state machines transition correctly
- **Verify side effects, not return values** — handlers communicate with Telegram via `ITelegramBotClient`; assert on `ClientMock.Verify(...)`
- **Reset state between tests** — create a fresh `TestTelegratorClient` or restart the host for each test
- **Do not test the framework** — you do not need to test `UpdateRouter` or `UpdateHandlersPool`; test your own handlers and filters
