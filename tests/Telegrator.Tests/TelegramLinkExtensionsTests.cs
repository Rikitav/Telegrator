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
using Xunit;

namespace Telegrator.Tests;

public class TelegramLinkExtensionsTests
{
    #region User Links

    [Fact]
    public void GetUserLink_ShouldReturnDeepLink()
    {
        User user = new User { Id = 123456, Username = "testuser" };

        user.GetUserLink().Should().Be("tg://user?id=123456");
    }

    [Fact]
    public void GetPublicLink_ForUser_WithUsername_ShouldReturnTmeLink()
    {
        User user = new User { Id = 1, Username = "testuser" };

        user.GetPublicLink().Should().Be("https://t.me/testuser");
    }

    [Fact]
    public void GetPublicLink_ForUser_WithoutUsername_ShouldReturnNull()
    {
        User user = new User { Id = 1 };

        user.GetPublicLink().Should().BeNull();
    }

    [Fact]
    public void GetDeepLink_ForUser_ShouldBeAliasToGetUserLink()
    {
        User user = new User { Id = 42 };

        user.GetDeepLink().Should().Be(user.GetUserLink());
    }

    [Fact]
    public void GetUserLink_WithNullUser_ShouldThrowArgumentNullException()
    {
        User? user = null;

        var act = () => user!.GetUserLink();
        act.Should().Throw<ArgumentNullException>().WithParameterName("user");
    }

    #endregion

    #region Chat Links

    [Fact]
    public void GetPublicLink_ForChat_WithUsername_ShouldReturnTmeLink()
    {
        Chat chat = new Chat { Id = 1, Username = "mychannel" };

        chat.GetPublicLink().Should().Be("https://t.me/mychannel");
    }

    [Fact]
    public void GetPublicLink_ForChat_WithoutUsername_ShouldReturnNull()
    {
        Chat chat = new Chat { Id = -100500 };

        chat.GetPublicLink().Should().BeNull();
    }

    [Fact]
    public void GetPublicLink_ForChat_WithNullUser_ShouldThrowArgumentNullException()
    {
        Chat? chat = null;

        var act = () => chat!.GetPublicLink();
        act.Should().Throw<ArgumentNullException>().WithParameterName("chat");
    }

    #endregion

    #region Message Links

    [Fact]
    public void GetMessageLink_ForPublicChat_ShouldReturnTmeMessageLink()
    {
        Message message = new Message
        {
            Id = 42,
            Chat = new Chat { Id = 1, Username = "channel" }
        };

        message.GetMessageLink().Should().Be("https://t.me/channel/42");
    }

    [Fact]
    public void GetMessageLink_ForPrivateChat_ShouldReturnNull()
    {
        Message message = new Message
        {
            Id = 1,
            Chat = new Chat { Id = 123 }
        };

        message.GetMessageLink().Should().BeNull();
    }

    [Fact]
    public void GetMessageLink_WithNullMessage_ShouldThrowArgumentNullException()
    {
        Message? message = null;

        var act = () => message!.GetMessageLink();
        act.Should().Throw<ArgumentNullException>().WithParameterName("message");
    }

    #endregion

    #region String Conversions

    [Theory]
    [InlineData("username", "https://t.me/username")]
    [InlineData("@username", "https://t.me/username")]
    [InlineData("  @username  ", "https://t.me/username")]
    public void ToTelegramPublicUrl_ShouldNormalizeAndReturnLink(string input, string expected)
    {
        input.ToTelegramPublicUrl().Should().Be(expected);
    }

    [Fact]
    public void ToTelegramPublicUrl_WithEmptyString_ShouldThrowArgumentException()
    {
        var act = () => "".ToTelegramPublicUrl();
        act.Should().Throw<ArgumentException>().WithParameterName("username");
    }

    [Theory]
    [InlineData("abc123", "https://t.me/+abc123")]
    [InlineData("+abc123", "https://t.me/+abc123")]
    public void ToTelegramInviteUrl_ShouldNormalizeAndReturnLink(string input, string expected)
    {
        input.ToTelegramInviteUrl().Should().Be(expected);
    }

    [Fact]
    public void ToTelegramInviteUrl_WithEmptyString_ShouldThrowArgumentException()
    {
        var act = () => "   ".ToTelegramInviteUrl();
        act.Should().Throw<ArgumentException>().WithParameterName("inviteHash");
    }

    #endregion

    #region Share & Deep Links

    [Fact]
    public void GetShareUrl_WithUrlOnly_ShouldReturnShareLink()
    {
        string result = TelegramLinkExtensions.GetShareUrl("https://example.com");

        result.Should().Be("https://t.me/share/url?url=https%3A%2F%2Fexample.com");
    }

    [Fact]
    public void GetShareUrl_WithUrlAndText_ShouldAppendText()
    {
        string result = TelegramLinkExtensions.GetShareUrl("https://example.com", "Check this out");

        result.Should().Be("https://t.me/share/url?url=https%3A%2F%2Fexample.com&text=Check%20this%20out");
    }

    [Fact]
    public void GetShareUrl_WithEmptyUrl_ShouldThrowArgumentException()
    {
        var act = () => TelegramLinkExtensions.GetShareUrl("  ");
        act.Should().Throw<ArgumentException>().WithParameterName("url");
    }

    [Fact]
    public void GetSettingsDeepLink_ShouldReturnSettingsLink()
    {
        TelegramLinkExtensions.GetSettingsDeepLink().Should().Be("tg://settings");
    }

    [Fact]
    public void GetAddStickersDeepLink_ShouldReturnEncodedLink()
    {
        TelegramLinkExtensions.GetAddStickersDeepLink("my_pack")
            .Should().Be("tg://addstickers?set=my_pack");
    }

    [Fact]
    public void GetAddStickersDeepLink_WithEmptyName_ShouldThrowArgumentException()
    {
        var act = () => TelegramLinkExtensions.GetAddStickersDeepLink("");
        act.Should().Throw<ArgumentException>().WithParameterName("setName");
    }

    [Fact]
    public void GetResolveDeepLink_ShouldReturnResolveLink()
    {
        TelegramLinkExtensions.GetResolveDeepLink("@durov")
            .Should().Be("tg://resolve?domain=durov");
    }

    [Fact]
    public void GetResolveDeepLink_WithEmptyUsername_ShouldThrowArgumentException()
    {
        var act = () => TelegramLinkExtensions.GetResolveDeepLink("   ");
        act.Should().Throw<ArgumentException>().WithParameterName("username");
    }

    [Fact]
    public void GetJoinDeepLink_ShouldReturnJoinLink()
    {
        TelegramLinkExtensions.GetJoinDeepLink("+AbCdEf")
            .Should().Be("tg://join?invite=AbCdEf");
    }

    [Fact]
    public void GetJoinDeepLink_WithEmptyHash_ShouldThrowArgumentException()
    {
        var act = () => TelegramLinkExtensions.GetJoinDeepLink("");
        act.Should().Throw<ArgumentException>().WithParameterName("inviteHash");
    }

    [Fact]
    public void GetMessageDeepLink_ShouldReturnEncodedMessageLink()
    {
        TelegramLinkExtensions.GetMessageDeepLink("Hello world")
            .Should().Be("tg://msg?text=Hello%20world");
    }

    [Fact]
    public void GetMessageDeepLink_WithEmptyText_ShouldThrowArgumentException()
    {
        var act = () => TelegramLinkExtensions.GetMessageDeepLink("");
        act.Should().Throw<ArgumentException>().WithParameterName("text");
    }

    #endregion
}
