using Microsoft.Extensions.DependencyInjection;
using Retkon.Decorators.DependencyInjection;
using Retkon.Decorators.DependencyInjection.Models;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Retkon.Decorators;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Decorate<TDecorator, TComponent>(this IServiceCollection serviceCollection, DecorateOptions? decorateOptions = null)
        where TDecorator : class, TComponent
    {
        decorateOptions ??= new DecorateOptions();

        return DecorateCore<TDecorator, TComponent>(serviceCollection, null, decorateOptions);
    }

    public static IServiceCollection Decorate<TDecorator, TComponent>(this IServiceCollection serviceCollection, Func<IServiceProvider, TComponent, TDecorator> factory, DecorateOptions? decorateOptions = null)
    where TDecorator : class, TComponent
    {
        decorateOptions ??= new DecorateOptions();

        return DecorateCore<TDecorator, TComponent>(serviceCollection, factory, decorateOptions);
    }

    private static IServiceCollection DecorateCore<TDecorator, TComponent>(IServiceCollection serviceCollection, Func<IServiceProvider, TComponent, TDecorator>? factory, DecorateOptions decorateOptions)
        where TDecorator : class, TComponent
    {
        for (int i = serviceCollection.Count - 1; i >= 0; i--)
        {
            var currentComponentServiceDescriptor = serviceCollection[i];

            if (currentComponentServiceDescriptor.ServiceType == typeof(TComponent))
            {
                var currentDecorationServiceDescriptor = currentComponentServiceDescriptor as DecorationServiceDescriptor;

                if (currentDecorationServiceDescriptor != null && (
                    (decorateOptions?.SkippedDecoratorTypes != null && decorateOptions.SkippedDecoratorTypes.Contains(currentDecorationServiceDescriptor.DecoratorType ?? currentDecorationServiceDescriptor.ComponentType)) ||
                    (decorateOptions?.SkippedDecoratorTypes == null && currentDecorationServiceDescriptor.ComponentType == typeof(TDecorator)) ||
                    (decorateOptions?.SkipSameDecoratorType != false && currentDecorationServiceDescriptor.DecoratorType == typeof(TDecorator))))
                    continue;

                if (decorateOptions?.DecoratedServiceKeys?.Any() == true && !decorateOptions.DecoratedServiceKeys.Contains(currentComponentServiceDescriptor.IsKeyedService ? currentComponentServiceDescriptor.ServiceKey : null))
                    continue;

                var componentServiceKey = Guid.NewGuid();
                DecorationServiceDescriptor newComponentServiceDescriptor;
                if (currentComponentServiceDescriptor.ImplementationInstance != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.ImplementationInstance.GetType(),
                        currentDecorationServiceDescriptor?.ComponentServiceKey,
                        currentComponentServiceDescriptor.ServiceType,
                        componentServiceKey,
                        currentComponentServiceDescriptor.ImplementationInstance);
                }
                else if (currentComponentServiceDescriptor.IsKeyedService && currentComponentServiceDescriptor.KeyedImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentDecorationServiceDescriptor?.ComponentType ?? typeof(TComponent),
                        currentDecorationServiceDescriptor?.ComponentServiceKey,
                        currentComponentServiceDescriptor.ServiceType,
                        componentServiceKey,
                        currentComponentServiceDescriptor.KeyedImplementationFactory,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentDecorationServiceDescriptor?.ComponentType ?? typeof(TComponent),
                        currentDecorationServiceDescriptor?.ComponentServiceKey,
                        currentComponentServiceDescriptor.ServiceType,
                        componentServiceKey,
                        (sp, key) => currentComponentServiceDescriptor.ImplementationFactory.Invoke(sp),
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.IsKeyedService && currentComponentServiceDescriptor.KeyedImplementationType != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.KeyedImplementationType,
                        currentDecorationServiceDescriptor?.ComponentServiceKey,
                        currentComponentServiceDescriptor.ServiceType,
                        componentServiceKey,
                        currentComponentServiceDescriptor.KeyedImplementationType,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationType != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.ImplementationType,
                        currentDecorationServiceDescriptor?.ComponentServiceKey,
                        currentComponentServiceDescriptor.ServiceType,
                        componentServiceKey,
                        currentComponentServiceDescriptor.ImplementationType,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else
                {
                    throw new NotImplementedException();
                }

                var decoratorServiceDescriptor = new DecorationServiceDescriptor(
                    null,
                    typeof(TDecorator),
                    componentServiceKey,
                    typeof(TComponent),
                    currentComponentServiceDescriptor.ServiceKey ?? decorateOptions?.DecoratorServiceKey,
                    (sp, key) =>
                    {
                        var component = (TComponent)sp.GetRequiredKeyedService(currentComponentServiceDescriptor.ServiceType, componentServiceKey);

                        TDecorator decorator;
                        if (factory != null)
                        {
                            decorator = factory.Invoke(sp, component);
                        }
                        else
                        {
                            decorator = ActivatorUtilities.CreateInstance<TDecorator>(sp, component);
                        }

                        return decorator;
                    },
                    currentComponentServiceDescriptor.Lifetime);


                //if (decorateOptions?.DecoratorServiceKey != null && currentComponentServiceDescriptor.ServiceKey == null)
                //{
                //    serviceCollection.Insert(i + 1, newComponentServiceDescriptor);
                //    serviceCollection.Insert(i + 2, decoratorServiceDescriptor);
                //}
                //else
                //{
                serviceCollection[i] = newComponentServiceDescriptor;
                serviceCollection.Insert(i + 1, decoratorServiceDescriptor);
                //}

            }
        }

        return serviceCollection;
    }
}
