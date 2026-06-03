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

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using Telegram.Bot;
using Telegrator.Hosting;
using Telegrator.Mediation;

namespace Telegrator;

/// <summary>
/// Provides extension methods for <see cref="IHostApplicationBuilder"/> to configure Telegrator.
/// </summary>
public static class WebHostBuilderExtensions
{
    private static WebhookerOptions ParseWebhookerOptions(IConfiguration configuration)
    {
        return new WebhookerOptions
        {
            WebhookUri = configuration["WebhookUri"],
            DropPendingUpdates = bool.TryParse(configuration["DropPendingUpdates"], out bool dropPendingUpdates) && dropPendingUpdates,
            SecretToken = configuration["SecretToken"]
        };
    }

    /// <summary>
    /// Registers Telegrator to receive updates via WebHooks.
    /// </summary>
    public static ITelegramBotHostBuilder WithWeb(this ITelegramBotHostBuilder builder)
    {
        if (builder.Services.Any(srvc => srvc.ServiceType == typeof(HostedUpdateReceiver)))
            throw new InvalidOperationException("`HostedUpdateReceiver` found in services. WebHost extension is not compatible with long-polling receiving. Please, remove `WithPolling` invocation from your WebApp configuration.");

        if (!builder.Services.Any(srvc => srvc.ServiceType == typeof(IOptions<WebhookerOptions>)))
        {
            IConfigurationSection section = builder.Configuration.GetSection(nameof(WebhookerOptions));
            if (!section.Exists())
                section = builder.Configuration.GetSection("Webhooker");

            if (!section.Exists())
                throw new MissingMemberException("Auto configuration enabled, yet no options of type 'WebhookerOptions' was registered. This configuration is runtime required!");

            WebhookerOptions webhookerOptions = ParseWebhookerOptions(section);
            builder.Services.AddSingleton(Options.Create(webhookerOptions));
        }

        builder.Services.AddTelegramWebhook();
        return builder;
    }
}

/// <summary>
/// Contains extensions for <see cref="IServiceCollection"/>
/// Provides method to configure Telegram Bot WebHost
/// </summary>
public static class WebServicesCollectionExtensions
{
    /// <summary>
    /// Adds WebhookerOptions to services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureWebhooker(this IServiceCollection services, WebhookerOptions options)
    {
        services.RemoveAll<IOptions<WebhookerOptions>>();
        services.AddSingleton(Options.Create(options));
        return services;
    }

    /// <summary>
    /// Searchs for <see cref="HostedUpdateWebhooker"/> hosted service inside hosts services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="webhooker"></param>
    /// <returns></returns>
    public static bool TryFindWebhooker(this IServiceProvider services, [NotNullWhen(true)] out HostedUpdateWebhooker? webhooker)
    {
        webhooker = services.GetServices<IHostedService>().FirstOrDefault(s => s is HostedUpdateWebhooker) as HostedUpdateWebhooker;
        return webhooker != null;
    }
}

/// <summary>
/// Provides useful methods to adjust Telegram bot Host
/// </summary>
public static class WebTelegramBotHostExtensions
{
    /// <summary>
    /// Allows to remap receiving webhook endpoint and map new route to webhost.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="webhookUri"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static async Task<T> RemapWebhookAsync<T>(this T app, string webhookUri) where T : IEndpointRouteBuilder, IHost
    {
        if (!app.ServiceProvider.TryFindWebhooker(out HostedUpdateWebhooker? webhooker))
            throw new InvalidOperationException("No service for type 'Telegrator.Mediation.HostedUpdateWebhooker' has been registered.");

        await webhooker.RemapWebhook(app, webhookUri, default).ConfigureAwait(false);
        return app;
    }

    /// <summary>
    /// Remaps the webhook endpoint to the specified URI.
    /// </summary>
    public static T RemapWebhook<T>(this T app, string webhookUri) where T : IEndpointRouteBuilder, IHost
    {
        return RemapWebhookAsync(app, webhookUri).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Registers <see cref="ITelegramBotClient"/> service with <see cref="HostedUpdateWebhooker"/> to receive updates using webhook
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddTelegramWebhook(this IServiceCollection services)
    {
        services.RemoveAll<IOptions<WebhookerOptions>>();
        services.RemoveAll<ITelegramBotClient>();

        services.AddHttpClient<ITelegramBotClient>("tgwebhook").RemoveAllLoggers().AddTypedClient(TypedTelegramBotClientFactory);
        services.AddHostedService<HostedUpdateWebhooker>();
        return services;
    }

    private static ITelegramBotClient TypedTelegramBotClientFactory(HttpClient httpClient, IServiceProvider provider)
        => new TelegramBotClient(provider.GetRequiredService<IOptions<TelegramBotClientOptions>>().Value, httpClient);
}
