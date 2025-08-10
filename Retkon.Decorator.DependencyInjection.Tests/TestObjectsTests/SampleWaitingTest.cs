using Retkon.Decorator.DependencyInjection.Tests.TestObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjectsTests;
[TestClass]
public class SampleWaitingTest
{

    [TestMethod]
    public async Task SampleWaitingComponent_Basic()
    {
        // Arrange

        var sut = new SampleWaitingComponent();

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
        Assert.IsGreaterThanOrEqualTo(min, resultMin);
        Assert.IsLessThan(max, resultMax);
    }

}
