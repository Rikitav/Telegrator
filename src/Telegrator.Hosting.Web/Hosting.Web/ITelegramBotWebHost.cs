using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Telegrator.Hosting.Web
{
    /// <summary>
    /// Interface for Telegram bot hosts with Webhook update receiving.
    /// Combines wbe application capabilities with reactive Telegram bot functionality.
    /// </summary>
    public interface ITelegramBotWebHost : ITelegramBotHost, IEndpointRouteBuilder, IApplicationBuilder, IAsyncDisposable
    {

    }
}
