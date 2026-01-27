using System;

namespace Game.Core.Configuration
{
    public interface IConfigService
    {
        IConfigValue<T> Observe<T>(string key);
        T GetValue<T>(string key);
        Type GetValueType(string key);
        void SetValue<T>(string key, T value);
        void SetValueFromString(string key, string value);
        void ResetValue(string key);
        void ResetAll();
        bool HasKey(string key);
    }
}
