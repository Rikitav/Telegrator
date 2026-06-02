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

| Implementation | Persistence | Use Case |
|---------------|-------------|----------|
| `DefaultStateStorage` | In-memory `ConcurrentDictionary` | Single-instance bots, testing |
| `RedisStateStorage` | Redis via `IConnectionMultiplexer` | Distributed bots, multi-instance deployments |

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

| Resolver | Key Format | Use Case |
|----------|-----------|----------|
| `SenderIdResolver` | `state:{userId}` | Per-user state |
| `ChatIdResolver` | `state:{chatId}` | Per-chat state |

### Methods

| Method | Description |
|--------|-------------|
| `Advance()` | Moves to the next state in the enum sequence |
| `Back()` | Moves to the previous state |
| `Current()` | Returns the current state value |
| `Reset()` | Clears the state |

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
