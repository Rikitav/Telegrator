# Telegrator (Core)

The heart of the Telegrator framework. Provides the mediator-based update routing system, aspect-oriented handlers, declarative filters, state management, and the awaiting mechanism.

---

## 📦 What's Inside

### Handlers
Independent, aspect-oriented modules that process Telegram updates. Each handler is a class that inherits from a base handler type (e.g., `MessageHandler`, `CommandHandler`, `CallbackQueryHandler`) and overrides the `Execute` method.

```csharp
[MessageHandler]
[TextContains("hello")]
public class HelloHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"Hello, {container.ActualUpdate.From?.FirstName}!");
        return Ok;
    }
}
```

### Filters
Declarative conditions that determine whether a handler should process an update. Filters are defined as attributes and evaluated by the mediator before handler execution.

Built-in filters include:
- `TextContains`, `TextStartsWith`, `TextEndsWith`
- `CommandAllias`
- `ChatType`
- `CallbackData`, `CallbackDataStartsWith`
- Regex-based filters
- Custom filters via `Filter<T>.If(...)`

### Routing & Mediation
The `UpdateRouter` dispatches incoming updates to appropriate handlers. It supports:
- **Awaiting handlers** — temporary handlers that wait for a specific follow-up update
- **Regular handlers** — persistent handlers registered at startup
- **Exclusive awaiting routing** — option to block regular handlers when awaiting handlers are active
- **Bounded channel backpressure** — `UpdateHandlersPool` uses `Channel.CreateBounded` with configurable capacity

### Result Pattern
Handlers return a lightweight `readonly record struct Result` to control routing:
- `Result.Ok()` — handler succeeded, stop routing
- `Result.Fault()` — handler failed, stop routing
- `Result.Next()` — handler skipped, continue to next handler
- `Result.Next<T>()` — route to a specific handler type

### State Management
Store and retrieve user/chat state without manual state machines.

```csharp
await StateStorage.GetStateMachine<SetupWizard>(HandlingUpdate).BySenderId().Advance(cancellation);
```

### Awaiting Mechanism
Pause handler execution and wait for the user's next message inline.

```csharp
await Reply("What is your name?");
var nextMessage = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
await Reply($"Hello, {nextMessage.Text}!");
```

### OpenTelemetry Integration
Automatic `ActivitySource` spans for:
- `Telegrator.UpdateRouter` — `HandleUpdate` span with `update.id` and `update.type` tags
- `Telegrator.UpdateHandlersPool` — `ProcessHandler` span with handler and update tags

---

## 🔧 Configuration

```csharp
var options = new TelegratorOptions
{
    Token = "<YOUR_BOT_TOKEN>",
    MaximumParallelWorkingHandlers = 10,   // Bounded channel capacity = 20
    ExclusiveAwaitingHandlerRouting = false
};

var bot = new TelegratorClient(options);
```

---

## 🔗 Related Packages

| Package | Purpose |
|---------|---------|
| `Telegrator.Hosting` | .NET Generic Host integration |
| `Telegrator.Hosting.Web` | ASP.NET Core webhook support |
| `Telegrator.Analyzers` | Source generators for AOT |
| `Telegrator.Testing` | Unit & integration test utilities |

---

## License

MIT
