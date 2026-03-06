using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Telegrator.Hosting.Configuration
{
    /// <summary>
    /// Abstract base class for configuring options from configuration sources.
    /// Provides a proxy pattern for binding configuration to strongly-typed options classes.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    public abstract class ConfigureOptionsProxy<TOptions> where TOptions : class
    {
        /// <summary>
        /// Configures the options using the default configuration section.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="configuration">The configuration source.</param>
        public void Configure(IServiceCollection services, IConfiguration configuration)
            => Configure(services, Options.DefaultName, configuration, null);

        /// <summary>
        /// Configures the options using a named configuration section.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="name">The name of the configuration section.</param>
        /// <param name="configuration">The configuration source.</param>
        public void Configure(IServiceCollection services, string? name, IConfiguration configuration)
            => Configure(services, name, configuration, null);

        /// <summary>
        /// Configures the options using a named configuration section with custom binder options.
        /// </summary>
        /// <param name="services">The service collection to configure.</param>
        /// <param name="name">The name of the configuration section.</param>
        /// <param name="configuration">The configuration source.</param>
        /// <param name="configureBinder">Optional action to configure the binder options.</param>
        public void Configure(IServiceCollection services, string? name, IConfiguration configuration, Action<BinderOptions>? configureBinder)
        {
            var namedConfigure = new NamedConfigureFromConfigurationOptions<ConfigureOptionsProxy<TOptions>>(name, configuration, configureBinder);
            namedConfigure.Configure(name, this);

            services.AddOptions();
            services.AddSingleton(Options.Create(Realize()));
        }

        /// <summary>
        /// Creates the actual options instance from the configuration.
        /// </summary>
        /// <returns>The configured options instance.</returns>
        protected abstract TOptions Realize();
    }
}
