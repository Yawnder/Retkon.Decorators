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
    /// If true, Components already decorated by the same Decorator type will be skipped.
    /// </summary>
    public bool SkipSameDecoratorType { get; init; } = true;

    /// <summary>
    /// If null, decorator of the same type will be skipped.
    /// </summary>
    public IReadOnlyList<Type>? SkippedDecoratorTypes
    {
        get => this._skippedDecoratorTypes;
        init
        {
            if (value != null)
                this._skippedDecoratorTypes = new List<Type>(value).AsReadOnly();
        }
    }
    private ReadOnlyCollection<Type>? _skippedDecoratorTypes;

    /// <summary>
    /// Registers the Decorator with that ServiceKey specifically.
    /// </summary>
    public string? DecoratorServiceKey { get; set; }

    /// <summary>
    /// If null, will decorate services with any keys.
    /// </summary>
    public IReadOnlyList<object>? DecoratedServiceKeys
    {
        get => this._decoratedServiceKeys;
        init
        {
            if (value != null)
                this._decoratedServiceKeys = new List<object>(value).AsReadOnly();
        }
    }
    private ReadOnlyCollection<object>? _decoratedServiceKeys;

}
