/*
 * Copyright (c) 2026 Rikitav Tim4ik
 * * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Telegrator.Core;
using Telegrator.Core.Descriptors;

namespace Telegrator.Providers;

/// <inheritdoc/>
public class HostHandlersCollection(IServiceCollection hostServiceColletion, TelegratorOptions options) : HandlersCollection(options)
{
    private readonly IServiceCollection Services = hostServiceColletion;

    /// <inheritdoc/>
    public override HandlerDescriptor CreateClassDescriptor(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods)] Type handlerType,
        Attribute[]? precompiledAttributes = null)
    {
        return new HostClassHandlerDescriptor(DescriptorType.General, handlerType, dontInspect: false, precompiledAttributes);
    }

    /// <inheritdoc/>
    public override IHandlersCollection AddDescriptor(HandlerDescriptor descriptor)
    {
        switch (descriptor.Type)
        {
            default:
                throw new InvalidOperationException("Unknown descriptor type");

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
