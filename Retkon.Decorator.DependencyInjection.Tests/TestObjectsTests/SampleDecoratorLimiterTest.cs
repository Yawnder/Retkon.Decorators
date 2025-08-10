using Retkon.Decorator.DependencyInjection.Tests.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjectsTests;
[TestClass]
public class SampleDecoratorLimiterTest
{

    [TestMethod]
    public async Task SampleDecoratorLimiter()
    {
        // Arrange

        var sampleObject = new SampleComponent();
        var sampleObjectDecoratorLimiterSettings = new SampleDecoratorLimiterSettings
        {
            MinimumMinimum = 5,
            MinimumMaximum = 6,
            MaximumMinimum = 8,
            MaximumMaximum = 9,
        };

        var sut = new SampleDecoratorLimiter(sampleObject, sampleObjectDecoratorLimiterSettings);

        var min = 10;
        var max = 20;
        var testCount = 100;
        var resultMin = int.MaxValue;
        var resultMax = int.MinValue;

        // Act

        for (int i = 0; i < testCount; i++)
        {
            var result = await sut.JustWaitPlease(min, max);
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
