---
title: "State Storage & StateMachine"
description: "Persist user and chat state for multi-step conversations."
---

# State Storage

Telegrator provides a lightweight state management system for building wizards, forms, and multi-step interactions. State is keyed by a resolver (e.g., sender ID or chat ID) and stored via `IStateStorage`.

## Interface

```csharp
public interface IStateStorage
{
    Task SetAsync<T>(string key, T state, CancellationToken cancellationToken);
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
}
```

## Built-in Implementations

<table>
<thead>
<tr><th>Implementation</th><th>Persistence</th><th>Use Case</th></tr>
</thead>
<tbody>
<tr><td>`DefaultStateStorage`</td><td>In-memory `ConcurrentDictionary`</td><td>Single-instance bots, testing</td></tr>
<tr><td>`RedisStateStorage`</td><td>Redis via `IConnectionMultiplexer`</td><td>Distributed bots, multi-instance deployments</td></tr>
</tbody>
</table>

## State Machine

A `StateMachine` wraps `IStateStorage` with typed state transitions:

```csharp
StateMachine<EnumStateMachine<QuizState>, QuizState> machine =
    StateStorage.GetStateMachine<QuizState>(HandlingUpdate);

await machine.BySenderId().Advance();
QuizState current = await machine.BySenderId().Current();
await machine.BySenderId().Reset();
```

### Key Resolvers

<table>
<thead>
<tr><th>Resolver</th><th>Key Format</th><th>Use Case</th></tr>
</thead>
<tbody>
<tr><td>`SenderIdResolver`</td><td>`state:{userId}`</td><td>Per-user state</td></tr>
<tr><td>`ChatIdResolver`</td><td>`state:{chatId}`</td><td>Per-chat state</td></tr>
</tbody>
</table>

### Methods

<table>
<thead>
<tr><th>Method</th><th>Description</th></tr>
</thead>
<tbody>
<tr><td>`Advance()`</td><td>Moves to the next state in the enum sequence</td></tr>
<tr><td>`Back()`</td><td>Moves to the previous state</td></tr>
<tr><td>`Current()`</td><td>Returns the current state value</td></tr>
<tr><td>`Reset()`</td><td>Clears the state</td></tr>
</tbody>
</table>

## Declarative State Filtering

Use the `[State]` attribute to restrict a handler to a specific state:

```csharp
public enum QuizState { Start, Q1, Q2, Finish }

[CommandHandler]
[CommandAllias("quiz")]
[State<QuizState>(QuizState.Start)]
public class StartQuizHandler : CommandHandler { }

[MessageHandler]
[State<QuizState>(QuizState.Q1)]
public class Q1Handler : MessageHandler { }
```

## Example: Wizard Flow

```csharp
public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
{
    await Reply("What is your name?");
    await StateStorage.GetStateMachine<WizardState>(HandlingUpdate).BySenderId().Advance();
    return Ok;
}
```

> [!TIP]
> Always `await` state machine methods. They perform asynchronous storage operations.
