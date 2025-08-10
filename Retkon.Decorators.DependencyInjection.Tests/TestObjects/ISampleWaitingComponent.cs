using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection.Tests.TestObjects;
internal interface ISampleWaitingComponent
{
    Task<int> JustWaitPlease(int mininum, int maximum);
}
