using Moq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
    private IUpdateRouter? _updateRouter;
    private readonly Mock<ITelegramBotClient> _clientMock;

    /// <inheritdoc/>
    public TelegratorOptions Options { get; }

    /// <inheritdoc/>
    public IHandlersCollection Handlers { get; }

    /// <inheritdoc/>
    public ITelegramBotInfo BotInfo { get; }

    /// <inheritdoc/>
    public IUpdateRouter UpdateRouter => _updateRouter ?? throw new System.InvalidOperationException("Router's not created yet. Invoke `StartTestReceiving` to initialize this property.");

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
            throw new System.InvalidOperationException("You must call StartTestReceiving() before emitting updates.");

        await _updateRouter.HandleUpdateAsync(Client, update, cancellationToken);
    }

    /// <summary>
    /// Simulates receiving a message update.
    /// </summary>
    public async Task EmitMessageAsync(Message message, CancellationToken cancellationToken = default)
    {
        Update update = new Update { Message = message };
        await EmitUpdateAsync(update, cancellationToken);
    }
}
