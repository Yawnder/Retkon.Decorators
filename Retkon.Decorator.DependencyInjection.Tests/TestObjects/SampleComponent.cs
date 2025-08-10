using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjects;
internal class SampleComponent : ISampleComponent
{
    public async Task<int> JustWaitPlease(int mininum, int maximum)
    {
        var delay = Random.Shared.Next(mininum, maximum);
        await Task.Delay(delay);

        return delay;
    }
}
