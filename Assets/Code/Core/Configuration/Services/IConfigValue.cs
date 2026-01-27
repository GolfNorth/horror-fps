using R3;

namespace Game.Core.Configuration
{
    public interface IConfigValue<T>
    {
        T Value { get; }
        Observable<T> Changed { get; }
    }
}
