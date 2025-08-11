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
When using `serviceCollection.Decorate<TDecorator, TComponent>();`, it looks for all registrations of `TComponent`, and converts them to a shadow _Keyed Registration_ (only the Decorator uses that _Service Key_), and then adds a registration identical to the previous registration, but that will instantiate a `TDecorator` instead, which will own a `TComponent`.

### Supported use cases
- `Decorate` that registers the decorator as a Service Type, with the `ServiceLifetime` of the decorated `TComponent`.
- `Decorate` that registers the decorator as a Factory, with the `ServiceLifetime` of the decorated `TComponent`.
- `Decorate` can only target specific _Keyed Registrations_ if instructed to.

### To be supported use cases.

### Unsupported use cases
- `Decorate` that registers _an instance of_ the decorator as a Singleton. At this point, sure it could be a decorator, but you don't need me to hook everything up.
- `Decorate` that decorates registrations that aren't directly of `TComponent` (`typeof(TComponent).IsAssignableFrom(...)`). Register your services as interfaces if you want them decorated.

![It doesn't make sense.](https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExZzVsaHZ1Zzk3OGx0ajlkZTQ1eWV3bnFrZzRncW1ydGhnMDFlb3FwYiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/3o6ZtlRhUv5G70Ecc8/giphy.gif)

## License
It's AGPL3, but contact me for a different one and I'll see what can be done.
