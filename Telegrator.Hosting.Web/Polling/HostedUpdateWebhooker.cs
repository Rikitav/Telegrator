﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Hosting.Web.Components;
using Telegrator.MadiatorCore;

namespace Telegrator.Hosting.Web.Polling
{
    /// <summary>
    /// Service for receiving updates for Hosted telegram bots via Webhooks
    /// </summary>
    public class HostedUpdateWebhooker : IHostedService
    {
        private const string SecretTokenHeader = "X-Telegram-Bot-Api-Secret-Token";

        private readonly ITelegramBotWebHost _botHost;
        private readonly ITelegramBotClient _botClient;
        private readonly IUpdateRouter _updateRouter;
        private readonly TelegratorWebOptions _options;

        /// <summary>
        /// Initiallizes new instance of <see cref="HostedUpdateWebhooker"/>
        /// </summary>
        /// <param name="botHost"></param>
        /// <param name="botClient"></param>
        /// <param name="updateRouter"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public HostedUpdateWebhooker(ITelegramBotWebHost botHost, ITelegramBotClient botClient, IUpdateRouter updateRouter, IOptions<TelegratorWebOptions> options)
        {
            if (string.IsNullOrEmpty(options.Value.WebhookUri))
                throw new ArgumentNullException(nameof(options), "Option \"WebhookUrl\" must be set to subscribe for update recieving");

            _botHost = botHost;
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
            string pattern = new UriBuilder(_options.WebhookUri).Path;
            _botHost.MapPost(pattern, (Delegate)ReceiveUpdate);

            await _botClient.SetWebhook(
                url: _options.WebhookUri,
                maxConnections: _options.MaxConnections,
                allowedUpdates: _botHost.UpdateRouter.HandlersProvider.AllowedTypes,
                dropPendingUpdates: _options.DropPendingUpdates,
                cancellationToken: cancellationToken);
        }

        /// <inheritdoc/>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _botClient.DeleteWebhook(_options.DropPendingUpdates, cancellationToken);
            return Task.CompletedTask;
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
}
