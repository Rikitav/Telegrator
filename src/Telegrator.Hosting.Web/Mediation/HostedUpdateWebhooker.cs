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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted telegram bots via Webhooks and queuing them to router
/// </summary>
public class HostedUpdateWebhooker : IHostedService
{
    private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

    private readonly ILogger<HostedUpdateWebhooker> _logger;
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateRouter _updateRouter;
    private readonly WebhookerOptions _options;

    /// <summary>
    /// Initiallizes new instance of <see cref="HostedUpdateWebhooker"/>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="botClient"></param>
    /// <param name="updateRouter"></param>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HostedUpdateWebhooker(ILogger<HostedUpdateWebhooker> logger, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<WebhookerOptions> options)
    {
        if (string.IsNullOrEmpty(options.Value.WebhookUri))
            throw new ArgumentNullException(nameof(options), "Option \"WebhookUrl\" must be set to subscribe for update recieving");

        _logger = logger;
        _botClient = botClient;
        _updateRouter = updateRouter;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = StartInternal(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _botClient.DeleteWebhook(_options.DropPendingUpdates, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Allows to remap receiving webhook endpoint and map new route to webhost.
    /// </summary>
    /// <param name="routeBuilder"></param>
    /// <param name="webhookUri"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task RemapWebhook(IEndpointRouteBuilder routeBuilder, string webhookUri, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(webhookUri, UriKind.Absolute, out Uri? _))
            throw new ArgumentException("invalid URL");

        //_options.WebhookUri = result.ToString();
        await SetWebhook(cancellationToken);
        MapWebhook(routeBuilder);
    }

    /// <summary>
    /// Maps bot webhook to application builder
    /// </summary>
    /// <param name="routeBuilder"></param>
    internal void MapWebhook(IEndpointRouteBuilder routeBuilder)
    {
        if (string.IsNullOrEmpty(_options.WebhookUri))
            return;

        string pattern = new UriBuilder(_options.WebhookUri).Path;
        routeBuilder.MapPost(pattern, (RequestDelegate)ReceiveUpdate);
    }

    private async Task StartInternal(CancellationToken cancellationToken)
    {
        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogInformation("Hosted update receiver starting");
            _logger.LogInformation("Receiving mode : WEB-HOOKING");
            _logger.LogInformation("Webhook URL : {WebhookUrl}", _options.WebhookUri);
            //_logger.LogInformation($"Webhhok status : https://api.telegram.org/bot{}/getWebhookInfo");
        }

        await SetWebhook(cancellationToken).ConfigureAwait(false);
    }

    private async Task SetWebhook(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.WebhookUri))
            return;

        await _botClient.SetWebhook(
            url: _options.WebhookUri,
            maxConnections: _options.MaxConnections,
            allowedUpdates: _updateRouter.HandlersProvider.AllowedTypes,
            dropPendingUpdates: _options.DropPendingUpdates,
            secretToken: _options.SecretToken,
            cancellationToken: cancellationToken);
    }

    private async Task ReceiveUpdate(HttpContext ctx)
    {
        if (_options.SecretToken != null)
        {
            if (!ctx.Request.Headers.TryGetValue(SecretTokenHeader, out StringValues strings))
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            string? secret = strings.SingleOrDefault();
            if (secret == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            if (_options.SecretToken != secret)
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        Update? update = await JsonSerializer.DeserializeAsync(ctx.Request.Body, JsonBotSerializerContext.Default.Update, ctx.RequestAborted).ConfigureAwait(false);
        if (update is not { Id: > 0 })
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        await _updateRouter.ConsumeUpdateAsync(_botClient, update, ctx.RequestAborted).ConfigureAwait(false);
        ctx.Response.StatusCode = StatusCodes.Status200OK;
    }
}
