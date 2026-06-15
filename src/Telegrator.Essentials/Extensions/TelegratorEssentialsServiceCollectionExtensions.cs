using Microsoft.Extensions.DependencyInjection;

namespace Telegrator.Extensions;

/// <summary>
/// Provides extension methods for registering Telegrator.Essentials services.
/// </summary>
public static class TelegratorEssentialsServiceCollectionExtensions
{
    /// <summary>
    /// Registers Telegrator.Essentials supporting services.
    /// Current components (filters, pre-processors, post-processors) are attribute-driven
    /// and do not require explicit DI registration, but this method is provided for
    /// future services such as persistent rate-limit stores or scheduled cleanup workers.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddTelegratorEssentials(this IServiceCollection services)
    {
        if (services is null)
            throw new ArgumentNullException(nameof(services));

        return services;
    }
}
