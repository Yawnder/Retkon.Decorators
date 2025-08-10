using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection.Tests.TestObjects;
internal class SampleStoringComponent : ISampleStoring
{

    public int Value { get; private set; }

    public void Store(int value)
    {
        this.Value = value;
    }
}
