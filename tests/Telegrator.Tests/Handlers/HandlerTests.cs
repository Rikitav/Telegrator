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

/// <summary>
/// РўРµСЃС‚С‹ РґР»СЏ РѕР±СЂР°Р±РѕС‚С‡РёРєРѕРІ РѕР±РЅРѕРІР»РµРЅРёР№.
///
/// РџРђР РђР”РР“РњР« РўР•РЎРўРР РћР’РђРќРРЇ:
/// 1. Mocking - СЃРѕР·РґР°РЅРёРµ РјРѕРєРѕРІ РґР»СЏ РёР·РѕР»СЏС†РёРё Р·Р°РІРёСЃРёРјРѕСЃС‚РµР№
/// 2. Dependency Injection - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ С‡РµСЂРµР· РёРЅС‚РµСЂС„РµР№СЃС‹
/// 3. Test Doubles - РёСЃРїРѕР»СЊР·РѕРІР°РЅРёРµ Р·Р°РіР»СѓС€РµРє РІРјРµСЃС‚Рѕ СЂРµР°Р»СЊРЅС‹С… РѕР±СЉРµРєС‚РѕРІ
/// 4. Behavior Verification - РїСЂРѕРІРµСЂРєР° РїРѕРІРµРґРµРЅРёСЏ, Р° РЅРµ С‚РѕР»СЊРєРѕ СЂРµР·СѓР»СЊС‚Р°С‚Р°
/// 5. Exception Testing - С‚РµСЃС‚РёСЂРѕРІР°РЅРёРµ РёСЃРєР»СЋС‡РµРЅРёР№
/// </summary>
public class HandlerTests
{
    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ Р±Р°Р·РѕРІРѕРіРѕ РѕР±СЂР°Р±РѕС‚С‡РёРєР° РѕР±РЅРѕРІР»РµРЅРёР№.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј Р°Р±СЃС‚СЂР°РєС‚РЅС‹Р№ РєР»Р°СЃСЃ С‡РµСЂРµР· РєРѕРЅРєСЂРµС‚РЅСѓСЋ СЂРµР°Р»РёР·Р°С†РёСЋ
    /// </summary>
    [Fact]
    public async Task UpdateHandlerBase_ShouldExecuteAndMarkLifetimeAsEnded()
    {
        // Arrange
        var mockContainer = new Mock<IHandlerContainer<Message>>();
        var testHandler = new TestUpdateHandler();
        var describedDescriptor = new DescribedHandlerDescriptor(
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
        var mockContainerFactory = new Mock<IHandlerContainerFactory>();
        mockContainerFactory.Setup(f => f.CreateContainer(It.IsAny<DescribedHandlerDescriptor>())).Returns(mockContainer.Object);

        var mockRouter = new Mock<IUpdateRouter>();
        mockRouter.Setup(r => r.DefaultContainerFactory).Returns(mockContainerFactory.Object);

        var properDescribed = new DescribedHandlerDescriptor(
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

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ РїСЂРѕРІРµСЂРєРё С‚РѕРєРµРЅР° Р¶РёР·РЅРµРЅРЅРѕРіРѕ С†РёРєР»Р°.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј СЃРѕСЃС‚РѕСЏРЅРёРµ РѕР±СЉРµРєС‚РѕРІ
    /// </summary>
    [Fact]
    public void HandlerLifetimeToken_ShouldTrackLifetimeCorrectly()
    {
        // Arrange
        var handler = new TestUpdateHandler();

        // Act & Assert
        handler.LifetimeToken.IsEnded.Should().BeFalse();

        // Act
        handler.LifetimeToken.LifetimeEnded();

        // Assert
        handler.LifetimeToken.IsEnded.Should().BeTrue();
    }

    /// <summary>
    /// РўРµСЃС‚ РґР»СЏ РїСЂРѕРІРµСЂРєРё РѕС‚РјРµРЅС‹ РѕРїРµСЂР°С†РёРё.
    ///
    /// РџР РРќР¦РРџ: РўРµСЃС‚РёСЂСѓРµРј Р°СЃРёРЅС…СЂРѕРЅРЅС‹Рµ РѕРїРµСЂР°С†РёРё Рё РѕС‚РјРµРЅСѓ
    /// </summary>
    [Fact]
    public async Task UpdateHandlerBase_ShouldHandleCancellation()
    {
        // Arrange
        var mockContainer = new Mock<IHandlerContainer<Message>>();
        var testHandler = new TestUpdateHandler();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // РћС‚РјРµРЅСЏРµРј СЃСЂР°Р·Сѓ

        // Act & Assert
        await testHandler.Invoking(h => h.Execute(mockContainer.Object, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }
}
