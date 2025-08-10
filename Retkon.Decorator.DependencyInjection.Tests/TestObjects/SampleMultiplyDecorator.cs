using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorator.DependencyInjection.Tests.TestObjects;
internal class SampleMultiplyDecorator : ISampleStoring
{
    private readonly ISampleStoring sampleAdding;

    public int Value => this.sampleAdding.Value;

    public SampleMultiplyDecorator(
        ISampleStoring sampleAdding)
    {
        this.sampleAdding = sampleAdding;
    }

    public void Store(int value)
    {
        Console.WriteLine($"Value {value} multiplied by to: {value * 2}");
        this.sampleAdding.Store(value * 2);
    }
}
