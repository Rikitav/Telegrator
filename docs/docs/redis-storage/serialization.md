---
title: "Serialization & Persistence"
description: "How Telegrator stores your complex state objects in Redis."
---

# Serialization & Persistence

When you store a state in Redis, Telegrator converts your C# objects into JSON. This process is handled automatically, but there are a few things to keep in mind.

## Supported Types
- **Primitives**: `int`, `string`, `bool`, `enum`.
- **POCOs**: Simple classes with public properties.
- **Collections**: `List<T>`, `Dictionary<string, T>`.

## Example: Complex State
```csharp
public class UserProfile
{
    public string Name { get; set; }
    public int Age { get; set; }
    public List<string> Interests { get; set; } = new();
}

// Storing in handler
var state = await StateStorage.GetStateMachine<UserProfile>(HandlingUpdate).BySenderId().Current();
state.Interests.Add("C#");
await StateStorage.GetStateMachine<UserProfile>(HandlingUpdate).BySenderId().Set(state);
```

## Custom Serialization
By default, Telegrator uses `System.Text.Json`. If you need custom serialization (e.g., handling private setters or using `Newtonsoft.Json`), you can implement a custom `IStateSerializer`.

## Key Resolution
Telegrator generates Redis keys using a combination of the **State Type Name** and the **Resolved ID**.
- Default key: `Telegrator:State:UserProfile:12345678`

## Best Practices
1. **Small States**: Keep your state objects small. Redis works best with small values.
2. **Nullable Properties**: Use them for optional data to keep JSON compact.
3. **Enums**: Store them as strings or integers. Telegrator handles both, but strings are more readable in Redis Desktop Manager.

## Limitations
- **Cyclic References**: JSON serialization will fail if your objects point to each other in a circle.
- **Deep Nesting**: Avoid extremely deep object hierarchies.
