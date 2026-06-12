using FluentAssertions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegrator.Annotations;
using Telegrator.Core.Filters;
using Xunit;

#pragma warning disable CS8625

namespace Telegrator.Tests.Essentials;

public class FilterAnnotationTests
{
    private static FilterExecutionContext<Message> MessageContext(string? text, ChatType chatType = ChatType.Private)
    {
        var update = new Update
        {
            Message = new Message
            {
                Id = 1,
                Text = text,
                Chat = new Chat { Id = 1, Type = chatType },
                From = new User { Id = 1 }
            }
        };

        return new FilterExecutionContext<Message>(null, new TelegramBotInfo(new User()), update, update.Message!, new Dictionary<string, object>(), new CompletedFiltersList());
    }

    private static FilterExecutionContext<CallbackQuery> CallbackContext(string data, int queryId = 1, long userId = 1)
    {
        var update = new Update
        {
            CallbackQuery = new CallbackQuery
            {
                Id = queryId.ToString(),
                Data = data,
                From = new User { Id = userId }
            }
        };

        return new FilterExecutionContext<CallbackQuery>(null, new TelegramBotInfo(new User()), update, update.CallbackQuery!, new Dictionary<string, object>(), new CompletedFiltersList());
    }

    #region PrefixedTriggerWordAttribute

    [Theory]
    [InlineData("/start", true)]
    [InlineData("/help", true)]
    [InlineData("!start", true)]
    [InlineData("plain text", false)]
    public void PrefixedTriggerWord_MatchesPrefixesAndWords(string text, bool expected)
    {
        var filter = new PrefixedTriggerWordAttribute { Prefixes = ["/", "!"], Words = ["start", "help"] };
        bool result = filter.CanPass(MessageContext(text));
        result.Should().Be(expected);
    }

    [Fact]
    public void PrefixedTriggerWord_MatchWholeWord_PreventsPartialMatch()
    {
        var filter = new PrefixedTriggerWordAttribute { Prefixes = ["/"], Words = ["start"], MatchWholeWord = true };
        filter.CanPass(MessageContext("/start")).Should().BeTrue();
        filter.CanPass(MessageContext("/startnow")).Should().BeFalse();
    }

    [Fact]
    public void PrefixedTriggerWord_AllowsAnyTextWhenWordsEmpty()
    {
        var filter = new PrefixedTriggerWordAttribute { Prefixes = ["/"] };
        filter.CanPass(MessageContext("/anything")).Should().BeTrue();
        filter.CanPass(MessageContext("no prefix")).Should().BeFalse();
    }

    #endregion

    #region WelcomeAttribute

    [Theory]
    [InlineData("/start", ChatType.Private, true)]
    [InlineData("/start with payload", ChatType.Private, true)]
    [InlineData("/start", ChatType.Group, false)]
    [InlineData("/help", ChatType.Private, false)]
    [InlineData(null, ChatType.Private, false)]
    public void Welcome_MatchesStartInPrivateChat(string? text, ChatType chatType, bool expected)
    {
        var filter = new WelcomeAttribute();
        bool result = filter.CanPass(MessageContext(text, chatType));
        result.Should().Be(expected);
    }

    #endregion

    #region PreventDoubleSubmit

    [Fact]
    public void PreventDoubleSubmit_AllowsFirstQuery()
    {
        var filter = new PreventDoubleSubmit { TimeoutSeconds = 5 };
        filter.CanPass(CallbackContext("vote", 1)).Should().BeTrue();
    }

    [Fact]
    public void PreventDoubleSubmit_BlocksDuplicateWithinTimeout()
    {
        var filter = new PreventDoubleSubmit { TimeoutSeconds = 5 };
        filter.CanPass(CallbackContext("vote", 1)).Should().BeTrue();
        filter.CanPass(CallbackContext("vote", 1)).Should().BeFalse();
    }

    [Fact]
    public void PreventDoubleSubmit_AllowsDifferentQueries()
    {
        var filter = new PreventDoubleSubmit { TimeoutSeconds = 5 };
        filter.CanPass(CallbackContext("vote", 1)).Should().BeTrue();
        filter.CanPass(CallbackContext("vote", 2)).Should().BeTrue();
    }

    #endregion

    #region CallbackDataFilterAttribute

    [Theory]
    [InlineData("confirm", "confirm", true)]
    [InlineData("confirm", "cancel", false)]
    [InlineData("CONFIRM", "confirm", true)]
    public void CallbackDataFilter_MatchesExactData(string actual, string expected, bool match)
    {
        var filter = new CallbackDataFilterAttribute(expected);
        filter.CanPass(CallbackContext(actual)).Should().Be(match);
    }

    [Fact]
    public void CallbackDataFilter_MatchesPrefix()
    {
        var filter = CallbackDataFilterAttribute.WithPrefix("page:");
        filter.CanPass(CallbackContext("page:2")).Should().BeTrue();
        filter.CanPass(CallbackContext("pages")).Should().BeFalse();
    }

    #endregion
}
