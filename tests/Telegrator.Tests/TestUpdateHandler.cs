using Telegram.Bot.Types;
using Telegrator.Handlers;

namespace Telegrator.Tests;

/// <summary>
/// Вспомогательный класс для тестирования абстрактного UpdateHandlerBase.
/// ПРИНЦИП: Создание тестовых двойников для абстрактных классов
/// </summary>
[MessageHandler]
internal class TestUpdateHandler : MessageHandler
{
    public bool WasExecuted { get; private set; }

    public override Task<Result> Execute(IHandlerContainer<Message> container, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WasExecuted = true;
        return Task.FromResult(Result.Ok());
    }
}
