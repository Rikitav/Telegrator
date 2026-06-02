# Telegrator.Testing

Testing utilities and helpers for the Telegrator framework. Enables unit and integration testing of handlers, filters, and routing logic without connecting to the real Telegram API.

---

## ✨ Features

- `TestTelegratorClient` — in-memory test client with a mocked `ITelegramBotClient`
- `EmitMessageAsync` / `EmitUpdateAsync` — simulate incoming updates directly
- `ClientMock` — Moq mock for verifying bot API calls
- Pre-configured `UpdateRouter` with `HandlersProvider`, `AwaitingProvider`, and `DefaultStateStorage`

---

## 🚀 Quick Start

### Installation

```shell
dotnet add package Telegrator.Testing
```

### Integration Test Example

```csharp
using Telegrator.Testing;

public class MyBotTests
{
    [Fact]
    public async Task Handler_ShouldReply()
    {
        var client = new TestTelegratorClient();
        client.Handlers.AddHandler<MyHandler>();
        client.StartTestReceiving();

        var message = new Message
        {
            Id = 1,
            Text = "hello",
            Chat = new Chat { Id = 42, Type = ChatType.Private },
            From = new User { Id = 42, FirstName = "Alice" }
        };

        await client.EmitMessageAsync(message);

        // Verify behavior via static flags or mock calls
        MyHandler.Executed.Should().BeTrue();
    }
}
```

### Awaiting Handler Test

When testing handlers that use `AwaitMessage`, run the first update in the background to avoid deadlock:

```csharp
var handlerTask = Task.Run(async () => await client.EmitMessageAsync(triggerMessage));
await Task.Delay(200); // let handler register the awaiter

await client.EmitMessageAsync(replyMessage);
await handlerTask.WaitAsync(TimeSpan.FromSeconds(5));
```

---

## 🔗 Related Packages

| Package | Purpose |
|---------|---------|
| `Telegrator` | Core framework |
| `Telegrator.Tests` | Example test suite (reference implementation) |

---

## License

MIT
