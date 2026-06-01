---
title: "Advanced Filters"
description: "Write your own attribute-based filters."
---

# Advanced Filters

You can create custom filters by inheriting from `FilterAnnotation<T>`:

```csharp
using Telegram.Bot.Types;
using Telegrator.Attributes;
using Telegrator.Handlers;

public class AdminOnlyAttribute : FilterAnnotation<Message>
{ 
    private readonly List<long> _adminIds = [];

    public void AddAdmin(long id) => _adminIds.Add(id);
    public void RemoveAdmin(long id) => _adminIds.Remove(id);
    
    public override bool CanPass(FilterExecutionContext<Message> context)
        => _adminIds.Contains(context.Input.From?.Id);
}

[MessageHandler]
[AdminOnly]
public class AdminHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, admin!");
        return Ok;
    }
}

// Usage
AdminOnlyAttribute.AddAdmin(123456789);
bot.StartReceiving();
```

> **How is it working?**
> 1. **Custom Filter**: `AdminOnlyAttribute` inherits from `FilterAnnotation<Message>` to create a reusable filter attribute.
> 2. **Filter Logic**: `CanPass()` method checks if the message sender's ID matches the admin ID.
> 3. **Usage**: The filter is applied as an attribute `[AdminOnly]` to restrict access to users that are not registered as admins.
