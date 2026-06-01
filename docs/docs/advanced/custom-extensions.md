---
title: "Custom Extensions"
description: "Extend filters, state keepers, and more."
---

# Custom Extensions

You can extend Telegrator by creating custom filters, aspects, and state keepers.

## Custom Filters
```csharp
public class RateLimitFilter : FilterAnnotation<Message>
{
    private readonly Dictionary<long, DateTime> _lastExecution = new();
    private readonly TimeSpan _cooldown = TimeSpan.FromSeconds(5);

    public override bool CanPass(FilterExecutionContext<Message> context)
    {
        var userId = context.Input.From?.Id;
        if (userId == null) return true;

        if (_lastExecution.TryGetValue(userId.Value, out var lastExec))
        {
            if (DateTime.Now - lastExec < _cooldown)
                return false; // Rate limit exceeded
        }

        _lastExecution[userId.Value] = DateTime.Now;
        return true;
    }
}
```

## Custom State Machines
You can implement your own state transition logic by implementing the `IStateMachine<TState>` interface. This allows for complex state transitions beyond simple linear progression.

```csharp
public class BranchingStateMachine : IStateMachine<UserState>
{
    public async Task Advance(IStateStorage storage, string updateKey, CancellationToken ct)
    {
        // Custom logic to decide next state based on current state or complex rules
    }

    public async Task<UserState?> Current(IStateStorage storage, string updateKey, CancellationToken ct)
    {
        return await storage.GetAsync<UserState>(updateKey, ct);
    }
    
    // Retreat, Reset implementations...
}
```

## Custom State Storage
Implement `IStateStorage` to persist user states in your preferred database (SQL, MongoDB, etc.).

```csharp
public class MyDatabaseStorage : IStateStorage
{
    public async Task SetAsync<T>(string key, T state, CancellationToken ct)
    {
        // Save to DB
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
    {
        // Retrieve from DB
    }

    public async Task DeleteAsync(string key, CancellationToken ct)
    {
        // Delete from DB
    }
}
```

## Automatic Handler Discovery

Telegrator provides high-performance, automatic discovery and registration of handlers using a **Source Generator**.

### Generative Collection (Recommended)
The `CollectHandlers()` method is generated at compile-time by the Telegrator Source Generator.

**How it works:**
- Scans your source code during compilation.
- Generates a static method that registers all found handlers.
- **No reflection** is used at runtime.
- **Native AOT Compatible**: Since the registration is static, it survives code trimming and works in Native AOT binaries.

**Example:**
```csharp
var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
bot.Handlers.CollectHandlers(); // Statically compiled discovery
bot.StartReceiving();
```

### Reflection-based Collection (Obsolete)
The `CollectHandlersDomainWide()` and `CollectHandlersAssemblyWide()` methods scan loaded assemblies using reflection.

**Limitations:**
- Slower than generative collection.
- Incompatible with **Native AOT** and code trimming.
- Can be blocked by some security configurations.

**Benefits of Generative Collection:**
- Significant performance boost during startup.
- Full support for Trimming and Native AOT.
- Design-time validation via **DeveloperHelperAnalyzer**.
- Zero boilerplate while maintaining maximum performance.
