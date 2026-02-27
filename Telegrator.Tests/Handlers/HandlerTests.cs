using FluentAssertions;
using Moq;
using Telegram.Bot.Types;
using Telegrator.Handlers;
using Xunit;

namespace Telegrator.Tests.Handlers
{
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
        /*s
        /// <summary>
        /// Тест для базового обработчика обновлений.
        /// 
        /// ПРИНЦИП: Тестируем абстрактный класс через конкретную реализацию
        /// </summary>
        [Fact]
        public async Task UpdateHandlerBase_ShouldExecuteAndMarkLifetimeAsEnded()
        {
            // Arrange
            var mockContainer = new Mock<IAbstractHandlerContainer<Message>>();
            var testHandler = new TestUpdateHandler();

            // Act
            await testHandler.Execute(mockContainer.Object);

            // Assert
            testHandler.WasExecuted.Should().BeTrue();
            testHandler.LifetimeToken.IsEnded.Should().BeTrue();
        }
        */

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
} 