/*
 * Copyright (c) 2026 Rikitav Tim4ik
 */

using Microsoft.Extensions.Hosting;
using Telegrator;
using Telegrator.Hosting;

namespace Telegrator.Examples;

public class StateBot
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        ITelegramBotHostBuilder tg = builder.AddTelegrator();
        tg.Handlers.CollectHandlers();
        tg.WithPolling();

        IHost host = builder.Build();
        host.UseTelegrator();
        host.Run();
    }
}
