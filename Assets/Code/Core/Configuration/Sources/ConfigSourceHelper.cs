#if UNITY_EDITOR
using System;
using System.Collections;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Game.Core.Configuration
{
    public static class ConfigSourceHelper
    {
        private static ValueDropdownList<IConfigSection> _sectionDropdown;

        private static void EnsureTypesLoaded()
        {
            if (_sectionDropdown != null)
                return;
            _sectionDropdown = new ValueDropdownList<IConfigSection>();
            var interfaceType = typeof(IConfigSection);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsValueType && !type.IsAbstract && interfaceType.IsAssignableFrom(type))
                        {
                            var defaultProp = type.GetProperty("Default", BindingFlags.Public | BindingFlags.Static)!;
                            var instance = (IConfigSection)defaultProp.GetValue(null);
                            _sectionDropdown.Add(new ValueDropdownItem<IConfigSection>(instance.DisplayName, instance));
                        }
                    }
                }
                catch
                {
                    // Skip assemblies that can't be loaded
                }
            }
        }

        internal static IEnumerable GetAllSections()
        {
            EnsureTypesLoaded();

            foreach (var item in _sectionDropdown)
            {
                yield return item;
            }
        }
    }
}
#endif