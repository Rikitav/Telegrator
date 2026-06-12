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

using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Mediation;
using Telegrator.Providers;
using Telegrator.States;

namespace Telegrator.Testing;

/// <summary>
/// A specialized testing analog of TelegratorClient that mimics the production service,
/// but allows mimicking updates and mocking the underlying ITelegramBotClient.
/// </summary>
public class TestTelegratorClient : ITelegratorBot, ICollectingProvider
{
    private UpdateRouter? _updateRouter;
    private readonly Mock<ITelegramBotClient> _clientMock;

    /// <inheritdoc/>
    public TelegratorOptions Options { get; }

    /// <inheritdoc/>
    public IHandlersCollection Handlers { get; }

    /// <inheritdoc/>
    public ITelegramBotInfo BotInfo { get; }

    /// <inheritdoc/>
    public IUpdateRouter UpdateRouter => _updateRouter ?? throw new InvalidOperationException("Router's not created yet. Invoke `StartTestReceiving` to initialize this property.");

    /// <summary>
    /// Gets the Mock of the underlying ITelegramBotClient to verify calls (e.g. Verify SendTextMessageAsync).
    /// </summary>
    public Mock<ITelegramBotClient> ClientMock => _clientMock;

    /// <summary>
    /// Gets the exposed mocked ITelegramBotClient.
    /// </summary>
    public ITelegramBotClient Client => _clientMock.Object;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestTelegratorClient"/> class.
    /// </summary>
    /// <param name="telegratorOptions">The Telegrator options.</param>
    /// <param name="botUser">Optional custom user info for the bot.</param>
    public TestTelegratorClient(TelegratorOptions? telegratorOptions = null, User? botUser = null)
    {
        Options = telegratorOptions ?? new TelegratorOptions();
        Handlers = new HandlersCollection(Options);

        botUser ??= new User
        {
            Id = 123456789,
            IsBot = true,
            FirstName = "TestBot",
            Username = "TestBot"
        };

        BotInfo = new TelegramBotInfo(botUser);
        _clientMock = new Mock<ITelegramBotClient>();
        _clientMock.Setup(x => x.BotId).Returns(botUser.Id);
    }

    /// <summary>
    /// Initializes the update router and providers for testing without actually connecting to Telegram.
    /// </summary>
    public void StartTestReceiving()
    {
        HandlersProvider handlerProvider = new HandlersProvider(Handlers, Options);
        AwaitingProvider awaitingProvider = new AwaitingProvider(Options);
        DefaultStateStorage stateStorage = new DefaultStateStorage();

        _updateRouter = new UpdateRouter(handlerProvider, awaitingProvider, stateStorage, Options, BotInfo);
    }

    /// <inheritdoc/>
    public Task StartReceivingAsync(Telegram.Bot.Polling.ReceiverOptions? receiverOptions = null, CancellationToken cancellationToken = default)
    {
        StartTestReceiving();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Simulates receiving an update from Telegram by pushing it directly into the update router.
    /// </summary>
    /// <param name="update">The update to simulate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the update processing.</returns>
    public async Task EmitUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (_updateRouter == null)
            throw new InvalidOperationException("You must call StartTestReceiving() before emitting updates.");

        await _updateRouter.HandleUpdateAsync(Client, update, cancellationToken);
    }

    /// <summary>
    /// Simulates receiving a message update.
    /// </summary>
    public async Task EmitMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        if (message.Id == 0)
            message.Id = 1; // Telegrator filters requires message Id, so we set a default if not provided

        Update update = new Update { Message = message };
        await EmitUpdateAsync(update, cancellationToken);
    }
}
