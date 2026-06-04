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

        builder.AddTelegrator()
            .WithPolling()
            .Handlers.CollectHandlers();

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
            .WithWide(dbConnectionFactory: provider => new SqliteConnection($"Data Source={Environment.ExpandEnvironmentVariables("%AppData%\\Telegrator\\%wtgb.db")}"))
            .Handlers.CollectHandlers();

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

        builder.AddTelegrator()
            .WithWeb()
            .Handlers.CollectHandlers();

        WebApplication app = builder.Build();
        app.UseTelegrator();
        app.RemapWebhook("https://amazing-butt-sex.cloudpub.ru/");
        app.Run();
    }
}
