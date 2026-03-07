using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Telegrator.Hosting;
using Telegrator.Hosting.Web;

namespace Telegrator;

internal class Program
{
    public static void TelegramBotWebHostBuilder_Example(string[] args)
    {
        TelegramBotWebHostBuilder builder = TelegramBotWebHost.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ApplicationName = "TelegramBotWebHost example",
        });

        builder.Handlers.CollectHandlersAssemblyWide();

        builder.Build()
            .AddLoggingAdapter()
            .SetBotCommands()
            .Run();
    }

    public static void WebApplicationBuilder_Example(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            Args = args,
            ApplicationName = "WebApplication example",
        });

        builder.AddTelegratorWeb();
        builder.Handlers.CollectHandlersAssemblyWide();

        builder.Build()
            .UseTelegratorWeb()
            .AddLoggingAdapter()
            .SetBotCommands()
            .Run();
    }

    public static void TelegramBotHostBuilder_Example(string[] args)
    {
        TelegramBotHostBuilder builder = TelegramBotHost.CreateBuilder(new HostApplicationBuilderSettings()
        {
            Args = args,
            ApplicationName = "TelegramBotHost example",
        });

        builder.Handlers.CollectHandlersAssemblyWide();

        builder.Build()
            .AddLoggingAdapter()
            .SetBotCommands()
            .Run();
    }

    public static void HostApplicationBuilder_Example(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings()
        {
            Args = args,
            ApplicationName = "Host example",
        });

        builder.AddTelegrator();
        builder.Handlers.CollectHandlersAssemblyWide();

        builder.Build()
            .UseTelegrator()
            .AddLoggingAdapter()
            .SetBotCommands()
            .Run();
    }
}
