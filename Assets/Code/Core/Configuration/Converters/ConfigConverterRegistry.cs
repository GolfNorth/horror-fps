using System;
using System.Collections.Generic;

namespace Game.Core.Configuration.Converters
{
    public static class ConfigConverterRegistry
    {
        private static readonly Dictionary<Type, object> Converters = new();

        static ConfigConverterRegistry()
        {
            Register(new FloatConverter());
            Register(new IntConverter());
            Register(new BoolConverter());
            Register(new StringConverter());
            Register(new DoubleConverter());
            Register(new Vector2Converter());
            Register(new Vector3Converter());
            Register(new ColorConverter());
            Register(new AssetReferenceConverter());
        }

        public static void Register<T>(IConfigValueConverter<T> converter)
        {
            Converters[typeof(T)] = converter;
        }

        public static IConfigValueConverter<T> Get<T>()
        {
            if (Converters.TryGetValue(typeof(T), out var converter))
                return (IConfigValueConverter<T>)converter;

            // Auto-create converter for enums
            if (typeof(T).IsEnum)
            {
                var converterType = typeof(EnumConverter<>).MakeGenericType(typeof(T));
                var enumConverter = Activator.CreateInstance(converterType);
                Converters[typeof(T)] = enumConverter;
                return (IConfigValueConverter<T>)enumConverter;
            }

            throw new InvalidOperationException($"No converter registered for type {typeof(T).Name}");
        }

        public static bool TryGet<T>(out IConfigValueConverter<T> converter)
        {
            if (Converters.TryGetValue(typeof(T), out var obj))
            {
                converter = (IConfigValueConverter<T>)obj;
                return true;
            }

            converter = null;
            return false;
        }

        public static T StringToValue<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
                return default;

            return Get<T>().StringToValue(value);
        }

        public static string ValueToString<T>(T value)
        {
            if (value == null)
                return "";

            return Get<T>().ValueToString(value);
        }
    }
}
