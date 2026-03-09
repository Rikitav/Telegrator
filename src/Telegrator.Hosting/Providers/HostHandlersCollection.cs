using Microsoft.Extensions.DependencyInjection;
using Telegrator.Core;
using Telegrator.Core.Descriptors;

namespace Telegrator.Providers;

/// <inheritdoc/>
public class HostHandlersCollection(IServiceCollection hostServiceColletion, TelegratorOptions options) : HandlersCollection(options)
{
    private readonly IServiceCollection Services = hostServiceColletion;

    /// <inheritdoc/>
    protected override bool MustHaveParameterlessCtor => false;

    /// <inheritdoc/>
    public override IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
    {
        switch (descriptor.Type)
        {
            case DescriptorType.General:
                {
                    if (descriptor.InstanceFactory != null)
                        Services.AddScoped(descriptor.HandlerType, _ => descriptor.InstanceFactory.Invoke());
                    else
                        Services.AddScoped(descriptor.HandlerType);

                    break;
                }

            case DescriptorType.Keyed:
                {
                    if (descriptor.InstanceFactory != null)
                        Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey, (_, _) => descriptor.InstanceFactory.Invoke());
                    else
                        Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey);

                    break;
                }

            case DescriptorType.Singleton:
                {
                    Services.AddSingleton(descriptor.HandlerType, descriptor.SingletonInstance ?? (descriptor.InstanceFactory != null
                        ? descriptor.InstanceFactory.Invoke()
                        : throw new Exception()));

                    break;
                }

            case DescriptorType.Implicit:
                {
                    Services.AddKeyedSingleton(descriptor.HandlerType, descriptor.ServiceKey, descriptor.SingletonInstance ?? (descriptor.InstanceFactory != null
                        ? descriptor.InstanceFactory.Invoke()
                        : throw new Exception()));
                    
                    break;
                }
        }

        return base.AddDescriptor(descriptor);
    }
}
