using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegrator.MadiatorCore;

namespace Telegrator.Hosting.Components
{
    /// <summary>
    /// Interface for building Telegram bot hosts with dependency injection support.
    /// Combines host application building capabilities with handler collection functionality.
    /// </summary>
    public interface ITelegramBotHostBuilder : ICollectingProvider
    {
        /// <summary>
        /// Gets the set of key/value configuration properties.
        /// </summary>
        IConfigurationManager Configuration { get; }

        /// <summary>
        /// Gets a collection of logging providers for the application to compose. This is useful for adding new logging providers.
        /// </summary>
        ILoggingBuilder Logging { get; }

        /// <summary>
        /// Gets a collection of services for the application to compose. This is useful for adding user provided or framework provided services.
        /// </summary>
        IServiceCollection Services { get; }
    }
}
