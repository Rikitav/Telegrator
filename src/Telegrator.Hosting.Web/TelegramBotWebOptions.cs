using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Options for configuring the behavior for TelegramBotWebHost.
    /// </summary>
    public class TelegramBotWebOptions : TelegratorOptions
    {
        /// <summary>
        /// Disables automatic configuration for all of required <see cref="IOptions{TOptions}"/> instances
        /// </summary>
        public bool DisableAutoConfigure { get; set; }

        /// <inheritdoc cref="WebApplicationOptions.Args"/>
        public string[]? Args { get; init; }

        /// <inheritdoc cref="WebApplicationOptions.EnvironmentName"/>
        public string? EnvironmentName { get; init; }

        /// <inheritdoc cref="WebApplicationOptions.ApplicationName"/>
        public string? ApplicationName { get; init; }

        /// <inheritdoc cref="WebApplicationOptions.ContentRootPath"/>
        public string? ContentRootPath { get; init; }

        /// <inheritdoc cref="WebApplicationOptions.WebRootPath"/>
        public string? WebRootPath { get; init; }

        internal WebApplicationOptions ToWebApplicationOptions() => new WebApplicationOptions()
        {
            ApplicationName = ApplicationName,
            Args = Args,
            ContentRootPath = ContentRootPath,
            EnvironmentName = EnvironmentName,
            WebRootPath = WebRootPath
        };
    }
}
