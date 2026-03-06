using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.MadiatorCore;
using Telegrator.Polling;

namespace Telegrator.Hosting.Polling
{
    /// <inheritdoc/>
    public class HostUpdateHandlersPool(IUpdateRouter router, IOptions<TelegratorOptions> options)
        : UpdateHandlersPool(router, options.Value, options.Value.GlobalCancellationToken)
    {

    }
}
