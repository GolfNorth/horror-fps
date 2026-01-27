using System;

namespace Game.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigKeyAttribute : Attribute
    {
        public string Key { get; }

        public ConfigKeyAttribute(string key)
        {
            Key = key;
        }
    }
}
