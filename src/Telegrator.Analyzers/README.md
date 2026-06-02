# Telegrator.Analyzers

Roslyn source generators and analyzers for the Telegrator framework. Eliminates runtime reflection during handler discovery, enabling **Native AOT** compilation and improving startup performance.

---

## ✨ What It Does

The `HandlersCollectorGenerator` inspects your project at compile time, finds all classes marked with Telegrator handler attributes (e.g., `[MessageHandler]`, `[CommandHandler]`), and generates a static `CollectHandlers` extension method.

Instead of scanning assemblies at runtime with reflection:

```csharp
// Runtime reflection (slower, not AOT-friendly)
handlers.CollectHandlersAssemblyWide();
```

You call the generated method:

```csharp
// Compile-time generated (fast, AOT-friendly)
handlers.CollectHandlers();
```

---

## 🚀 Usage

### 1. Install the Package

```shell
dotnet add package Telegrator.Analyzers
```

### 2. Mark Your Handlers

```csharp
using Telegrator.Annotations;
using Telegrator.Handlers;

[MessageHandler]
public class EchoHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply($"Echo: {container.ActualUpdate.Text}");
        return Ok;
    }
}
```

### 3. Register via Generated Method

```csharp
var bot = new TelegratorClient("<TOKEN>");
bot.Handlers.CollectHandlers(); // Generated at compile time
bot.StartReceiving();
```

---

## 🔗 Hosting Integration

When `Telegrator.Hosting` is referenced, an additional overload is generated:

```csharp
builder.AddTelegrator()
    .WithPolling()
    .Handlers.CollectHandlers();
```

---

## ⚠️ Requirements

- Handlers must be `public` or `internal` classes with a parameterless constructor.
- The source generator matches handler attributes against their expected base class (e.g., `[MessageHandler]` → `MessageHandler`).

---

## License

MIT
