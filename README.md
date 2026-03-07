# Telegrator

![Telegrator Banner](https://github.com/Rikitav/Telegrator/blob/master/resources%2FTelegrator_banner.png)

> **A modern reactive framework for Telegram bots in C# with aspect-oriented design, mediator-based dispatching, and flexible architecture.**

---

## 🚀 About Telegrator

Telegrator is a next-generation framework for building Telegram bots in C#, inspired by AOP (Aspect-Oriented Programming) and the mediator pattern. It enables decentralized, easily extensible, and maintainable bot logic without traditional state machines or monolithic handlers.

---

## ✨ Key Features

- **Aspect-oriented approach**: Handlers and filters are "aspects" of the bot, easily composable and extendable.
- **Decentralized logic**: Each handler is an independent module—no more giant switch/case blocks.
- **Mediator-based dispatching**: All updates are routed through a powerful mediator-dispatcher.
- **Flexible filtering**: Filters for commands, text, sender, chat, regex, and much more.
- **Execution order and priorities**: Easily control handler priorities and execution order.
- **Thread safety and concurrency control**: Limit the number of concurrent handlers, await other updates inside a handler.
- **Extensibility via attributes and providers**: Easily add your own filters, handlers, and state keepers.
- **Minimal boilerplate—maximum declarativity!**

---

## 🧩 Architecture & Approach

- **Decentralization**: Bot logic is split into independent handlers (aspects), each responsible for its own part.
- **Mediator**: All Telegram updates go through a mediator, which decides which handlers should process them and in what order.
- **Filters**: Describe handler trigger conditions in a flexible, declarative way.
- **State**: Built-in mechanisms for user/chat state without manual state machines.

---

## 🛠️ Quick Start

### 1. Installation

```shell
# .NET CLI
dotnet add package Telegrator

# NuGet CLI
NuGet\Install-Package Telegrator
```

### 2. Minimal Bot Example

```csharp
using Telegrator.Handlers;
using Telegrator.Annotations;

[MessageHandler]
public class HelloHandler : MessageHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Reply("Hello, world!", cancellationToken: cancellation);
        return Result.Ok();
    }
}

// Registration and launch:
var bot = new TelegratorClient("<YOUR_BOT_TOKEN>");
bot.Handlers.AddHandler<HelloHandler>();
bot.StartReceiving();
```

### 3. Adding Filtering and Commands

```csharp
using Telegram.Bot.Types.Enums;
using Telegrator.Handlers;
using Telegrator.Annotations;

[CommandHandler, CommandAlias("start", "hello"), ChatType(ChatType.Private)]
public class StartCommandHandler : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        await Responce("Welcome!", cancellationToken: cancellation);
        return Result.Ok();
    }
}

// Registration:
bot.Handlers.AddHandler<StartCommandHandler>();
```

### 4. State Management Example

```csharp
using Telegrator.Handlers;
using Telegrator.Annotations;

[CommandHandler, CommandAlias("first"), NumericState(SpecialState.NoState)]
public class StateKeepFirst : CommandHandler
{
    public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
    {
        container.CreateNumericState();
        container.ForwardNumericState();
        await Reply("first state moved (1)", cancellationToken: cancellation);
        return Result.Ok();
    }
}

// Registration:
bot.Handlers.AddHandler<StateKeepFirst>();
```

---

## 🏆 Why Telegrator over state machines?

- **No tangled switch/case**—logic is split into independent handlers.
- **Flexible dispatching**—the mediator decides who and when processes an event.
- **Simple state management**—no need to implement state machines manually.
- **Easy scaling**—add new handlers without rewriting old ones.
- **High code readability and maintainability.**

---

## 📚 Documentation & Examples

- [Documentation](https://github.com/Rikitav/Telegrator/wiki/)
- [Usage examples (WIP)](https://github.com/Rikitav/Telegrator/tree/master/examples)

---

## 🤝 Contribution & Feedback

We welcome your questions, suggestions, and pull requests! Open issues or contact us directly.

---

## ⚡ License

GPLv3
