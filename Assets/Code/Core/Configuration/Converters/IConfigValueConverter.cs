namespace Game.Core.Configuration.Converters
{
    public interface IConfigValueConverter<T>
    {
        string ValueToString(T value);
        T StringToValue(string value);
    }
}
