using System;
using System.Collections.Generic;
using Game.Core.Configuration.Converters;
using R3;

namespace Game.Core.Configuration
{
    public class ConfigService : IConfigService, IDisposable
    {
        private readonly ConfigRegistry _registry;
        private readonly Dictionary<string, ReactiveProperty<string>> _properties = new();
        private readonly Dictionary<string, object> _bindings = new();
        private readonly HashSet<string> _overriddenKeys = new();

        public ConfigService(ConfigRegistry registry)
        {
            _registry = registry;
        }

        public IConfigValue<T> Observe<T>(string key)
        {
            if (_bindings.TryGetValue(key, out var existing))
            {
                if (existing is IConfigValue<T> typed)
                    return typed;

                throw new InvalidOperationException(
                    $"Config key '{key}' already bound with different type. " +
                    $"Expected {typeof(T).Name}, got {existing.GetType().GenericTypeArguments[0].Name}");
            }

            var property = GetOrCreateProperty(key);
            var binding = new ConfigValue<T>(property);
            _bindings[key] = binding;

            return binding;
        }

        public T GetValue<T>(string key)
        {
            var property = GetOrCreateProperty(key);
            var binding = new ConfigValue<T>(property);
            return binding.Value;
        }

        public Type GetValueType(string key)
        {
            if (_registry.TryGetType(key, out var type))
                return type;

            throw new KeyNotFoundException($"Config key not found: '{key}'");
        }

        public void SetValue<T>(string key, T value)
        {
            var stringValue = ConfigConverterRegistry.ValueToString(value);
            SetValueInternal(key, stringValue);
        }

        public void SetValueFromString(string key, string value)
        {
            SetValueInternal(key, value);
        }

        private void SetValueInternal(string key, string value)
        {
            if (!_registry.Values.ContainsKey(key))
                throw new KeyNotFoundException($"Config key not found: '{key}'");

            _overriddenKeys.Add(key);
            var property = GetOrCreateProperty(key);
            property.Value = value;
        }

        public void ResetValue(string key)
        {
            if (!_overriddenKeys.Remove(key)) return;

            if (_registry.TryGetValue(key, out var baseValue) && _properties.TryGetValue(key, out var property))
            {
                property.Value = baseValue;
            }
        }

        public void ResetAll()
        {
            var keys = new List<string>(_overriddenKeys);
            _overriddenKeys.Clear();

            foreach (var key in keys)
            {
                if (_registry.TryGetValue(key, out var baseValue) && _properties.TryGetValue(key, out var property))
                {
                    property.Value = baseValue;
                }
            }
        }

        public bool HasKey(string key)
        {
            return _registry.Values.ContainsKey(key);
        }

        public IEnumerable<string> GetAllKeys()
        {
            return _registry.Values.Keys;
        }

        public bool IsOverridden(string key)
        {
            return _overriddenKeys.Contains(key);
        }

        public string GetStringValue(string key)
        {
            if (_properties.TryGetValue(key, out var property))
                return property.Value;

            if (_registry.TryGetValue(key, out var value))
                return value;

            throw new KeyNotFoundException($"Config key not found: '{key}'");
        }

        private ReactiveProperty<string> GetOrCreateProperty(string key)
        {
            if (_properties.TryGetValue(key, out var existing))
                return existing;

            if (!_registry.TryGetValue(key, out var baseValue))
                throw new KeyNotFoundException($"Config key not found: '{key}'");

            var property = new ReactiveProperty<string>(baseValue);
            _properties[key] = property;
            return property;
        }

        public void Dispose()
        {
            foreach (var binding in _bindings.Values)
            {
                if (binding is IDisposable disposable)
                    disposable.Dispose();
            }

            foreach (var property in _properties.Values)
            {
                property.Dispose();
            }

            _bindings.Clear();
            _properties.Clear();
            _overriddenKeys.Clear();
        }
    }
}
