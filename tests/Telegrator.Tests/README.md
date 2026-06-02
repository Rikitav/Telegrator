# Тесты для Telegrator

Этот проект содержит комплексные тесты для библиотеки Telegrator, демонстрирующие различные парадигмы и подходы к тестированию.

## Структура тестов

### 1. Filters (Фильтры)
**Файлы:** `Filters/FilterTests.cs`, `Filters/FilterCompositionTests.cs`

**Парадигмы тестирования:**
- **AAA (Arrange-Act-Assert)** — структура теста: подготовка, действие, проверка
- **Given-When-Then** — альтернативная формулировка AAA для лучшей читаемости
- **Тестирование граничных случаев** и исключений
- **Использование моков** для изоляции тестируемого кода
- **Тестирование как позитивных, так и негативных сценариев**

**Что тестируется:**
- Базовые фильтры (AnyFilter, ReverseFilter, AndFilter, OrFilter)
- Композиция фильтров
- Логические операции между фильтрами
- Свойства фильтров (IsCollectible)

### 2. Handlers (Обработчики)
**Файлы:** `Handlers/HandlerTests.cs`, `Handlers/HandlerExecutionTests.cs`

**Парадигмы тестирования:**
- **Mocking** — создание моков для изоляции зависимостей
- **Dependency Injection** — тестирование через интерфейсы
- **Test Doubles** — использование заглушек вместо реальных объектов
- **Behavior Verification** — проверка поведения, а не только результата
- **Exception Testing** — тестирование исключений

**Что тестируется:**
- Базовые обработчики обновлений
- Жизненный цикл обработчиков
- Обработка исключений
- Отмена операций
- Токены жизненного цикла

### 3. Descriptors (Дескрипторы)
**Файлы:** `Descriptors/HandlerDescriptorTests.cs`, `Descriptors/DescriptorFiltersSetTests.cs`

**Парадигмы тестирования:**
- **Builder Pattern Testing** — тестирование паттерна строителя
- **Factory Pattern Testing** — тестирование фабричных методов
- **Immutable Object Testing** — тестирование неизменяемых объектов
- **Configuration Testing** — тестирование конфигурации объектов
- **Validation Testing** — тестирование валидации данных

**Что тестируется:**
- Создание дескрипторов обработчиков
- Различные типы дескрипторов (General, Singleton, Keyed, Implicit)
- Наборы фильтров (`DescriptorFiltersSet`)
- Наборы аспектов (`DescriptorAspectsSet`)
- Индексаторы
- Валидация параметров

### 4. Providers (Провайдеры)
**Файлы:** `Providers/HandlersProviderTests.cs`, `Providers/AwaitingProviderTests.cs`

**Парадигмы тестирования:**
- **Service Layer Testing** — тестирование сервисного слоя
- **Integration Testing** — тестирование интеграции компонентов
- **Collection Testing** — тестирование коллекций и их операций
- **Provider Pattern Testing** — тестирование паттерна провайдера
- **Dependency Resolution Testing** — тестирование разрешения зависимостей

**Что тестируется:**
- Провайдеры обработчиков (`HandlersProvider`)
- Провайдеры ожидания (`AwaitingProvider`)
- Коллекции обработчиков (`HandlersCollection`)
- Операции с коллекциями
- Интеграция между провайдерами

### 5. Mediation (Медиация)
**Файлы:** `Mediation/UpdateRouterTests.cs`, `Mediation/UpdateHandlersPoolTests.cs`

**Парадигмы тестирования:**
- **Host Testing** — тестирование хостинга приложений
- **Configuration Testing** — тестирование конфигурации
- **Dependency Injection Testing** — тестирование DI контейнера
- **Builder Pattern Testing** — тестирование паттерна строителя
- **Lifecycle Testing** — тестирование жизненного цикла приложения

**Что тестируется:**
- `UpdateRouter` — маршрутизация обновлений
- `UpdateHandlersPool` — пул обработчиков с bounded channel
- Жизненный цикл хостов
- Валидация параметров

### 6. Integration (Интеграционные тесты)
**Файлы:** `Integration/TelegratorClientIntegrationTests.cs`

**Парадигмы тестирования:**
- **Integration Testing** — тестирование взаимодействия компонентов
- **End-to-End Testing** — тестирование полного потока
- **System Testing** — тестирование системы в целом
- **Workflow Testing** — тестирование рабочих процессов
- **Scenario Testing** — тестирование сценариев использования

**Что тестируется:**
- Полный цикл обработки обновлений через `TestTelegratorClient`
- Взаимодействие фильтров и обработчиков
- Механизм ожидания (`AwaitMessage`)
- Жизненный цикл обработчиков
- Интеграция компонентов

### 7. Extensions (Расширения типов)
**Файлы:** `SimpleTypesExtensionsTests.cs`

**Парадигмы тестирования:**
- **Extension Method Testing** — тестирование методов расширения
- **Reflection Testing** — тестирование рефлексии
- **String Manipulation Testing** — тестирование манипуляций со строками

**Что тестируется:**
- Расширения коллекций (`ToReadOnlyDictionary`)
- Расширения строк (`SliceBy`, `HasFlag`)
- Расширения типов (`IsFilterType`, `IsHandlerAbstract`, `HasParameterlessCtor`)

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

### С покрытием кода
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Через Visual Studio
1. Откройте Test Explorer
2. Запустите все тесты или выберите конкретные

### Через Rider
1. Откройте Unit Tests window
2. Запустите тесты

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
