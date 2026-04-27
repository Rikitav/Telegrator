using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Telegrator.Hosting;
using Telegrator.Hosting.Web;

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

        builder.AddTelegrator(action: builder => builder.Handlers
            .CollectHandlersAssemblyWide());

        builder.Build()
            .UseTelegrator()
            .AddLoggingAdapter()
            .SetBotCommands()
            .Run();
    }
}
