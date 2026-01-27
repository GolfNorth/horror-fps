using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Core.Configuration.Converters;

namespace Game.Core.Configuration
{
    public class ConfigRegistry
    {
        private readonly Dictionary<string, string> _values = new();
        private readonly Dictionary<string, Type> _types = new();

        public IReadOnlyDictionary<string, string> Values => _values;
        public IReadOnlyDictionary<string, Type> Types => _types;

        public void RegisterSource(ConfigSource source)
        {
            if (source == null) return;

            var rootKey = source.RootKey;

            foreach (var section in source.Sections)
            {
                if (section == null) continue;

                var sectionKey = string.IsNullOrEmpty(rootKey)
                    ? section.Key
                    : $"{rootKey}.{section.Key}";

                ParseSection(sectionKey, section);
            }
        }

        public void Clear()
        {
            _values.Clear();
            _types.Clear();
        }

        private void ParseSection(string prefix, IConfigSection section)
        {
            var type = section.GetType();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<ConfigKeyAttribute>();
                if (attr == null) continue;

                var fullKey = $"{prefix}.{attr.Key}";
                var value = field.GetValue(section);
                var fieldType = field.FieldType;
                var stringValue = ConvertToString(value, fieldType);

                _values[fullKey] = stringValue;
                _types[fullKey] = fieldType;
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            return _values.TryGetValue(key, out value);
        }

        public bool TryGetType(string key, out Type type)
        {
            return _types.TryGetValue(key, out type);
        }

        private static string ConvertToString(object value, Type type)
        {
            if (value == null) return "";

            var method = typeof(ConfigConverterRegistry)
                .GetMethod(nameof(ConfigConverterRegistry.ValueToString))
                ?.MakeGenericMethod(type);

            return method?.Invoke(null, new[] { value }) as string ?? value.ToString();
        }
    }
}
