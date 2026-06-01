---
title: "State Management Wizard"
description: "Build a state machine that collects user inputs consecutively."
---

# State Management Wizard

You can manage sequential tasks using numerical, string-based, or enum states easily.

```csharp
[CommandHandler]
[CommandAllias("wizard")]
[State<SenderIdResolver, int>(0)] // 0 is default/no state
public class StartWizardHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Move to state 1
        await StateStorage.GetStateMachine<int>(HandlingUpdate).BySenderId().Advance();
        
        await Reply("What is your name?");
        return Ok;
    }
}

[MessageHandler]
[State<SenderIdResolver, int>(1)]
public class NameHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        // Move to state 2
        await StateStorage.GetStateMachine<int>(HandlingUpdate).BySenderId().Advance();
        
        await Reply($"Nice to meet you, {Input.Text}! How old are you?");
        return Ok;
    }
}

[MessageHandler]
[State<SenderIdResolver, int>(2)]
public class AgeHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (int.TryParse(Input.Text, out int age))
        {
            await Reply($"Thank you! You are {age} years old. Wizard completed!");
            
            // Reset/Clear state
            await StateStorage.GetStateMachine<int>(HandlingUpdate).BySenderId().Reset();
        }
        else
        {
            await Reply("Please enter a valid age (number).");
        }
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Initial State**: `[State<..., int>(0)]` catches users with no previous state (default int value as 0).
> 2. **State Progression**: `GetStateMachine<int>(...).BySenderId().Advance()` increments the numeric state.
> 3. **State Filtering**: Each handler is locked to a specific state value using the `[State]` attribute.
> 4. **State Reset**: `Reset()` clears the state (setting it back to 0) when the wizard completes.
