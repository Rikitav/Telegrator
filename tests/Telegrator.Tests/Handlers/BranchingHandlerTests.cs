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
using Telegrator.Core.Descriptors;
using Telegrator.Handlers;
using Telegrator.Providers;
using Xunit;

#pragma warning disable CA1822, IDE0060
namespace Telegrator.Tests.Handlers;

public class BranchingHandlerTests
{
    [Fact]
    public void BranchingUpdateHandler_DescribeHandlers_ShouldReturnBranchDescriptors()
    {
        // Arrange
        TestBranchingHandler handler = new TestBranchingHandler();

        // Act
        HandlerDescriptor[] descriptors = handler.DescribeHandlers().ToArray();

        // Assert
        descriptors.Should().HaveCount(3);
        descriptors.Should().AllSatisfy(d => d.HandlerType.Should().Be<TestBranchingHandler>());
        descriptors.Should().AllSatisfy(d => d.UpdateType.Should().Be(UpdateType.Message));
    }

    [Fact]
    public void BranchingUpdateHandler_BranchDescriptors_ShouldHaveCorrectFilters()
    {
        // Arrange
        TestBranchingHandler handler = new TestBranchingHandler();

        // Act
        HandlerDescriptor[] descriptors = handler.DescribeHandlers().ToArray();

        // Assert
        HandlerDescriptor helloBranch = descriptors.Should().ContainSingle(d => d.DisplayString == "TestBranchingHandler+HandleHello").Subject;
        helloBranch.Filters?.UpdateFilters.Should().NotBeNullOrEmpty();

        HandlerDescriptor helpBranch = descriptors.Should().ContainSingle(d => d.DisplayString == "TestBranchingHandler+HandleHelp").Subject;
        helpBranch.Filters?.UpdateFilters.Should().NotBeNullOrEmpty();

        HandlerDescriptor adminBranch = descriptors.Should().ContainSingle(d => d.DisplayString == "TestBranchingHandler+HandleAdmin").Subject;
        adminBranch.Filters?.UpdateFilters.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void BranchingUpdateHandler_ClassFilters_ShouldBeAppliedToAllBranches()
    {
        // Arrange
        TestBranchingHandlerWithClassFilter handler = new TestBranchingHandlerWithClassFilter();

        // Act
        HandlerDescriptor[] descriptors = handler.DescribeHandlers().ToArray();

        // Assert
        descriptors.Should().HaveCount(2);
        descriptors.Should().AllSatisfy(d => d.Filters?.UpdateFilters.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void BranchingUpdateHandler_VoidBranch_ShouldBeDescribed()
    {
        // Arrange
        TestBranchingHandlerWithVoidBranch handler = new TestBranchingHandlerWithVoidBranch();

        // Act
        HandlerDescriptor[] descriptors = handler.DescribeHandlers().ToArray();

        // Assert
        descriptors.Should().HaveCount(2);
        descriptors.Should().ContainSingle(d => d.DisplayString == "TestBranchingHandlerWithVoidBranch+HandleVoid");
    }

    [Fact]
    public void AddHandler_BranchingHandler_ShouldRegisterViaCustomDescriptorsProvider()
    {
        // Arrange
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());

        // Act
        collection.AddHandler<TestBranchingHandler>();

        // Assert
        collection.Keys.Should().Contain(UpdateType.Message);
        collection[UpdateType.Message].Should().HaveCount(3);
    }

    [Fact]
    public void AddHandler_BranchingHandler_ShouldCreateDistinctDescriptors()
    {
        // Arrange
        HandlersCollection collection = new HandlersCollection(new TelegratorOptions());

        // Act
        collection.AddHandler<TestBranchingHandler>();

        // Assert
        HandlerDescriptorList list = collection[UpdateType.Message];
        list.Select(d => d.DisplayString).Should().Contain("TestBranchingHandler+HandleHello");
        list.Select(d => d.DisplayString).Should().Contain("TestBranchingHandler+HandleHelp");
        list.Select(d => d.DisplayString).Should().Contain("TestBranchingHandler+HandleAdmin");
    }

    [Fact]
    public void BranchingUpdateHandler_NoBranches_ShouldThrow()
    {
        // Arrange
        EmptyBranchingHandler handler = new EmptyBranchingHandler();

        // Act & Assert
        handler.Invoking(h => h.DescribeHandlers().ToArray())
            .Should().Throw<InvalidOperationException>();
    }

    // Test branching handlers
    [MessageHandler]
    [ChatType(ChatType.Private)]
    public class TestBranchingHandler : BranchingMessageHandler
    {
        [TextContains("hello")]
        public async Task<Result> HandleHello(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("Hello there!", cancellationToken: cancellation);
            return Ok;
        }

        [TextContains("help")]
        public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("How can I help you?", cancellationToken: cancellation);
            return Ok;
        }

        [FromUser("Admin")]
        public async Task<Result> HandleAdmin(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Reply("Admin command received!", cancellationToken: cancellation);
            return Ok;
        }
    }

    [MessageHandler]
    [ChatType(ChatType.Private)]
    public class TestBranchingHandlerWithClassFilter : BranchingMessageHandler
    {
        [TextContains("hello")]
        public async Task<Result> HandleHello(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            return Ok;
        }

        [TextContains("help")]
        public async Task<Result> HandleHelp(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            return Ok;
        }
    }

    [MessageHandler]
    public class TestBranchingHandlerWithVoidBranch : BranchingMessageHandler
    {
        [TextContains("void")]
        public void HandleVoid()
        {
        }

        [TextContains("task")]
        public async Task<Result> HandleTask(IHandlerContainer<Message> container, CancellationToken cancellation)
        {
            await Task.CompletedTask;
            return Ok;
        }
    }

    [MessageHandler]
    public class EmptyBranchingHandler : BranchingMessageHandler
    {
    }
}

#pragma warning restore
