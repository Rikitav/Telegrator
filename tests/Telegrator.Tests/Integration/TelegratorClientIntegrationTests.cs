/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using FluentAssertions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegrator.Annotations;
using Telegrator.Handlers;
using Telegrator.Markups;
using Telegrator.Testing;
using Xunit;

namespace Telegrator.Tests.Integration;

public partial class TelegratorClientIntegrationTests
{
    [Fact]
    public async Task EmitMessageAsync_ShouldRouteToMessageHandler()
    {
        EchoHandler.Executed = false;
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<EchoHandler>();
        client.StartTestReceiving();

        Message message = new Message
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
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<EchoHandler>();
        client.StartTestReceiving();

        Message message = new Message
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
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<ButtonHandler>();
        client.StartTestReceiving();

        Update update = new Update
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
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<AskNameHandler>();
        client.StartTestReceiving();

        Message message1 = new Message
        {
            Id = 1, // Required
            Text = "/ask",
            Entities = new[] { new MessageEntity { Type = MessageEntityType.BotCommand, Offset = 0, Length = 4 } }, // Required for command recognition
            Chat = new Chat { Id = 7, Type = ChatType.Private },
            From = new User { Id = 7, FirstName = "Charlie" }
        };

        Message message2 = new Message
        {
            Id = 2, // Required
            Text = "Charlie",
            Chat = new Chat { Id = 7, Type = ChatType.Private },
            From = new User { Id = 7, FirstName = "Charlie" }
        };

        // Step 1: trigger in background because handler blocks awaiting next message
        Task handlerTask = Task.Run(async () => await client.EmitMessageAsync(message1));
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
        TestTelegratorClient client = new TestTelegratorClient();
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
            await Reply($"Echo: {container.ActualUpdate.Text}", cancellationToken: cancellation);
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
            await Answer("Action processed!", cancellationToken: cancellation);
            return Ok;
        }
    }

    [CommandHandler]
    [CommandAllias("ask")]
    [MightAwait(UpdateType.Message)]
    public partial class AskNameHandler : CommandHandler
    {
        public static bool Asked { get; set; }
        public static bool Answered { get; set; }

        public override async Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            Asked = true;
            await Reply("What is your name?", cancellationToken: cancellation);

            Message? nextMessage = await AwaitingProvider.AwaitMessage(HandlingUpdate).BySenderId(cancellation);
            if (nextMessage == null)
                return Ok;

            Answered = true;
            await Reply($"Hello, {nextMessage.Text}!", replyMarkup: Keyboard(10), cancellationToken: cancellation);
            return Ok;
        }

        [CallbackButton("Отмена", "cancell"), CallbackButton("Применить", "apply:{val}")]
        private static partial InlineKeyboardMarkup Keyboard(int val);
    }
}
