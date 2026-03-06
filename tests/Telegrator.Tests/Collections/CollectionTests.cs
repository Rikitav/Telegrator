using FluentAssertions;
using Telegram.Bot.Types.Enums;
using Telegrator.Filters.Components;
using Telegrator.MadiatorCore.Descriptors;
using Xunit;

namespace Telegrator.Tests.Collections
{
    /// <summary>
    /// Тесты для коллекций.
    /// 
    /// ПАРАДИГМЫ ТЕСТИРОВАНИЯ:
    /// 1. Collection Testing - тестирование коллекций и их операций
    /// 2. List Testing - тестирование списков
    /// 3. Indexing Testing - тестирование индексации
    /// 4. Enumeration Testing - тестирование перечисления
    /// 5. Capacity Testing - тестирование емкости коллекций
    /// </summary>
    public class CollectionTests
    {
        /// <summary>
        /// Тест для HandlerDescriptorList - создание списка.
        /// 
        /// ПРИНЦИП: Тестируем создание коллекций
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_ShouldBeCreated()
        {
            // Arrange & Act
            var list = new HandlerDescriptorList();

            // Assert
            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - добавление дескриптора.
        /// 
        /// ПРИНЦИП: Тестируем добавление элементов в коллекцию
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_Add_ShouldAddDescriptor()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var descriptor = CreateTestDescriptor(UpdateType.Message);

            // Act
            list.Add(descriptor);

            // Assert
            list.Should().HaveCount(1);
            list.Should().Contain(descriptor);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - добавление нескольких дескрипторов.
        /// 
        /// ПРИНЦИП: Тестируем множественные операции
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_AddMultiple_ShouldAddAllDescriptors()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var descriptor1 = CreateTestDescriptor(UpdateType.Message);
            var descriptor2 = CreateTestDescriptor(UpdateType.CallbackQuery);
            var descriptor3 = CreateTestDescriptor(UpdateType.InlineQuery);

            // Act
            list.Add(descriptor1);
            list.Add(descriptor2);
            list.Add(descriptor3);

            // Assert
            list.Should().HaveCount(3);
            list.Should().Contain(descriptor1);
            list.Should().Contain(descriptor2);
            list.Should().Contain(descriptor3);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - получение по индексу.
        /// 
        /// ПРИНЦИП: Тестируем индексацию коллекций
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_Indexer_ShouldReturnDescriptorAtIndex()
        {
            // Arrange
            var descriptor = CreateTestDescriptor(UpdateType.Message);
            var list = new HandlerDescriptorList
            {
                descriptor
            };

            // Act
            var result = list[0];

            // Assert
            result.Should().Be(descriptor);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - получение по неверному индексу.
        /// 
        /// ПРИНЦИП: Тестируем исключения при некорректном доступе
        /// </summary>
        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(100)]
        public void HandlerDescriptorList_IndexerWithInvalidIndex_ShouldThrowArgumentOutOfRangeException(int invalidIndex)
        {
            // Arrange
            var list = new HandlerDescriptorList
            {
                CreateTestDescriptor(UpdateType.Message)
            };

            // Act & Assert
            list.Invoking(l => _ = l[invalidIndex])
                .Should().Throw<ArgumentOutOfRangeException>();
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - перечисление элементов.
        /// 
        /// ПРИНЦИП: Тестируем перечисление коллекций
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_ShouldBeEnumerable()
        {
            // Arrange
            var descriptor1 = CreateTestDescriptor(UpdateType.Message);
            var descriptor2 = CreateTestDescriptor(UpdateType.CallbackQuery);
            var list = new HandlerDescriptorList
            {
                descriptor1,
                descriptor2
            };

            // Act
            var enumeratedItems = list.ToList();

            // Assert
            enumeratedItems.Should().HaveCount(2);
            enumeratedItems.Should().Contain(descriptor1);
            enumeratedItems.Should().Contain(descriptor2);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - очистка списка.
        /// 
        /// ПРИНЦИП: Тестируем очистку коллекций
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_Clear_ShouldRemoveAllDescriptors()
        {
            // Arrange
            var list = new HandlerDescriptorList
            {
                CreateTestDescriptor(UpdateType.Message),
                CreateTestDescriptor(UpdateType.CallbackQuery)
            };

            // Act
            list.Clear();

            // Assert
            list.Should().BeEmpty();
            list.Should().HaveCount(0);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - проверка содержания элемента.
        /// 
        /// ПРИНЦИП: Тестируем поиск в коллекциях
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_Contains_ShouldReturnCorrectResult()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var descriptor = CreateTestDescriptor(UpdateType.Message);
            var nonExistentDescriptor = CreateTestDescriptor(UpdateType.CallbackQuery);

            // Act
            list.Add(descriptor);
            var containsExisting = list.Contains(descriptor);
            var containsNonExistent = list.Contains(nonExistentDescriptor);

            // Assert
            containsExisting.Should().BeTrue();
            containsNonExistent.Should().BeFalse();
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - удаление элемента.
        /// 
        /// ПРИНЦИП: Тестируем удаление элементов из коллекций
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_Remove_ShouldRemoveDescriptor()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var descriptor = CreateTestDescriptor(UpdateType.Message);
            list.Add(descriptor);

            // Act
            var removed = list.Remove(descriptor);

            // Assert
            removed.Should().BeTrue();
            list.Should().BeEmpty();
            list.Should().NotContain(descriptor);
        }

        /// <summary>
        /// Тест для HandlerDescriptorList - удаление несуществующего элемента.
        /// 
        /// ПРИНЦИП: Тестируем удаление несуществующих элементов
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_RemoveNonExistent_ShouldReturnFalse()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var nonExistentDescriptor = CreateTestDescriptor(UpdateType.CallbackQuery);

            // Act
            var removed = list.Remove(nonExistentDescriptor);

            // Assert
            removed.Should().BeFalse();
            list.Should().BeEmpty();
        }

        /// <summary>
        /// Тест для CompletedFiltersList - создание списка.
        /// 
        /// ПРИНЦИП: Тестируем создание специализированных коллекций
        /// </summary>
        [Fact]
        public void CompletedFiltersList_ShouldBeCreated()
        {
            // Arrange & Act
            var list = new CompletedFiltersList();

            // Assert
            list.Should().NotBeNull();
            list.Should().BeEmpty();
        }

        /// <summary>
        /// Тест для проверки производительности коллекций.
        /// 
        /// ПРИНЦИП: Тестируем производительность при большом количестве элементов
        /// </summary>
        [Fact]
        public void HandlerDescriptorList_ShouldHandleLargeNumberOfItems()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var itemsCount = 1000;

            // Act
            for (int i = 0; i < itemsCount; i++)
            {
                list.Add(CreateTestDescriptor(UpdateType.Message));
            }

            // Assert
            list.Should().HaveCount(itemsCount);
        }

        /// <summary>
        /// Тест для проверки потокобезопасности (базовый тест).
        /// 
        /// ПРИНЦИП: Тестируем базовую потокобезопасность
        /// </summary>
        [Fact]
        public async void HandlerDescriptorList_ShouldHandleConcurrentAccess()
        {
            // Arrange
            var list = new HandlerDescriptorList();
            var tasks = new List<Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < 10; j++)
                    {
                        list.Add(CreateTestDescriptor(UpdateType.Message));
                    }
                }));
            }

            await Task.WhenAll(tasks.ToArray());

            // Assert
            list.Should().HaveCount(100);
        }

        /// <summary>
        /// Вспомогательный метод для создания тестового дескриптора.
        /// </summary>
        private static HandlerDescriptor CreateTestDescriptor(UpdateType updateType)
        {
            return new HandlerDescriptor(DescriptorType.General, typeof(TestUpdateHandler));
        }
    }
} 