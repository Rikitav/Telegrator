---
title: "State Management"
description: "Maintain state easily for wizards and complex flows without external storage."
---

# State Management

Telegrator provides built-in state management for multi-step conversations (wizards, forms, quizzes) with or without a database.

> [!NOTE]
> Each type of `StateKeeper`'s keys and states are shared between **EVERY** handler in project.

**How to Use:**
1. Define your state (e.g. `public enum MyEnum { Step1, Step2 }`)
2. Use a state filter attribute on your handler:
   - `[State<SenderIdResolver, MyEnum>(MyEnum.Step1)]`
3. Change state inside the handler using `StateStorage` and `StateMachine`:
   - `await StateStorage.GetStateMachine<MyEnum>(HandlingUpdate).BySenderId().Advance();`
   - `await StateStorage.GetStateMachine<MyEnum>(HandlingUpdate).BySenderId().Reset();`
   - `TValue current = await StateStorage.GetStateMachine<MyEnum>(HandlingUpdate).BySenderId().Current();`

## Example

```csharp
public enum QuizState
{
    Start, Q1, Q2
}

[CommandHandler]
[CommandAllias("quiz")]
[State<QuizState>(QuizState.Start)]
public class StartQuizHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        StateStorage.GetStateMachine<QuizState>().BySenderId().Advance();
        await Reply("Quiz started! Question 1: What is the capital of France?");
        return Ok;
    }
}

[MessageHandler]
[State<QuizState>(QuizState.Q1)]
public class Q1Handler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        if (Input.Text.Trim().Equals("Paris", StringComparison.InvariantCultureIgnoreCase))
            await Reply("Correct!");
        else
            await Reply("Incorrect. The answer is Paris.");

        StateStorage.GetStateMachine<QuizState>().BySenderId().Advance();
        await Reply("Question 2: What is 2 + 2?");
        return Ok;
    }
}
```

> **How is it working?**
> 1. **Enum State Definition**: `QuizState` enum defines the conversation flow with `Start = SpecialState.NoState` indicating no initial state.
> 2. **State Filter**: `[State<QuizState>(QuizState.Start)]` ensures the handler only runs when the user is in the "Start" state.
> 3. **State Transition**: `StateStorage.GetStateMachine<QuizState>().BySenderId().Advance()` moves the user to the next state (Q1).
> 4. **Next Handler**: The `Q1Handler` will only run when the user is in state `QuizState.Q1`.
> 5. **State Management**: Each handler manages its own state transition, creating a clear conversation flow.
