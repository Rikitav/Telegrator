using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegrator.Core.Descriptors;
using Telegrator.Logging;

namespace Telegrator.Tests;

internal static class Program
{
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

        builder.AddTelegrator(action: builder => builder.Handlers
            .CollectHandlersAssemblyWide());

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

        builder.AddWideTelegrator(
            dbConnectionFactory: provider => new SqliteConnection($"Data Source={Environment.ExpandEnvironmentVariables("%AppData%\\Telegrator\\%wtgb.db")}"),
            action: builder => builder.Handlers.CollectHandlersAssemblyWide());

        builder.Build()
            .UseWideTelegrator()
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

        builder.AddTelegratorWeb(action: builder => builder.Handlers
            .CollectHandlersAssemblyWide());
        
        builder.Build()
            .UseTelegratorWeb(dontMap: true)
            .RemapWebhook("https://amazing-butt-sex.cloudpub.ru/")
            .Run();
    }
}
