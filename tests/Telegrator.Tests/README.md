# Тесты для Telegrator

Этот проект содержит комплексные тесты для библиотеки Telegrator, демонстрирующие различные парадигмы и подходы к тестированию.

## Структура тестов

### 1. Filters (Фильтры)
**Файл:** `Filters/FilterTests.cs`

**Парадигмы тестирования:**
- **AAA (Arrange-Act-Assert)** - структура теста: подготовка, действие, проверка
- **Given-When-Then** - альтернативная формулировка AAA для лучшей читаемости
- **Тестирование граничных случаев** и исключений
- **Использование моков** для изоляции тестируемого кода
- **Тестирование как позитивных, так и негативных сценариев**

**Что тестируется:**
- Базовые фильтры (AnyFilter, ReverseFilter, AndFilter, OrFilter)
- Компиляция фильтров
- Логические операции между фильтрами
- Свойства фильтров (IsCollectible)

### 2. Handlers (Обработчики)
**Файл:** `Handlers/HandlerTests.cs`

**Парадигмы тестирования:**
- **Mocking** - создание моков для изоляции зависимостей
- **Dependency Injection** - тестирование через интерфейсы
- **Test Doubles** - использование заглушек вместо реальных объектов
- **Behavior Verification** - проверка поведения, а не только результата
- **Exception Testing** - тестирование исключений

**Что тестируется:**
- Базовые обработчики обновлений
- Жизненный цикл обработчиков
- Обработка исключений
- Отмена операций
- Токены жизненного цикла

### 3. Descriptors (Дескрипторы)
**Файл:** `Descriptors/HandlerDescriptorTests.cs`

**Парадигмы тестирования:**
- **Builder Pattern Testing** - тестирование паттерна строителя
- **Factory Pattern Testing** - тестирование фабричных методов
- **Immutable Object Testing** - тестирование неизменяемых объектов
- **Configuration Testing** - тестирование конфигурации объектов
- **Validation Testing** - тестирование валидации данных

**Что тестируется:**
- Создание дескрипторов обработчиков
- Различные типы дескрипторов (General, Singleton, Keyed, Implicit)
- Наборы фильтров
- Индексаторы
- Валидация параметров

### 4. Providers (Провайдеры)
**Файл:** `Providers/ProviderTests.cs`

**Парадигмы тестирования:**
- **Service Layer Testing** - тестирование сервисного слоя
- **Integration Testing** - тестирование интеграции компонентов
- **Collection Testing** - тестирование коллекций и их операций
- **Provider Pattern Testing** - тестирование паттерна провайдера
- **Dependency Resolution Testing** - тестирование разрешения зависимостей

**Что тестируется:**
- Провайдеры обработчиков
- Коллекции обработчиков
- Операции с коллекциями
- Интеграция между провайдерами

### 5. Hosting (Хостинг)
**Файл:** `Hosting/HostingTests.cs`

**Парадигмы тестирования:**
- **Host Testing** - тестирование хостинга приложений
- **Configuration Testing** - тестирование конфигурации
- **Dependency Injection Testing** - тестирование DI контейнера
- **Builder Pattern Testing** - тестирование паттерна строителя
- **Lifecycle Testing** - тестирование жизненного цикла приложения

**Что тестируется:**
- Строители хостов
- Конфигурация ботов
- Жизненный цикл хостов
- Валидация параметров

### 6. Collections (Коллекции)
**Файл:** `Collections/CollectionTests.cs`

**Парадигмы тестирования:**
- **Collection Testing** - тестирование коллекций и их операций
- **List Testing** - тестирование списков
- **Indexing Testing** - тестирование индексации
- **Enumeration Testing** - тестирование перечисления
- **Capacity Testing** - тестирование емкости коллекций

**Что тестируется:**
- Списки дескрипторов обработчиков
- Списки завершенных фильтров
- Операции с коллекциями
- Производительность
- Потокобезопасность

### 7. Integration (Интеграционные тесты)
**Файл:** `Integration/IntegrationTests.cs`

**Парадигмы тестирования:**
- **Integration Testing** - тестирование взаимодействия компонентов
- **End-to-End Testing** - тестирование полного потока
- **System Testing** - тестирование системы в целом
- **Workflow Testing** - тестирование рабочих процессов
- **Scenario Testing** - тестирование сценариев использования

**Что тестируется:**
- Полный цикл обработки обновлений
- Взаимодействие фильтров и обработчиков
- Композиция фильтров
- Жизненный цикл обработчиков
- Интеграция компонентов

### 8. TestHelpers (Вспомогательные утилиты)
**Файл:** `TestHelpers/TestUtilities.cs`

**Парадигмы тестирования:**
- **Test Utilities** - создание вспомогательных методов для тестов
- **Test Data Builders** - построители тестовых данных
- **Mock Factories** - фабрики моков
- **Test Fixtures** - фикстуры для тестов
- **Test Helpers** - вспомогательные классы для тестирования

**Что предоставляется:**
- Утилиты для создания тестовых данных
- Фабрики моков
- Вспомогательные классы
- Тестовые обработчики

## Основные принципы тестирования

### 1. AAA (Arrange-Act-Assert)
```csharp
[Fact]
public void TestExample()
{
    // Arrange - подготовка тестовых данных
    var filter = Filter<Update>.Any();
    var context = TestUtilities.CreateFilterContext();

    // Act - выполнение тестируемого действия
    var result = filter.CanPass(context);

    // Assert - проверка результата
    result.Should().BeTrue();
}
```

### 2. Тестирование граничных случаев
```csharp
[Theory]
[InlineData(-1)]
[InlineData(1)]
[InlineData(100)]
public void TestBoundaryConditions(int invalidIndex)
{
    // Тестируем граничные случаи
}
```

### 3. Использование моков
```csharp
[Fact]
public void TestWithMocks()
{
    // Arrange
    var mockClient = new Mock<ITelegramBotClient>();
    var mockContainer = TestUtilities.CreateMockHandlerContainer();

    // Act & Assert
    // Тестирование с моками
}
```

### 4. Тестирование исключений
```csharp
[Fact]
public void TestExceptions()
{
    // Act & Assert
    Action action = () => { /* код, который должен выбросить исключение */ };
    action.Should().Throw<ArgumentException>();
}
```

## Запуск тестов

### Через командную строку
```bash
dotnet test
```

### Через Visual Studio
1. Откройте Test Explorer
2. Запустите все тесты или выберите конкретные

### Через Rider
1. Откройте Unit Tests window
2. Запустите тесты

## Покрытие кода

Для анализа покрытия кода используйте:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Рекомендации по написанию тестов

1. **Именование тестов** должно быть описательным и следовать паттерну `MethodName_Scenario_ExpectedResult`
2. **Каждый тест** должен тестировать только одну вещь
3. **Используйте моки** для изоляции зависимостей
4. **Тестируйте как позитивные, так и негативные сценарии**
5. **Группируйте связанные тесты** в отдельные классы
6. **Используйте вспомогательные методы** для создания тестовых данных
7. **Документируйте сложные тесты** с помощью комментариев

## Полезные ссылки

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [.NET Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/) 