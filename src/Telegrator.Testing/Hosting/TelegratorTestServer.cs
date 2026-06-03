/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

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
