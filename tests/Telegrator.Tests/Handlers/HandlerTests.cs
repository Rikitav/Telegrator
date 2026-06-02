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
/// Тесты для обработчиков обновлений.
///
/// ПАРАДИГМЫ ТЕСТИРОВАНИЯ:
/// 1. Mocking - создание моков для изоляции зависимостей
/// 2. Dependency Injection - тестирование через интерфейсы
/// 3. Test Doubles - использование заглушек вместо реальных объектов
/// 4. Behavior Verification - проверка поведения, а не только результата
/// 5. Exception Testing - тестирование исключений
/// </summary>
public class HandlerTests
{
    /// <summary>
    /// Тест для базового обработчика обновлений.
    ///
    /// ПРИНЦИП: Тестируем абстрактный класс через конкретную реализацию
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
    /// Тест для проверки токена жизненного цикла.
    ///
    /// ПРИНЦИП: Тестируем состояние объектов
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
    /// Тест для проверки отмены операции.
    ///
    /// ПРИНЦИП: Тестируем асинхронные операции и отмену
    /// </summary>
    [Fact]
    public async Task UpdateHandlerBase_ShouldHandleCancellation()
    {
        // Arrange
        var mockContainer = new Mock<IHandlerContainer<Message>>();
        var testHandler = new TestUpdateHandler();
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel(); // Отменяем сразу

        // Act & Assert
        await testHandler.Invoking(h => h.Execute(mockContainer.Object, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }
}
