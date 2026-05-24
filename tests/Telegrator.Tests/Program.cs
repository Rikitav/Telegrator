using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegrator.Handlers;
using Telegrator.Logging;

namespace Telegrator.Tests;

internal static class Program
{
    [AnyUpdateHandler(Priority = 100)]
    public sealed class DummyUpdateHandler : AnyUpdateHandler
    {
        public override Task<Result> Execute(IHandlerContainer<Update> container, CancellationToken cancellation)
            => throw new NotImplementedException();
    }

    public static void HostApplicationBuilder_Example(string[] args)
    {
        TelegratorLogging.MinimalLevel = Telegrator.Logging.LogLevel.Trace;

        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            Args = args,
            ApplicationName = "Host example",
        });

        builder.Services.ConfigureReceiver(new ReceiverOptions()
        {
            DropPendingUpdates = true,
            Limit = 100
        });

        builder.AddTelegrator().WithPolling();

        builder.Build()
            .UseTelegrator()
            .Run();
    }

    public static void WideBotApplicationBuilder_Example(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            Args = args,
            ApplicationName = "WBot example",
        });

        builder.Services.ConfigureWideBot(new WideBotOptions()
        {
            ApiId = 123,
            ApiHash = "API_HASH",
            DropPendingUpdates = true,
        });

        builder.AddTelegrator()
            .WithWide(
                dbConnectionFactory: provider => new SqliteConnection($"Data Source={Environment.ExpandEnvironmentVariables("%AppData%\\Telegrator\\%wtgb.db")}"));

        builder.Build()
            .UseTelegrator()
            .Run();
    }

    public static void WebApplicationBuilder_Example(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ApplicationName = "WebApplication example",
        });

        builder.Services.ConfigureWebhooker(new WebhookerOptions()
        {
            WebhookUri = "https://medic-gaming.com/",
            DropPendingUpdates = true,
            SecretToken = "MEDIC_GAMING"
        });

        builder.AddTelegrator().WithWeb();

        var app = builder.Build();
        app.UseTelegrator();
        app.RemapWebhook("https://amazing-butt-sex.cloudpub.ru/");
        app.Run();
    }
}
