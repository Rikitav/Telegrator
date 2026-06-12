using FluentAssertions;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core.Handlers;
using Telegrator.Essentials.Aspects;
using Telegrator.Essentials.Extensions;
using Xunit;

namespace Telegrator.Tests.Essentials;

public class AutoDeletePostProcessorTests
{
    [Fact]
    public async Task AfterExecution_WhenNoMessageScheduled_ReturnsOk()
    {
        var container = new Mock<IHandlerContainer>();
        container.SetupGet(c => c.ExtraData).Returns(new Dictionary<string, object>());

        var processor = new AutoDeletePostProcessor();
        var result = await processor.AfterExecution(container.Object);

        result.Success.Should().BeTrue();
    }

    [Fact]
    public void ScheduleAutoDelete_ShouldStoreMessageAndDelay()
    {
        var extraData = new Dictionary<string, object>();
        var container = new Mock<IHandlerContainer>();
        container.SetupGet(c => c.ExtraData).Returns(extraData);

        var message = new Message { Chat = new Chat { Id = 123 } };
        var delay = TimeSpan.FromSeconds(2);

        container.Object.ScheduleAutoDelete(message, delay);

        extraData[AutoDeletePostProcessor.MessageKey].Should().Be(message);
        extraData[AutoDeletePostProcessor.DelayKey].Should().Be(delay);
    }
}
