using System.Globalization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Core.Configuration.Converters
{
    public class FloatConverter : IConfigValueConverter<float>
    {
        public string ValueToString(float value) => value.ToString(CultureInfo.InvariantCulture);
        public float StringToValue(string value) => float.Parse(value, CultureInfo.InvariantCulture);
    }

    public class IntConverter : IConfigValueConverter<int>
    {
        public string ValueToString(int value) => value.ToString();
        public int StringToValue(string value) => int.Parse(value, CultureInfo.InvariantCulture);
    }

    public class BoolConverter : IConfigValueConverter<bool>
    {
        public string ValueToString(bool value) => value ? "true" : "false";
        public bool StringToValue(string value) => value.ToLowerInvariant() == "true" || value == "1";
    }

    public class StringConverter : IConfigValueConverter<string>
    {
        public string ValueToString(string value) => value ?? "";
        public string StringToValue(string value) => value;
    }

    public class DoubleConverter : IConfigValueConverter<double>
    {
        public string ValueToString(double value) => value.ToString(CultureInfo.InvariantCulture);
        public double StringToValue(string value) => double.Parse(value, CultureInfo.InvariantCulture);
    }

    public class Vector2Converter : IConfigValueConverter<Vector2>
    {
        public string ValueToString(Vector2 v) =>
            $"{v.x.ToString(CultureInfo.InvariantCulture)},{v.y.ToString(CultureInfo.InvariantCulture)}";

        public Vector2 StringToValue(string value)
        {
            var parts = value.Split(',');
            return new Vector2(
                float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture));
        }
    }

    public class Vector3Converter : IConfigValueConverter<Vector3>
    {
        public string ValueToString(Vector3 v) =>
            $"{v.x.ToString(CultureInfo.InvariantCulture)},{v.y.ToString(CultureInfo.InvariantCulture)},{v.z.ToString(CultureInfo.InvariantCulture)}";

        public Vector3 StringToValue(string value)
        {
            var parts = value.Split(',');
            return new Vector3(
                float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture));
        }
    }

    public class ColorConverter : IConfigValueConverter<Color>
    {
        public string ValueToString(Color c) =>
            $"{c.r.ToString(CultureInfo.InvariantCulture)},{c.g.ToString(CultureInfo.InvariantCulture)},{c.b.ToString(CultureInfo.InvariantCulture)},{c.a.ToString(CultureInfo.InvariantCulture)}";

        public Color StringToValue(string value)
        {
            if (ColorUtility.TryParseHtmlString(value, out var color))
                return color;

            var parts = value.Split(',');
            return new Color(
                float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture),
                parts.Length > 3 ? float.Parse(parts[3].Trim(), CultureInfo.InvariantCulture) : 1f);
        }
    }

    public class EnumConverter<T> : IConfigValueConverter<T> where T : struct, System.Enum
    {
        public string ValueToString(T value) => value.ToString();

        public T StringToValue(string value)
        {
            if (System.Enum.TryParse<T>(value, true, out var result))
                return result;

            if (int.TryParse(value, out var intValue))
                return (T)(object)intValue;

            return default;
        }
    }

    public class AssetReferenceConverter : IConfigValueConverter<AssetReference>
    {
        public string ValueToString(AssetReference value)
        {
            if (value == null || !value.RuntimeKeyIsValid())
                return "";

            return value.RuntimeKey.ToString();
        }

        public AssetReference StringToValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            return new AssetReference(value);
        }
    }
}
