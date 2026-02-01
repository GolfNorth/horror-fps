using Game.Core.Configuration;
using Game.Core.Events;
using Game.Core.Modules;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.Bootstrap
{
    /// <summary>
    /// Root application scope that lives in DontDestroyOnLoad.
    /// Contains all singleton services shared across scenes.
    /// </summary>
    public sealed class ApplicationScope : LifetimeScope
    {
        [Header("Configuration")]
        [SerializeField] private ConfigSource[] _configSources;

        [Header("Services")]
        [SerializeField] private ServicesModule[] _serviceModules;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterMessagePipe(builder);
            RegisterConfigServices(builder);
            RegisterServiceModules(builder);
            RegisterEntryPoints(builder);
        }

        private static void RegisterMessagePipe(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            builder.RegisterMessageBroker<GameStateChangedEvent>(options);
            builder.RegisterMessageBroker<PauseStateChangedEvent>(options);

            builder.RegisterMessageBroker<PlayerSpawnedEvent>(options);
            builder.RegisterMessageBroker<PlayerDeathEvent>(options);
            builder.RegisterMessageBroker<PlayerHealthChangedEvent>(options);

            builder.RegisterMessageBroker<DamageDealtEvent>(options);
            builder.RegisterMessageBroker<WeaponFiredEvent>(options);
            builder.RegisterMessageBroker<WeaponReloadedEvent>(options);

            builder.RegisterMessageBroker<UINavigationEvent>(options);
            builder.RegisterMessageBroker<UIVisibilityChangedEvent>(options);
        }

        private void RegisterConfigServices(IContainerBuilder builder)
        {
            builder.Register<ConfigRegistry>(Lifetime.Singleton);
            builder.Register<ConfigService>(Lifetime.Singleton).As<IConfigService>().AsSelf();

            builder.RegisterBuildCallback(container =>
            {
                var registry = container.Resolve<ConfigRegistry>();
                RegisterConfigSources(registry);

                var configService = container.Resolve<ConfigService>();
                ConfigServiceLocator.SetInstance(configService);
            });
        }

        private void RegisterConfigSources(ConfigRegistry registry)
        {
            if (_configSources == null) return;

            foreach (var source in _configSources)
            {
                if (source != null)
                {
                    registry.RegisterSource(source);
                }
            }
        }

        private void RegisterServiceModules(IContainerBuilder builder)
        {
            if (_serviceModules == null) return;

            foreach (var module in _serviceModules)
            {
                module?.Configure(builder);
            }
        }

        private static void RegisterEntryPoints(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
