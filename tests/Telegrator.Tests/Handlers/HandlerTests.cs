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
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.Descriptors;
using Telegrator.Core.Filters;
using Telegrator.Core.Handlers;
using Telegrator.Core.States;
using Telegrator.Handlers;
using Xunit;

namespace Telegrator.Tests.Handlers;

public class HandlerTests
{
    [Fact]
    public async Task UpdateHandlerBase_ShouldExecuteAndMarkLifetimeAsEnded()
    {
        // Arrange
        Mock<IHandlerContainer<Message>> mockContainer = new Mock<IHandlerContainer<Message>>();
        TestUpdateHandler testHandler = new TestUpdateHandler();
        DescribedHandlerDescriptor describedDescriptor = new DescribedHandlerDescriptor(
            new ClassHandlerDescriptor(typeof(TestUpdateHandler), new object()),
            new Mock<IUpdateRouter>().Object,
            new Mock<IAwaitingProvider>().Object,
            new Mock<IStateStorage>().Object,
            new Mock<ITelegramBotClient>().Object,
            testHandler,
            new FilterExecutionContext<Update>(
                new Mock<IUpdateRouter>().Object,
                new TelegramBotInfo(null!),
                new Update { Message = new Message() },
                new Update { Message = new Message() },
                new Dictionary<string, object>(),
                new CompletedFiltersList()),
                "test");

        mockContainer.Setup(c => c.HandlingUpdate).Returns(new Update { Message = new Message() });

        // Override creation logic for the test container by using the DefaultContainerFactory Mock
        Mock<IHandlerContainerFactory> mockContainerFactory = new Mock<IHandlerContainerFactory>();
        mockContainerFactory.Setup(f => f.CreateContainer(It.IsAny<DescribedHandlerDescriptor>())).Returns(mockContainer.Object);

        Mock<IUpdateRouter> mockRouter = new Mock<IUpdateRouter>();
        mockRouter.Setup(r => r.DefaultContainerFactory).Returns(mockContainerFactory.Object);

        DescribedHandlerDescriptor properDescribed = new DescribedHandlerDescriptor(
            new ClassHandlerDescriptor(typeof(TestUpdateHandler), new object()),
            mockRouter.Object,
            new Mock<IAwaitingProvider>().Object,
            new Mock<IStateStorage>().Object,
            new Mock<ITelegramBotClient>().Object,
            testHandler,
            new FilterExecutionContext<Update>(
                mockRouter.Object,
                new TelegramBotInfo(null!),
                new Update { Message = new Message() },
                new Update { Message = new Message() },
                new Dictionary<string, object>(),
                new CompletedFiltersList()),
                "test");

        // Act
        await testHandler.Execute(properDescribed);

        // Assert
        testHandler.WasExecuted.Should().BeTrue();
        testHandler.LifetimeToken.IsEnded.Should().BeTrue();
    }

    [Fact]
    public void HandlerLifetimeToken_ShouldTrackLifetimeCorrectly()
    {
        // Arrange
        TestUpdateHandler handler = new TestUpdateHandler();

        // Act & Assert
        handler.LifetimeToken.IsEnded.Should().BeFalse();

        // Act
        handler.LifetimeToken.LifetimeEnded();

        // Assert
        handler.LifetimeToken.IsEnded.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateHandlerBase_ShouldHandleCancellation()
    {
        // Arrange
        Mock<IHandlerContainer<Message>> mockContainer = new Mock<IHandlerContainer<Message>>();
        TestUpdateHandler testHandler = new TestUpdateHandler();
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        // Act & Assert
        await testHandler.Invoking(h => h.Execute(mockContainer.Object, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }
}
