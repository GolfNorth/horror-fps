using System;
using Game.Core.Configuration.Converters;
using R3;

namespace Game.Core.Configuration
{
    internal class ConfigValue<T> : IConfigValue<T>, IDisposable
    {
        private readonly IDisposable _subscription;
        private T _cachedValue;

        public T Value => _cachedValue;
        public Observable<T> Changed { get; }

        public ConfigValue(ReactiveProperty<string> source)
        {
            _cachedValue = ConfigConverterRegistry.StringToValue<T>(source.Value);

            Changed = source
                .Skip(1)
                .Select(ConfigConverterRegistry.StringToValue<T>);

            _subscription = Changed.Subscribe(v => _cachedValue = v);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
        }
    }
}
