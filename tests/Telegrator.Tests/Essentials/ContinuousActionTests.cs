using FluentAssertions;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Handlers;
using Telegrator.Extensions;
using Xunit;

namespace Telegrator.Tests.Essentials;

public class ContinuousActionTests
{
    [Fact]
    public void StartTypingAction_ShouldCreateContinuousAction()
    {
        var update = new Update
        {
            Message = new Message { Chat = new Chat { Id = 42 } }
        };

        var container = new Mock<IHandlerContainer>();
        container.SetupGet(c => c.HandlingUpdate).Returns(update);
        container.SetupGet(c => c.Client).Returns(Mock.Of<ITelegramBotClient>());

        using var action = container.Object.StartTypingAction();

        action.Should().NotBeNull();
    }

    [Fact]
    public void StartContinuousAction_ShouldThrowWhenChatMissing()
    {
        var container = new Mock<IHandlerContainer>();
        container.SetupGet(c => c.HandlingUpdate).Returns(new Update());
        container.SetupGet(c => c.Client).Returns(Mock.Of<ITelegramBotClient>());

        Action act = () => container.Object.StartContinuousAction(ChatAction.Typing);

        act.Should().Throw<InvalidOperationException>();
    }
}
