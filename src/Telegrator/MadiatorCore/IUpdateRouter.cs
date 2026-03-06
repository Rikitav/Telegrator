using Telegram.Bot.Polling;
using Telegrator.Handlers.Components;

namespace Telegrator.MadiatorCore
{
    /// <summary>
    /// Interface for update routers that handle incoming updates and manage handler execution.
    /// Combines update handling capabilities with polling provider functionality and exception handling.
    /// </summary>
    public interface IUpdateRouter : IUpdateHandler, IPollingProvider
    {   
        /// <summary>
        /// Gets the <see cref="TelegratorOptions"/> for the router.
        /// </summary>
        public TelegratorOptions Options { get; }
        
        /// <summary>
        /// Gets the <see cref="IUpdateHandlersPool"/> that manages handler execution.
        /// </summary>
        public IUpdateHandlersPool HandlersPool { get; }

        /// <summary>
        /// Gets or sets the <see cref="IRouterExceptionHandler"/> for handling exceptions.
        /// </summary>
        public IRouterExceptionHandler? ExceptionHandler { get; set; }

        /// <summary>
        /// Default hand;er container factory
        /// </summary>
        public IHandlerContainerFactory? DefaultContainerFactory { get; set; }
    }
}
