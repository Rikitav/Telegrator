using Microsoft.Extensions.Options;
using Telegrator.Core;
using Telegrator.Mediation;

namespace Telegrator.Polling
{
    /// <inheritdoc/>
    public class HostUpdateHandlersPool(IUpdateRouter router, IOptions<TelegratorOptions> options)
        : UpdateHandlersPool(router, options.Value, options.Value.GlobalCancellationToken)
    {

    }
}
