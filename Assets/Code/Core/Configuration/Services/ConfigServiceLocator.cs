using System;

namespace Game.Core.Configuration
{
    /// <summary>
    /// Service locator for ConfigService.
    /// Used only for Editor tools access. Do not use in gameplay code.
    /// </summary>
    public static class ConfigServiceLocator
    {
        private static ConfigService _instance;

        public static ConfigService Instance => _instance;
        public static bool HasInstance => _instance != null;

        public static event Action<ConfigService> OnInstanceChanged;

        internal static void SetInstance(ConfigService service)
        {
            _instance = service;
            OnInstanceChanged?.Invoke(service);
        }

        internal static void ClearInstance()
        {
            _instance = null;
            OnInstanceChanged?.Invoke(null);
        }
    }
}
