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

It's not necessary to register the Decorator beforehand.

### What it does
When using `serviceCollection.Decorate<TDecorator, TComponent>();`, it looks for all registrations of `TComponent`, and converts them to a _Keyed Registration_, and then adds a registration identical to the previous registration, but that will instantiate a `TDecorator` instead, which will own a `TComponent`.

### Supported use cases
- `Decorate` that registers the decorator as a Service Type.

### To be supported use cases.
- `Decorate` that registers the decorator as a Factory.

### Unsupported use cases
- `Decorate` that registers the decorator as a Singleton.
![It doesn't make sense.](https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExZzVsaHZ1Zzk3OGx0ajlkZTQ1eWV3bnFrZzRncW1ydGhnMDFlb3FwYiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3o6ZtlRhUv5G70Ecc8/giphy.gif)