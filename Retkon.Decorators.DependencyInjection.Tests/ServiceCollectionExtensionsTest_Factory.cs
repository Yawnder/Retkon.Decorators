using Microsoft.Extensions.DependencyInjection;
using Retkon.Decorators.DependencyInjection.Tests.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection.Tests;
[TestClass]
public class ServiceCollectionExtensionsTest_Factory
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

        // Act
        serviceCollection.AddTransient<ISampleWaitingComponent, SampleWaitingComponent>();
        serviceCollection.Decorate<SampleWaitingDecoratorLimiter, ISampleWaitingComponent>((sp, c) => new SampleWaitingDecoratorLimiter(c, sampleObjectDecoratorLimiterSettings));
        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var sampleWaitingComponent = serviceProvider.GetRequiredService<ISampleWaitingComponent>();

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
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));

        /*Note: The DI stack should be:
         * - Store
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
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
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));

        /*Note: The DI stack should be:
         * - Store
         * - Adding
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(1, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_DecorateTwice_ForcedReapply()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();

        // Act
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c),
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

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(2, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAdding_DecorateTwice_ForcedTotal()
    {
        // Arrange

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();

        // Act
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c),
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

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
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
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));
        serviceCollection.Decorate<SampleMultiplyDecorator, ISampleStoring>((_, c) => new SampleMultiplyDecorator(c));
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));

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

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(9, sampleStoring.Value);
    }

    [TestMethod]
    public void ServiceCollectionExtensions_SampleAddingMultiply_AddSkipsMultiplyToo()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();

        // Act
        serviceCollection.AddTransient<ISampleStoring, SampleStoringComponent>();
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c));
        serviceCollection.Decorate<SampleMultiplyDecorator, ISampleStoring>((_, c) => new SampleMultiplyDecorator(c));
        serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>((_, c) => new SampleAddingDecorator(c),
            new DecorateOptions
            {
                SkippedDecoratorTypes = [
                    typeof(SampleAddingDecorator),
                    typeof(SampleMultiplyDecorator)],
            });

        /*Note: The DI stack should be:
         * - Store
         * - Multiply
         * - Adding
         * - Multiply
        */

        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();
        sampleStoring.Store(0);

        // Assert
        Assert.AreEqual(2, sampleStoring.Value);
    }

}
