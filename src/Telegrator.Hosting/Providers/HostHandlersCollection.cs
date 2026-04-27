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
            default:
                throw new Exception("Unknown descriptor type");

            case DescriptorType.General:
                {
                    if (descriptor.InstanceFactory != null)
                    {
                        Services.AddScoped(descriptor.HandlerType, _ => descriptor.InstanceFactory.Invoke());
                        break;
                    }

                    Services.AddScoped(descriptor.HandlerType);
                    break;
                }

            case DescriptorType.Keyed:
                {
                    if (descriptor.InstanceFactory != null)
                    {
                        Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey, (_, _) => descriptor.InstanceFactory.Invoke());
                        break;
                    }

                    Services.AddKeyedScoped(descriptor.HandlerType, descriptor.ServiceKey);
                    break;
                }

            case DescriptorType.Singleton:
                {
                    if (descriptor.SingletonInstance != null)
                    {
                        Services.AddSingleton(descriptor.HandlerType, descriptor.SingletonInstance);
                        break;
                    }

                    if (descriptor.InstanceFactory == null)
                        throw new InvalidOperationException("Singleton handler descriptor without singleton instance should implement `InstanceFactory`");

                    Services.AddSingleton(descriptor.HandlerType, descriptor.InstanceFactory.Invoke());
                    break;
                }

            case DescriptorType.Implicit:
                {
                    if (descriptor.SingletonInstance != null)
                    {
                        Services.AddKeyedSingleton(descriptor.HandlerType, descriptor.ServiceKey, descriptor.SingletonInstance);
                        break;
                    }

                    if (descriptor.InstanceFactory == null)
                        throw new InvalidOperationException("Implicit handler descriptor without singleton instance should implement `InstanceFactory`");

                    Services.AddKeyedSingleton(descriptor.HandlerType, descriptor.ServiceKey, descriptor.InstanceFactory.Invoke());
                    break;
                }
        }

        return base.AddDescriptor(descriptor);
    }
}
