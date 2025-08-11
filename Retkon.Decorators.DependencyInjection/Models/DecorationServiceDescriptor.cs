using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection.Models;
[DebuggerDisplay("{DebuggerToString(),nq}")]
internal class DecorationServiceDescriptor : ServiceDescriptor
{

    public Type? DecoratorType { get; init; }
    public Type ComponentType { get; init; }
    public Guid? ComponentServiceKey { get; init; }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        object instance)
        : base(serviceType, instance)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        ServiceLifetime lifetime)
        : base(serviceType, implementationType, lifetime)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        object? serviceKey,
        object instance)
        : base(serviceType, serviceKey, instance)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        Func<IServiceProvider, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, factory, lifetime)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        object? serviceKey,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
        ServiceLifetime lifetime)
        : base(serviceType, serviceKey, implementationType, lifetime)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    public DecorationServiceDescriptor(
        Type? decoratorType,
        Type componentType,
        Guid? componentServiceKey,
        Type serviceType,
        object? serviceKey,
        Func<IServiceProvider, object?, object> factory,
        ServiceLifetime lifetime)
        : base(serviceType, serviceKey, factory, lifetime)
    {
        this.DecoratorType = decoratorType;
        this.ComponentType = componentType;
        this.ComponentServiceKey = componentServiceKey;
    }

    private string DebuggerToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.ComponentType.Name);
        sb.Append(" (");

        if (this.IsKeyedService)
        {
            sb.Append(this.ServiceKey);
        }
        else
        {
            sb.Append("[No Key]");
        }

        if (this.ComponentServiceKey != null)
        {
            sb.Append("<-");
            sb.Append(this.ComponentServiceKey);
        }

        sb.Append(")");

        return sb.ToString();
    }

}
