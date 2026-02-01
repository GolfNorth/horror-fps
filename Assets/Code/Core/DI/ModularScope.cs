using Game.Core.Configuration;
using Game.Core.Modules;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.DI
{
    /// <summary>
    /// Base scope with ConfigSource and ServicesModule support.
    /// Inherit for scene or entity scopes.
    /// </summary>
    public abstract class ModularScope : LifetimeScope
    {
        [Header("Configuration")]
        [SerializeField] private ConfigSource[] _configSources;

        [Header("Modules")]
        [SerializeField] private ServicesModule[] _serviceModules;

        protected override void Configure(IContainerBuilder builder)
        {
            ConfigureScope(builder);
            RegisterConfigSources(builder);
            RegisterServiceModules(builder);
        }

        /// <summary>
        /// Override to add scope-specific registrations.
        /// Called before config sources and modules.
        /// </summary>
        protected virtual void ConfigureScope(IContainerBuilder builder) { }

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
