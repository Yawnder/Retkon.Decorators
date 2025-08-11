using Microsoft.Extensions.DependencyInjection;
using Retkon.Decorators.DependencyInjection.Tests.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection.Tests;
[TestClass]
public class ServiceCollectionExtensionsTest_OnlySpecificKeys
{
    [TestMethod]
    public async Task ServiceCollectionExtensions_SampleWaiting_Decorate()
    {
        // Arrange

        var min = 10;
        var max = 20;
        var testCount = 100;
        var resultMin = int.MaxValue;
        var resultMax = int.MinValue;

        var sampleObjectDecoratorLimiterSettings = new SampleWaitingDecoratorLimiterSettings
        {
            MinimumMinimum = 5,
            MinimumMaximum = 6,
            MaximumMinimum = 8,
            MaximumMaximum = 9,
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton(sampleObjectDecoratorLimiterSettings);

        // Act
        serviceCollection.AddKeyedScoped<ISampleWaitingComponent, SampleWaitingComponent>("MyKey");
        serviceCollection.Decorate<SampleWaitingDecoratorLimiter, ISampleWaitingComponent>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleWaitingComponent = scope.ServiceProvider.GetRequiredKeyedService<ISampleWaitingComponent>("MyKey");

        for (int i = 0; i < testCount; i++)
        {
            var result = await sampleWaitingComponent.JustWaitPlease(min, max);
            resultMin = Math.Min(result, resultMin);
            resultMax = Math.Max(result, resultMax);
        }

        // Assert
        Assert.IsGreaterThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MinimumMinimum, resultMin);
        Assert.IsLessThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MinimumMaximum, resultMin);
        Assert.IsGreaterThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MaximumMinimum - 1, resultMax);
        Assert.IsLessThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MaximumMaximum - 1, resultMax);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_Decorate()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });

        /*Note: The DI stack should be:
         * - Store
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(1, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_DecorateTwice()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });

        /*Note: The DI stack should be:
         * - Store
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(1, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_DecorateTwice_ForcedReapply()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");

        // Act
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(
            new DecorateOptions
            {
                SkipSameDecoratorType = false,
            });

        /*Note: The DI stack should be:
         * - Store
         * - Adding
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(2, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_DecorateTwice_ForcedTotal()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");

        // Act
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(
            new DecorateOptions
            {
                SkippedDecoratorTypes = [],
                SkipSameDecoratorType = false,
            });

        /*Note: The DI stack should be:
         * - Store
         * - Adding
         * - Adding
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(3, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAddingMultiply_OrderPreserved()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>();
        serviceCollection.Decorate<SampleMultiplyDecorator, ISampleStoring>();
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });

        /*Note: The DI stack should be:
         * - Store
         * - Adding
         * - Multiply
         * - Adding
         * - Adding
         * - Multiply
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(6, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAddingMultiply_AddSkipsMultiplyToo()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddKeyedScoped<ISampleStoring, SampleStoringComponent>("MyKey");
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        serviceCollection.Decorate<SampleMultiplyDecorator, ISampleStoring>(new DecorateOptions { DecoratedServiceKeys = ["MyKey"] });
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>(
            new DecorateOptions
            {
                SkippedDecoratorTypes = [
                    typeof(SampleAddingDecorator),
                    typeof(SampleMultiplyDecorator)],
                DecoratedServiceKeys = ["MyKey"],
            });

        /*Note: The DI stack should be:
         * - Store
         * - Multiply
         * - Adding
         * - Multiply
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);
        using var scope = serviceProvider.CreateScope();

        var sampleStoring = scope.ServiceProvider.GetRequiredKeyedService<ISampleStoring>("MyKey");
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(1, sampleStoring.Value);
    }

}
