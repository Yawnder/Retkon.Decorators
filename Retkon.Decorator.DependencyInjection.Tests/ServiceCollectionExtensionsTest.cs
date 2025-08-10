using Microsoft.Extensions.DependencyInjection;
using Retkon.Decorator.DependencyInjection.Tests.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests;
[TestClass]
public class ServiceCollectionExtensionsTest
{
    [TestMethod]
    public async Task ServiceCollectionExtensions_Decorate()
    {
        // Arrange

        var min = 10;
        var max = 20;
        var testCount = 100;
        var resultMin = int.MaxValue;
        var resultMax = int.MinValue;

        var sampleObjectDecoratorLimiterSettings = new SampleDecoratorLimiterSettings
        {
            MinimumMinimum = 5,
            MinimumMaximum = 6,
            MaximumMinimum = 8,
            MaximumMaximum = 9,
        };

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<ISampleComponent, SampleComponent>();
        serviceCollection.AddSingleton(sampleObjectDecoratorLimiterSettings);

        // Act
        serviceCollection.Decorate<SampleDecoratorLimiter, ISampleComponent>();
        var serviceProvider = serviceCollection.BuildServiceProvider(true);

        var sampleObject = serviceProvider.GetRequiredService<ISampleComponent>();

        for (int i = 0; i < testCount; i++)
        {
            var result = await sampleObject.JustWaitPlease(min, max);
            resultMin = Math.Min(result, resultMin);
            resultMax = Math.Max(result, resultMax);
        }

        // Assert
        Assert.IsGreaterThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MinimumMinimum, resultMin);
        Assert.IsLessThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MinimumMaximum, resultMin);
        Assert.IsGreaterThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MaximumMinimum - 1, resultMax);
        Assert.IsLessThanOrEqualTo(sampleObjectDecoratorLimiterSettings.MaximumMaximum - 1, resultMax);
    }
}
