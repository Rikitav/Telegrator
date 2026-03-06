using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Telegrator.Providers
{
    /// <inheritdoc/>
    public class HostAwaitingProvider(IOptions<TelegratorOptions> options, ILogger<HostAwaitingProvider> logger) : AwaitingProvider(options.Value)
    {
        private readonly ILogger<HostAwaitingProvider> _logger = logger;
    }
}
