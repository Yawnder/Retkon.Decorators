using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjects;
internal abstract class SampleDecorator : ISampleComponent
{
    private readonly ISampleComponent sampleObject;

    public SampleDecorator(
        ISampleComponent sampleObject)
    {
        this.sampleObject = sampleObject;
    }

    public async Task<int> JustWaitPlease(int mininum, int maximum)
    {
        if (await this.PreJustWaitPlease(ref mininum, ref maximum, out var result))
            return result;

        result = await this.sampleObject.JustWaitPlease(mininum, maximum);
        await this.PostJustWaitPlease(mininum, maximum, ref result);
        return result;
    }

    protected virtual Task<bool> PreJustWaitPlease(ref int mininum, ref int maximum, out int result)
    {
        result = 0;
        return Task.FromResult(false);
    }

    protected virtual Task PostJustWaitPlease(int mininum, int maximum, ref int result)
    {
        return Task.CompletedTask;
    }

}
