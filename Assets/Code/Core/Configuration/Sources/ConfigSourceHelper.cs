#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Game.Core.Configuration
{
    public static class ConfigSourceHelper
    {
        private static List<ValueDropdownItem<IConfigSection>> _cachedItems;
        private static bool _initialized;

        public static IEnumerable GetAllSections()
        {
            if (!_initialized)
            {
                BuildCache();
                _initialized = true;
            }

            return _cachedItems;
        }

        private static void BuildCache()
        {
            _cachedItems = new List<ValueDropdownItem<IConfigSection>>();

            var interfaceType = typeof(IConfigSection);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!type.IsValueType || type.IsAbstract || !interfaceType.IsAssignableFrom(type))
                            continue;

                        var defaultProperty = type.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);

                        IConfigSection instance;

                        if (defaultProperty != null)
                        {
                            instance = (IConfigSection)defaultProperty.GetValue(null);
                        }
                        else
                        {
                            instance = (IConfigSection)Activator.CreateInstance(type);
                        }

                        var displayName = instance.DisplayName ?? type.Name;

                        _cachedItems.Add(new ValueDropdownItem<IConfigSection>(displayName, instance));
                    }
                }
                catch
                {
                    // Skip problematic assemblies
                }
            }

            _cachedItems = _cachedItems.OrderBy(x => x.Text).ToList();
        }

        public static void ClearCache()
        {
            _cachedItems = null;
            _initialized = false;
        }
    }
}
#endif