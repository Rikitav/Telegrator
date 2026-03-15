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
using Telegrator.Hosting.Web;

namespace Telegrator.Mediation;

/// <summary>
/// Service for receiving updates for Hosted telegram bots via Webhooks
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
    /// <param name="botHost"></param>
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
    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartInternal(cancellationToken);
        return Task.CompletedTask;
    }

    private async void StartInternal(CancellationToken cancellationToken)
    {
        await _botClient.SetWebhook(
            url: _options.WebhookUri,
            maxConnections: _options.MaxConnections,
            allowedUpdates: _updateRouter.HandlersProvider.AllowedTypes,
            dropPendingUpdates: _options.DropPendingUpdates,
            secretToken: _options.SecretToken,
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _botClient.DeleteWebhook(_options.DropPendingUpdates, cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Maps bot webhook to application builder
    /// </summary>
    /// <param name="routeBuilder"></param>
    public void MapWebhook(IEndpointRouteBuilder routeBuilder)
    {
        string pattern = new UriBuilder(_options.WebhookUri).Path;
        routeBuilder.MapPost(pattern, (Delegate)ReceiveUpdate);
    }

    private async Task<IResult> ReceiveUpdate(HttpContext ctx)
    {
        if (_options.SecretToken != null)
        {
            if (!ctx.Request.Headers.TryGetValue(SecretTokenHeader, out StringValues strings))
                return Results.BadRequest();

            string? secret = strings.SingleOrDefault();
            if (secret == null)
                return Results.BadRequest();

            if (_options.SecretToken != secret)
                return Results.StatusCode(401);
        }

        Update? update = await JsonSerializer.DeserializeAsync<Update>(ctx.Request.Body, JsonBotAPI.Options, ctx.RequestAborted);
        if (update is not { Id: > 0 })
            return Results.BadRequest();

        await _updateRouter.HandleUpdateAsync(_botClient, update, ctx.RequestAborted);
        return Results.Ok();
    }
}
