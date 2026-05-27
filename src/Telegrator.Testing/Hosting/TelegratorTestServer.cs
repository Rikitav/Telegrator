#if NET10_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;

namespace Telegrator.Testing.Hosting;

/// <summary>
/// A specialized testing server for the hosting pipeline that mimics the production service.
/// </summary>
public class TelegratorTestServer
{
    private readonly IUpdateRouter _updateRouter;
    private readonly ITelegramBotClient _client;

    /// <summary>
    /// Gets the Mock of the underlying ITelegramBotClient to verify calls.
    /// This requires the client registered in DI to be the mock object.
    /// </summary>
    public Mock<ITelegramBotClient> ClientMock { get; }

    /// <summary>
    /// Gets the exposed mocked ITelegramBotClient.
    /// </summary>
    public ITelegramBotClient Client => _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="TelegratorTestServer"/> class.
    /// </summary>
    /// <param name="updateRouter">The update router from DI.</param>
    /// <param name="client">The telegram bot client from DI.</param>
    public TelegratorTestServer(IUpdateRouter updateRouter, ITelegramBotClient client)
    {
        _updateRouter = updateRouter;
        _client = client;
        ClientMock = Mock.Get(client);
    }

    /// <summary>
    /// Simulates receiving an update from Telegram by pushing it directly into the update router.
    /// </summary>
    /// <param name="update">The update to simulate.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the update processing.</returns>
    public async Task EmitUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        await _updateRouter.HandleUpdateAsync(_client, update, cancellationToken);
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
#endif
