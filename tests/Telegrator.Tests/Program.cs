using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using System.Data.Common;

namespace Telegrator;

internal static class Program
{
    public static void WebApplicationBuilder_Example(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ApplicationName = "WebApplication example",
        });

        builder.AddTelegratorWeb(action: builder => builder.Handlers
            .CollectHandlersAssemblyWide());
        
        builder.Build()
            .UseTelegratorWeb(dontMap: true)
            .RemapWebhook("https://amazing-butt-sex.cloudpub.ru/")
            .Run();
    }

    public static void HostApplicationBuilder_Example(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            Args = args,
            ApplicationName = "Host example",
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

        using DbConnection connection = new SqliteConnection(@"Data Source=wtgb.db");
        builder.Services.ConfigureWideTelegram(new Telegram.Bot.WTelegramBotClientOptions(token: "BOT_TOKEN", apiId: 123, apiHash: "API_HASH", dbConnection: connection));

        builder.AddWideTelegrator(action: builder => builder.Handlers
            .CollectHandlersAssemblyWide());

        builder.Build()
            .UseWideTelegrator()
            .Run();
    }
}
