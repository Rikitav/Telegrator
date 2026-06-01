#if NET10_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegrator.Hosting;

namespace Telegrator.Testing.Hosting;

/// <summary>
/// Provides extension methods for adding a Telegrator test server.
/// </summary>
public static class TestServerExtensions
{
    /// <summary>
    /// Registers Telegrator for testing by mocking the TelegramBotClient and avoiding
    /// long-polling/webhook registration. Adds <see cref="TelegratorTestServer"/> to the DI container.
    /// </summary>
    /// <param name="builder">The telegram bot host builder.</param>
    /// <param name="botUser">Optional custom user info for the mocked bot.</param>
    /// <returns>The same builder instance.</returns>
    public static ITelegramBotHostBuilder WithTestServer(this ITelegramBotHostBuilder builder, User? botUser = null)
    {
        botUser ??= new User
        {
            Id = 123456789,
            IsBot = true,
            FirstName = "TestBot",
            Username = "TestBot"
        };

        builder.Services.RemoveAll<ITelegramBotClient>();

        ITelegramBotClient mockClient = new Mock<ITelegramBotClient>();
        mockClient.Setup(x => x.BotId).Returns(botUser.Id);

        mockClient.Setup(x => x.SendRequest<User>(
                It.IsAny<Telegram.Bot.Requests.GetMeRequest>(),
                It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(botUser);

        builder.Services.AddSingleton<ITelegramBotClient>(mockClient.Object);
        builder.Services.AddSingleton<TelegratorTestServer>();

        // Remove any receivers (long-polling or webhooks) just in case
        var receiverDescriptors = builder.Services.Where(x =>
            x.ServiceType == typeof(IHostedService) &&
            x.ImplementationType != null &&
            x.ImplementationType.Name.Contains("HostedUpdate")).ToList();

        foreach (var descriptor in receiverDescriptors)
            builder.Services.Remove(descriptor);

        return builder;
    }

    /// <summary>
    /// Retrieves the registered <see cref="TelegratorTestServer"/> from the built host.
    /// </summary>
    /// <param name="host">The built host.</param>
    /// <returns>The test server instance.</returns>
    public static TelegratorTestServer GetTestServer(this IHost host)
    {
        return host.Services.GetRequiredService<TelegratorTestServer>();
    }
}
#endif
