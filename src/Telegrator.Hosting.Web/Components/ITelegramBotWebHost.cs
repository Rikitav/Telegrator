using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Telegrator.Hosting.Components;

namespace Telegrator.Hosting.Web.Components
{
    /// <summary>
    /// Interface for Telegram bot hosts with Webhook update receiving.
    /// Combines wbe application capabilities with reactive Telegram bot functionality.
    /// </summary>
    public interface ITelegramBotWebHost : ITelegramBotHost, IEndpointRouteBuilder, IApplicationBuilder, IAsyncDisposable
    {

    }
}
