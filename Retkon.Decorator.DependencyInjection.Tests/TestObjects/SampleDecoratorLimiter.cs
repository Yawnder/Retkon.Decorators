using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjects;
internal class SampleDecoratorLimiter : SampleDecorator
{
    private readonly SampleDecoratorLimiterSettings sampleObjectDecoratorLimiterSettings;

    public SampleDecoratorLimiter(
        ISampleComponent sampleObject,
        SampleDecoratorLimiterSettings sampleObjectDecoratorLimiterSettings)
        : base(sampleObject)
    {
        this.sampleObjectDecoratorLimiterSettings = sampleObjectDecoratorLimiterSettings;
    }

    protected override Task<bool> PreJustWaitPlease(ref int mininum, ref int maximum, out int result)
    {
        mininum = Math.Clamp(mininum, this.sampleObjectDecoratorLimiterSettings.MinimumMinimum, this.sampleObjectDecoratorLimiterSettings.MinimumMaximum);
        maximum = Math.Clamp(maximum, this.sampleObjectDecoratorLimiterSettings.MaximumMinimum, this.sampleObjectDecoratorLimiterSettings.MaximumMaximum);
        result = 0;

        return Task.FromResult(false);
    }

}
