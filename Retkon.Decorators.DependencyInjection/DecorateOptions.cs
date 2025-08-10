using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retkon.Decorators.DependencyInjection;
public class DecorateOptions
{

    /// <summary>
    /// If true, will not Decorate a registration already decorated by the same Decorator type.
    /// </summary>
    public bool SkipSameDecoratorType { get; init; } = true;

    /// <summary>
    /// If null, decorator of the same type will be skipped.
    /// </summary>
    private ReadOnlyCollection<Type>? _skippedDecoratorTypes;
    public IReadOnlyList<Type>? SkippedDecoratorTypes
    {
        get => this._skippedDecoratorTypes;
        init
        {
            if (value != null)
                this._skippedDecoratorTypes = new List<Type>(value).AsReadOnly();
        }
    }

}
