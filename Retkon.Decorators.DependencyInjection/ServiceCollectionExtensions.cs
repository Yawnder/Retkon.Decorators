using Microsoft.Extensions.DependencyInjection;
using Retkon.Decorators.DependencyInjection;
using Retkon.Decorators.DependencyInjection.Models;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Retkon.Decorators;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    //TODO: Parameters to single Option Parameter.
    public static IServiceCollection Decorate<TDecorator, TComponent>(this IServiceCollection serviceCollection, DecorateOptions? decorateOptions = null)
        where TDecorator : class, TComponent
    {
        //TODO: If nothing decorated, throw.

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

                var decoratingKey = Guid.NewGuid();
                DecorationServiceDescriptor newComponentServiceDescriptor;
                if (currentComponentServiceDescriptor.ImplementationInstance != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.ImplementationInstance.GetType(),
                        currentComponentServiceDescriptor.ServiceType,
                        decoratingKey,
                        currentComponentServiceDescriptor.ImplementationInstance);
                }
                else if (currentComponentServiceDescriptor.IsKeyedService && currentComponentServiceDescriptor.KeyedImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentDecorationServiceDescriptor?.ComponentType ?? typeof(TComponent),
                        currentComponentServiceDescriptor.ServiceType,
                        decoratingKey,
                        currentComponentServiceDescriptor.KeyedImplementationFactory,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentDecorationServiceDescriptor?.ComponentType ?? typeof(TComponent),
                        currentComponentServiceDescriptor.ServiceType,
                        decoratingKey,
                        (sp, key) => currentComponentServiceDescriptor.ImplementationFactory.Invoke(sp),
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.IsKeyedService && currentComponentServiceDescriptor.KeyedImplementationType != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.KeyedImplementationType,
                        currentComponentServiceDescriptor.ServiceType,
                        decoratingKey,
                        currentComponentServiceDescriptor.KeyedImplementationType,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationType != null)
                {
                    newComponentServiceDescriptor = new DecorationServiceDescriptor(
                        typeof(TDecorator),
                        currentComponentServiceDescriptor.ImplementationType,
                        currentComponentServiceDescriptor.ServiceType, decoratingKey,
                        currentComponentServiceDescriptor.ImplementationType,
                        currentComponentServiceDescriptor.Lifetime);
                }
                else
                {
                    throw new NotImplementedException();
                }

                serviceCollection[i] = newComponentServiceDescriptor;

                var decoratorServiceDescriptor = new DecorationServiceDescriptor(
                    null,
                    typeof(TDecorator),
                    currentComponentServiceDescriptor.ServiceType,
                    currentComponentServiceDescriptor.ServiceKey,
                    (sp, key) =>
                    {
                        var component = (TComponent)sp.GetRequiredKeyedService(currentComponentServiceDescriptor.ServiceType, decoratingKey);
                        var decorator = ActivatorUtilities.CreateInstance<TDecorator>(sp, component);
                        return decorator;
                    }, currentComponentServiceDescriptor.Lifetime);

                serviceCollection.Insert(i + 1, decoratorServiceDescriptor);

                //TODO: Cases to handle
                //serviceCollection.AddScoped<IDisposable>();
                //serviceCollection.AddScoped<IDisposable>(new Func<IServiceProvider, IDisposable>(sp=>throw new NotImplementedException()));
                //serviceCollection.AddScoped<IDisposable, IDisposable>();
                //serviceCollection.AddScoped<IDisposable, IDisposable>(new Func<IServiceProvider, IDisposable>(sp => throw new NotImplementedException()));

                //serviceCollection.Insert(i+1, new ServiceDescriptor())
                //break;
            }
        }

        return serviceCollection;
    }
}
