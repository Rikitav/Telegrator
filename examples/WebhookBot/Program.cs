/*
 * Copyright (c) 2026 Rikitav Tim4ik
 */

using Microsoft.AspNetCore.Builder;
using Telegrator;
using Telegrator.Hosting;

namespace Telegrator.Examples;

public class WebhookBot
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.AddTelegrator()
            .WithWeb()
            .Handlers.CollectHandlers();

        WebApplication app = builder.Build();
        app.UseTelegrator();
        app.Run();
    }
}
