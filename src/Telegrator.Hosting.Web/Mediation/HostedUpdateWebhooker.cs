using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Core;
using Telegrator.Hosting;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted telegram bots via Webhooks and queuing them to router
/// </summary>
public class HostedUpdateWebhooker : IHostedService
{
    private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateRouter _updateRouter;
    private readonly WebhookerOptions _options;

    /// <summary>
    /// Initiallizes new instance of <see cref="HostedUpdateWebhooker"/>
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="updateRouter"></param>
    /// <param name="options"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public HostedUpdateWebhooker(ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<WebhookerOptions> options)
    {
        if (string.IsNullOrEmpty(options.Value.WebhookUri))
            throw new ArgumentNullException(nameof(options), "Option \"WebhookUrl\" must be set to subscribe for update recieving");

        _botClient = botClient;
        _updateRouter = updateRouter;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_updateRouter.BotInfo is HostedTelegramBotInfo hostedInfo)
            hostedInfo.User = await _botClient.GetMe(cancellationToken).ConfigureAwait(false);

        await StartInternal(cancellationToken).ConfigureAwait(false);
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
        if (!Uri.TryCreate(webhookUri, UriKind.Absolute, out Uri? result))
            throw new ArgumentException("invalid URL");

        _options.WebhookUri = result.ToString();
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

        await _updateRouter.HandleUpdateAsync(_botClient, update, ctx.RequestAborted).ConfigureAwait(false);
        ctx.Response.StatusCode = StatusCodes.Status200OK;
    }
}
