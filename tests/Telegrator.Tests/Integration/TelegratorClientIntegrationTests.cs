using FluentAssertions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Handlers;
using Telegrator.Testing;
using Xunit;

namespace Telegrator.Tests.Integration;

public class TelegratorClientIntegrationTests
{
    [Fact]
    public async Task EmitMessageAsync_ShouldRouteToMessageHandler()
    {
        EchoHandler.Executed = false;
        var client = new TestTelegratorClient();
        client.Handlers.AddHandler<EchoHandler>();
        client.StartTestReceiving();

        var message = new Message
        {
            Id = 1, // Required
            Text = "hello",
            Chat = new Chat { Id = 42, Type = ChatType.Private },
            From = new User { Id = 42, FirstName = "Alice" }
        };

        await client.EmitMessageAsync(message);

        EchoHandler.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task EmitMessageAsync_ShouldNotRoute_WhenFilterDoesNotMatch()
    {
        EchoHandler.Executed = false;
        var client = new TestTelegratorClient();
        client.Handlers.AddHandler<EchoHandler>();
        client.StartTestReceiving();

        var message = new Message
        {
            Id = 1, // Required
            Text = "goodbye",
            Chat = new Chat { Id = 42, Type = ChatType.Private },
            From = new User { Id = 42, FirstName = "Alice" }
        };

        await client.EmitMessageAsync(message);

        EchoHandler.Executed.Should().BeFalse();
    }

    [Fact]
    public async Task EmitUpdateAsync_ShouldRouteToCallbackQueryHandler()
    {
        ButtonHandler.Executed = false;
        var client = new TestTelegratorClient();
        client.Handlers.AddHandler<ButtonHandler>();
        client.StartTestReceiving();

        var update = new Update
        {
            Id = 1,
            CallbackQuery = new CallbackQuery
            {
                Id = "cq1",
                Data = "action_click",
                From = new User { Id = 99, FirstName = "Bob" },
                Message = new Message { Chat = new Chat { Id = 99 } }
            }
        };

        await client.EmitUpdateAsync(update);

        ButtonHandler.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task EmitMessageAsync_ShouldSupportAwaitingMechanism()
    {
        AskNameHandler.Asked = false;
        AskNameHandler.Answered = false;
        var client = new TestTelegratorClient();
        client.Handlers.AddHandler<AskNameHandler>();
        client.StartTestReceiving();

        var message1 = new Message
        {
            Id = 1, // Required
            Text = "/ask",
            Entities = new[] { new MessageEntity { Type = MessageEntityType.BotCommand, Offset = 0, Length = 4 } }, // Required for command recognition
            Chat = new Chat { Id = 7, Type = ChatType.Private },
            From = new User { Id = 7, FirstName = "Charlie" }
        };

        var message2 = new Message
        {
            Id = 2, // Required
            Text = "Charlie",
            Chat = new Chat { Id = 7, Type = ChatType.Private },
            From = new User { Id = 7, FirstName = "Charlie" }
        };

        // Step 1: trigger in background because handler blocks awaiting next message
        var handlerTask = Task.Run(async () => await client.EmitMessageAsync(message1));
        await Task.Delay(200); // let the handler register the awaiter

        AskNameHandler.Asked.Should().BeTrue();
        AskNameHandler.Answered.Should().BeFalse();

        // Step 2: simulate user reply
        await client.EmitMessageAsync(message2);

        // Step 3: wait for the original handler to complete
        await handlerTask.WaitAsync(TimeSpan.FromSeconds(5));

        AskNameHandler.Answered.Should().BeTrue();
    }

    [Fact]
    public void StartTestReceiving_ShouldInitializeRouter()
    {
        var client = new TestTelegratorClient();
        Action action = () => { client.UpdateRouter.GetType(); };
        action.Should().Throw<InvalidOperationException>();

        client.StartTestReceiving();
        client.UpdateRouter.Should().NotBeNull();
    }

    [MessageHandler]
    [TextContains("hello")]
    public class EchoHandler : MessageHandler
    {
        public static bool Executed { get; set; }

        public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            Executed = true;
            await Reply($"Echo: {container.ActualUpdate.Text}");
            return Ok;
        }
    }

    [CallbackQueryHandler]
    [CallbackDataStartsWith("action_")]
    public class ButtonHandler : CallbackQueryHandler
    {
        public static bool Executed { get; set; }

        public override async Task<Result> Execute(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
        {
            Executed = true;
            await Container.AnswerCallbackQuery("Action processed!");
            return Ok;
        }
    }

    [CommandHandler]
    [CommandAllias("ask")]
    public class AskNameHandler : CommandHandler
    {
        public static bool Asked { get; set; }
        public static bool Answered { get; set; }

        public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            Asked = true;
            await Reply("What is your name?");

            var nextMessage = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
            Answered = true;
            await Reply($"Hello, {nextMessage.Text}!");

            return Ok;
        }
    }
}
