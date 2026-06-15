/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
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
using Telegrator.Annotations;
using Telegrator.Handlers;
using Telegrator.Testing;
using Xunit;

#pragma warning disable CA1822, IDE0060
namespace Telegrator.Tests.Integration;

public class BranchingHandlerIntegrationTests
{
    [Fact]
    public async Task EmitMessageAsync_BranchingMessageHandler_ShouldRouteToMatchingBranch()
    {
        ResetFlags();
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<BranchingEchoHandler>();
        client.StartTestReceiving();

        Message message = new Message
        {
            Text = "hello world",
            Chat = new Chat { Id = 1, Type = ChatType.Private },
            From = new User { Id = 1, FirstName = "User" }
        };

        await client.EmitMessageAsync(message);

        BranchingEchoHandler.HelloExecuted.Should().BeTrue();
        BranchingEchoHandler.HelpExecuted.Should().BeFalse();
    }

    [Fact]
    public async Task EmitMessageAsync_BranchingMessageHandler_ShouldNotRoute_WhenNoBranchMatches()
    {
        ResetFlags();
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<BranchingEchoHandler>();
        client.StartTestReceiving();

        Message message = new Message
        {
            Text = "unknown phrase",
            Chat = new Chat { Id = 1, Type = ChatType.Private },
            From = new User { Id = 1, FirstName = "User" }
        };

        await client.EmitMessageAsync(message);

        BranchingEchoHandler.HelloExecuted.Should().BeFalse();
        BranchingEchoHandler.HelpExecuted.Should().BeFalse();
    }

    [Fact]
    public async Task EmitMessageAsync_BranchingMessageHandler_ShouldRespectClassFilter()
    {
        ResetFlags();
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<GroupOnlyBranchingHandler>();
        client.StartTestReceiving();

        Message privateMessage = new Message
        {
            Text = "hello",
            Chat = new Chat { Id = 1, Type = ChatType.Private },
            From = new User { Id = 1, FirstName = "User" }
        };

        await client.EmitMessageAsync(privateMessage);
        GroupOnlyBranchingHandler.Executed.Should().BeFalse();

        Message groupMessage = new Message
        {
            Text = "hello",
            Chat = new Chat { Id = 2, Type = ChatType.Group },
            From = new User { Id = 1, FirstName = "User" }
        };

        await client.EmitMessageAsync(groupMessage);
        GroupOnlyBranchingHandler.Executed.Should().BeTrue();
    }

    [Fact]
    public async Task EmitMessageAsync_BranchingCommandHandler_ShouldRouteToMatchingCommand()
    {
        ResetFlags();
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<BranchingCommandHandlerImpl>();
        client.StartTestReceiving();

        Message startCommand = new Message
        {
            Text = "/start",
            Entities = new[] { new MessageEntity { Type = MessageEntityType.BotCommand, Offset = 0, Length = 6 } },
            Chat = new Chat { Id = 1, Type = ChatType.Private },
            From = new User { Id = 1, FirstName = "User" }
        };

        await client.EmitMessageAsync(startCommand);

        BranchingCommandHandlerImpl.StartExecuted.Should().BeTrue();
        BranchingCommandHandlerImpl.HelpExecuted.Should().BeFalse();
    }

    [Fact]
    public async Task EmitUpdateAsync_BranchingCallbackQueryHandler_ShouldRouteToMatchingBranch()
    {
        ResetFlags();
        TestTelegratorClient client = new TestTelegratorClient();
        client.Handlers.AddHandler<BranchingCallbackHandler>();
        client.StartTestReceiving();

        Update update = new Update
        {
            Id = 1,
            CallbackQuery = new CallbackQuery
            {
                Id = "cq1",
                Data = "confirm",
                From = new User { Id = 1, FirstName = "User" },
                Message = new Message { Chat = new Chat { Id = 1 } }
            }
        };

        await client.EmitUpdateAsync(update);

        BranchingCallbackHandler.ConfirmExecuted.Should().BeTrue();
        BranchingCallbackHandler.CancelExecuted.Should().BeFalse();
    }

    private static void ResetFlags()
    {
        BranchingEchoHandler.Reset();
        GroupOnlyBranchingHandler.Reset();
        BranchingCommandHandlerImpl.Reset();
        BranchingCallbackHandler.Reset();
    }

    [MessageHandler]
    public class BranchingEchoHandler : BranchingMessageHandler
    {
        public static bool HelloExecuted { get; set; }
        public static bool HelpExecuted { get; set; }

        public static void Reset()
        {
            HelloExecuted = false;
            HelpExecuted = false;
        }

        [TextContains("hello")]
        public async Task<Result> HandleHello(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            HelloExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }

        [TextContains("help")]
        public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            HelpExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }
    }

    [MessageHandler]
    [ChatType(ChatType.Group)]
    public class GroupOnlyBranchingHandler : BranchingMessageHandler
    {
        public static bool Executed { get; set; }

        public static void Reset() => Executed = false;

        [TextContains("hello")]
        public async Task<Result> HandleHello(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            Executed = true;
            await Task.CompletedTask;
            return Ok;
        }
    }

    [CommandHandler]
    public class BranchingCommandHandlerImpl : BranchingCommandHandler
    {
        public static bool StartExecuted { get; set; }
        public static bool HelpExecuted { get; set; }

        public static void Reset()
        {
            StartExecuted = false;
            HelpExecuted = false;
        }

        [CommandAlias("start")]
        public async Task<Result> HandleStart(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            StartExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }

        [CommandAlias("help")]
        public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            HelpExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }
    }

    [CallbackQueryHandler]
    public class BranchingCallbackHandler : BranchingCallbackQueryHandler
    {
        public static bool ConfirmExecuted { get; set; }
        public static bool CancelExecuted { get; set; }

        public static void Reset()
        {
            ConfirmExecuted = false;
            CancelExecuted = false;
        }

        [CallbackData("confirm")]
        public async Task<Result> HandleConfirm(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
        {
            ConfirmExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }

        [CallbackData("cancel")]
        public async Task<Result> HandleCancel(IHandlerContainer<CallbackQuery> container, CancellationToken cancellation)
        {
            CancelExecuted = true;
            await Task.CompletedTask;
            return Ok;
        }
    }
}
