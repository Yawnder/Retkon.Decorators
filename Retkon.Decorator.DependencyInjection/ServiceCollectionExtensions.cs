using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Retkon.Decorator;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Decorate<TDecorator, TComponent>(this IServiceCollection serviceCollection)
        where TDecorator : class, TComponent
    {
        //TODO: If nothing decorated, throw.

        for (int i = serviceCollection.Count - 1; i >= 0; i--)
        {
            var currentComponentServiceDescriptor = serviceCollection[i];

            if (currentComponentServiceDescriptor.ServiceType == typeof(TComponent))
            {
                var decoratingKey = Guid.NewGuid();
                ServiceDescriptor newComponentServiceDescriptor;
                if (currentComponentServiceDescriptor.ImplementationInstance != null)
                {
                    newComponentServiceDescriptor = new ServiceDescriptor(currentComponentServiceDescriptor.ServiceType, decoratingKey, currentComponentServiceDescriptor.ImplementationInstance);
                }
                else if (currentComponentServiceDescriptor.IsKeyedService && currentComponentServiceDescriptor.KeyedImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new ServiceDescriptor(currentComponentServiceDescriptor.ServiceType, decoratingKey, currentComponentServiceDescriptor.KeyedImplementationFactory, currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationFactory != null)
                {
                    newComponentServiceDescriptor = new ServiceDescriptor(currentComponentServiceDescriptor.ServiceType, decoratingKey, (sp, key) => currentComponentServiceDescriptor.ImplementationFactory.Invoke(sp), currentComponentServiceDescriptor.Lifetime);
                }
                else if (currentComponentServiceDescriptor.ImplementationType != null)
                {
                    newComponentServiceDescriptor = new ServiceDescriptor(currentComponentServiceDescriptor.ServiceType, decoratingKey, currentComponentServiceDescriptor.ImplementationType, currentComponentServiceDescriptor.Lifetime);
                }
                else
                {
                    throw new NotImplementedException();
                }

                serviceCollection[i] = newComponentServiceDescriptor;

                var decoratorServiceDescriptor = new ServiceDescriptor(
                    currentComponentServiceDescriptor.ServiceType,
                    currentComponentServiceDescriptor.ServiceKey,
                    (sp, key) =>
                    {
                        var component = (TComponent)sp.GetRequiredKeyedService(currentComponentServiceDescriptor.ServiceType, decoratingKey);
                        var decorator = ActivatorUtilities.CreateInstance<TDecorator>(sp, component);
                        return decorator;
                    }, currentComponentServiceDescriptor.Lifetime);

                serviceCollection.Add(decoratorServiceDescriptor);

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
