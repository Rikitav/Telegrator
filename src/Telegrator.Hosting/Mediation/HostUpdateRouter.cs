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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Core.States;

namespace Telegrator.Mediation;

/// <inheritdoc/>
public class HostUpdateRouter : UpdateRouter
{
    /// <summary>
    /// <see cref="ILogger"/> of this router
    /// </summary>
    protected readonly ILogger<HostUpdateRouter> Logger;

    /// <inheritdoc/>
    public HostUpdateRouter(
        IHandlersProvider handlersProvider,
        IAwaitingProvider awaitingProvider,
        IStateStorage stateStorage,
        IOptions<TelegratorOptions> options,
        ITelegramBotInfo botInfo,
        ILogger<HostUpdateRouter> logger) : base(handlersProvider, awaitingProvider, stateStorage, options.Value, botInfo)
    {
        Logger = logger;
        ExceptionHandler = new DefaultRouterExceptionHandler(HandleException);
    }

    /// <inheritdoc/>
    public override Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        //Logger.LogInformation("Received update of type \"{type}\"", update.Type);
        return base.HandleUpdateAsync(botClient, update, cancellationToken);
    }

    /// <summary>
    /// Default exception handler of this router
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="exception"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    public void HandleException(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        if (exception is HandlerFaultedException handlerFaultedException)
        {
            Logger.LogError("\"{handler}\" handler's execution was faulted :\n{exception}",
                handlerFaultedException.HandlerInfo.ToString(),
                handlerFaultedException.InnerException?.ToString() ?? "No inner exception");
            return;
        }

        Logger.LogError("Exception was thrown during update routing faulted :\n{exception}", exception.ToString());
    }
}
