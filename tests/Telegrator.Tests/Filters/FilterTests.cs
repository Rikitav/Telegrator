using FluentAssertions;
using Telegram.Bot.Types;
using Telegrator.Filters;
using Telegrator.Filters.Components;
using Xunit;

#pragma warning disable CS8625
namespace Telegrator.Tests.Filters
{
    /// <summary>
    /// Тесты для базовых фильтров.
    /// 
    /// ПАРАДИГМЫ ТЕСТИРОВАНИЯ:
    /// 1. AAA (Arrange-Act-Assert) - структура теста: подготовка, действие, проверка
    /// 2. Given-When-Then - альтернативная формулировка AAA для лучшей читаемости
    /// 3. Тестирование граничных случаев и исключений
    /// 4. Использование моков для изоляции тестируемого кода
    /// 5. Тестирование как позитивных, так и негативных сценариев
    /// </summary>
    public class FilterTests
    {
        /// <summary>
        /// Тест для AnyFilter - фильтр, который всегда проходит.
        /// 
        /// ПРИНЦИП: Тестируем базовое поведение - фильтр должен всегда возвращать true
        /// </summary>
        [Fact]
        public void AnyFilter_ShouldAlwaysPass()
        {
            // Arrange (Given) - подготовка тестовых данных
            var anyFilter = Filter<Update>.Any();
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act (When) - выполнение тестируемого действия
            var result = anyFilter.CanPass(context);

            // Assert (Then) - проверка результата
            result.Should().BeTrue();
        }

        /// <summary>
        /// Тест для ReverseFilter - инвертирование результата фильтра.
        /// 
        /// ПРИНЦИП: Тестируем композицию фильтров и логику инверсии
        /// </summary>
        [Fact]
        public void ReverseFilter_ShouldInvertResult()
        {
            // Arrange
            var alwaysTrueFilter = Filter<Update>.Any();
            var reverseFilter = alwaysTrueFilter.Not();
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act
            var result = reverseFilter.CanPass(context);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Тест для AndFilter - логическое И между фильтрами.
        /// 
        /// ПРИНЦИП: Тестируем комбинирование фильтров и логику И
        /// </summary>
        [Theory]
        [InlineData(true, true, true)]   // Оба фильтра проходят
        [InlineData(true, false, false)] // Первый проходит, второй нет
        [InlineData(false, true, false)] // Первый не проходит, второй проходит
        [InlineData(false, false, false)] // Оба фильтра не проходят
        public void AndFilter_ShouldCombineFiltersWithAndLogic(bool firstResult, bool secondResult, bool expectedResult)
        {
            // Arrange
            var firstFilter = Filter<Update>.If(_ => firstResult);
            var secondFilter = Filter<Update>.If(_ => secondResult);
            var andFilter = firstFilter.And(secondFilter);
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act
            var result = andFilter.CanPass(context);

            // Assert
            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// Тест для OrFilter - логическое ИЛИ между фильтрами.
        /// 
        /// ПРИНЦИП: Тестируем комбинирование фильтров и логику ИЛИ
        /// </summary>
        [Theory]
        [InlineData(true, true, true)]   // Оба фильтра проходят
        [InlineData(true, false, true)]  // Первый проходит, второй нет
        [InlineData(false, true, true)]  // Первый не проходит, второй проходит
        [InlineData(false, false, false)] // Оба фильтра не проходят
        public void OrFilter_ShouldCombineFiltersWithOrLogic(bool firstResult, bool secondResult, bool expectedResult)
        {
            // Arrange
            var firstFilter = Filter<Update>.If(_ => firstResult);
            var secondFilter = Filter<Update>.If(_ => secondResult);
            var orFilter = firstFilter.Or(secondFilter);
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act
            var result = orFilter.CanPass(context);

            // Assert
            result.Should().Be(expectedResult);
        }

        /// <summary>
        /// Тест для CompiledFilter - компиляция нескольких фильтров.
        /// 
        /// ПРИНЦИП: Тестируем сложную композицию фильтров
        /// </summary>
        [Fact]
        public void CompiledFilter_ShouldPassOnlyWhenAllFiltersPass()
        {
            // Arrange
            var filter1 = Filter<Update>.If(_ => true);
            var filter2 = Filter<Update>.If(_ => true);
            var filter3 = Filter<Update>.If(_ => false);
            
            var compiledFilter = new CompiledFilter<Update>(filter1, filter2, filter3);
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act
            var result = compiledFilter.CanPass(context);

            // Assert
            result.Should().BeFalse(); // Должен вернуть false, так как filter3 возвращает false
        }

        /// <summary>
        /// Тест для проверки IsCollectible свойства.
        /// 
        /// ПРИНЦИП: Тестируем свойства объектов
        /// </summary>
        [Fact]
        public void Filter_IsCollectible_ShouldBeTrueForAnyFilter()
        {
            // Arrange
            var anyFilter = Filter<Update>.Any();

            // Act
            var isCollectible = anyFilter.IsCollectible;

            // Assert
            isCollectible.Should().BeFalse();
        }

        /// <summary>
        /// Тест для FunctionFilter - фильтр на основе функции.
        /// 
        /// ПРИНЦИП: Тестируем создание фильтров из функций
        /// </summary>
        [Fact]
        public void FunctionFilter_ShouldUseProvidedFunction()
        {
            // Arrange
            var wasCalled = false;
            var functionFilter = Filter<Update>.If(_ => 
            {
                wasCalled = true;
                return true;
            });
            var context = new FilterExecutionContext<Update>(new TelegramBotInfo(null), new Update(), new Update(), new Dictionary<string, object>(), new CompletedFiltersList());

            // Act
            var result = functionFilter.CanPass(context);

            // Assert
            result.Should().BeTrue();
            wasCalled.Should().BeTrue();
        }
    }
} 