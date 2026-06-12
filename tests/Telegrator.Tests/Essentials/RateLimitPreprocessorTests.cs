using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Telegrator.Aspects;
using Telegrator.Core.Handlers;
using Telegrator.Core.States;
using Telegrator.States;
using Xunit;

namespace Telegrator.Tests.Essentials;

public class RateLimitPreprocessorTests
{
    private static IHandlerContainer CreateContainer(Update update, IStateStorage storage)
    {
        var mock = new Mock<IHandlerContainer>();
        mock.SetupGet(c => c.HandlingUpdate).Returns(update);
        mock.SetupGet(c => c.StateStorage).Returns(storage);
        return mock.Object;
    }

    [Fact]
    public async Task BeforeExecution_AllowsRequestsWithinLimit()
    {
        var storage = new DefaultStateStorage();
        var processor = new RateLimitPreprocessor { MaxRequests = 2, WindowSeconds = 60 };
        var update = new Update { Message = new Message { From = new User { Id = 1 } } };

        var result1 = await processor.BeforeExecution(CreateContainer(update, storage));
        var result2 = await processor.BeforeExecution(CreateContainer(update, storage));

        result1.Success.Should().BeTrue();
        result2.Success.Should().BeTrue();
    }

    [Fact]
    public async Task BeforeExecution_BlocksRequestsOverLimit()
    {
        var storage = new DefaultStateStorage();
        var processor = new RateLimitPreprocessor { MaxRequests = 1, WindowSeconds = 60 };
        var update = new Update { Message = new Message { From = new User { Id = 1 } } };

        var result1 = await processor.BeforeExecution(CreateContainer(update, storage));
        var result2 = await processor.BeforeExecution(CreateContainer(update, storage));

        result1.Success.Should().BeTrue();
        result2.Success.Should().BeFalse();
    }

    [Fact]
    public async Task BeforeExecution_TracksUsersIndependently()
    {
        var storage = new DefaultStateStorage();
        var processor = new RateLimitPreprocessor { MaxRequests = 1, WindowSeconds = 60 };
        var updateA = new Update { Message = new Message { From = new User { Id = 1 } } };
        var updateB = new Update { Message = new Message { From = new User { Id = 2 } } };

        var resultA1 = await processor.BeforeExecution(CreateContainer(updateA, storage));
        var resultB1 = await processor.BeforeExecution(CreateContainer(updateB, storage));

        resultA1.Success.Should().BeTrue();
        resultB1.Success.Should().BeTrue();
    }
}
