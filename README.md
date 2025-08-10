# Retkon.Decorators

Build Status: [![Test](https://github.com/Yawnder/Retkon.Decorators/actions/workflows/build-test.yml/badge.svg)](https://github.com/Yawnder/Retkon.Decorators/actions/workflows/build-test.yml)

## Retkon.Decorators.DependencyInjection

### Usage
```cs
var serviceCollection = new ServiceCollection();
serviceCollection.AddSingleton<ISampleStoring, SampleStoringComponent>();
serviceCollection.Decorate<SampleAddingDecorator, ISampleStoring>();
var serviceProvider = serviceCollection.BuildServiceProvider(true);

var sampleStoring = serviceProvider.GetRequiredService<ISampleStoring>();

// Will instantiate an `SampleAddingDecorator` that owns a `SampleStoringComponent`.
sampleStoring.Store(0);
```

### What it does
When using `serviceCollection.Decorate<TDecorator, TComponent>();`, it looks for all registrations of `TComponent`, and converts them to a _Keyed Registration_, and then adds a registration identical to the previous registration, but that will instantiate a `TDecorator` instead, which will own a `TComponent`.

### Supported use cases