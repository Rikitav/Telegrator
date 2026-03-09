using Microsoft.Extensions.Options;

namespace Telegrator.Providers;

/// <inheritdoc/>
public class HostAwaitingProvider(IOptions<TelegratorOptions> options) : AwaitingProvider(options.Value)
{

}
