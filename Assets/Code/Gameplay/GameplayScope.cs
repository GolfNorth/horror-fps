using Game.Core.Configuration;
using Game.Core.Modules;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    /// <summary>
    /// Scene-level scope for gameplay scenes.
    /// Child of ApplicationScope, contains scene-specific services.
    /// </summary>
    public sealed class GameplayScope : LifetimeScope
    {
        [Header("Configuration")]
        [SerializeField] private ConfigSource[] _configSources;

        [Header("Services")]
        [SerializeField] private ServicesModule[] _serviceModules;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigSources(builder);
            RegisterServiceModules(builder);
        }

        private void RegisterConfigSources(IContainerBuilder builder)
        {
            if (_configSources == null || _configSources.Length == 0) return;

            builder.RegisterBuildCallback(container =>
            {
                var registry = container.Resolve<ConfigRegistry>();
                foreach (var source in _configSources)
                {
                    if (source != null)
                    {
                        registry.RegisterSource(source);
                    }
                }
            });
        }

        private void RegisterServiceModules(IContainerBuilder builder)
        {
            if (_serviceModules == null) return;

            foreach (var module in _serviceModules)
            {
                module?.Configure(builder);
            }
        }
    }
}
