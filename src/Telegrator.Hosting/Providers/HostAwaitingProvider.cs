using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    /// <inheritdoc/>
    public class HostAwaitingProvider(IOptions<TelegratorOptions> options, ILogger<HostAwaitingProvider> logger) : AwaitingProvider(options.Value)
    {
        private readonly ILogger<HostAwaitingProvider> _logger = logger;
    }
}
