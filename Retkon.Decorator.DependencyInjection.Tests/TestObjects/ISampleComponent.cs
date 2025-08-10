using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjects;
internal interface ISampleComponent
{
    Task<int> JustWaitPlease(int mininum, int maximum);
}
