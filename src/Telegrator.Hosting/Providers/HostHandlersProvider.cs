using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegrator.Handlers.Components;
using Telegrator.MadiatorCore;
using Telegrator.MadiatorCore.Descriptors;
using Telegrator.Providers;

namespace Telegrator.Hosting.Providers
{
    /// <inheritdoc/>
    public class HostHandlersProvider : HandlersProvider
    {
        private readonly IServiceProvider Services;
        private readonly ILogger<HostHandlersProvider> Logger;

        /// <inheritdoc/>
        public HostHandlersProvider(
            IHandlersCollection handlers,
            IOptions<TelegratorOptions> options,
            IServiceProvider serviceProvider,
            ILogger<HostHandlersProvider> logger) : base(handlers, options.Value)
        {
            Services = serviceProvider;
            Logger = logger;
        }

        /// <inheritdoc/>
        public override UpdateHandlerBase GetHandlerInstance(HandlerDescriptor descriptor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IServiceScope scope = Services.CreateScope();

            object handlerInstance = descriptor.ServiceKey == null
                ? scope.ServiceProvider.GetRequiredService(descriptor.HandlerType)
                : scope.ServiceProvider.GetRequiredKeyedService(descriptor.HandlerType, descriptor.ServiceKey);

            if (handlerInstance is not UpdateHandlerBase updateHandler)
                throw new InvalidOperationException("Failed to resolve " + descriptor.HandlerType + " as UpdateHandlerBase");

            descriptor.LazyInitialization?.Invoke(updateHandler);
            updateHandler.LifetimeToken.OnLifetimeEnded += _ => scope.Dispose();
            return updateHandler;
        }
    }
}
