using Game.Core.Configuration;
using Game.Core.Coroutines;
using Game.Core.Events;
using Game.Core.Logging;
using Game.Core.Time;
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
        [SerializeField]
        private GameConfig[] _configs;

        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();

            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));

            RegisterConfigs(builder);
            RegisterMessageBrokers(builder, options);
            RegisterCoreServices(builder);
            RegisterEntryPoints(builder);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            foreach (var config in _configs)
            {
                if (config != null)
                    builder.RegisterInstance(config).As(config.GetType());
            }
        }

        private static void RegisterMessageBrokers(IContainerBuilder builder, MessagePipeOptions options)
        {
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

        private static void RegisterCoreServices(IContainerBuilder builder)
        {
            builder.Register<UnityLogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<GameTimeService>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterComponentOnNewGameObject<CoroutineRunner>(
                Lifetime.Singleton,
                "CoroutineRunner"
            ).As<ICoroutineRunner>();
        }

        private static void RegisterEntryPoints(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameBootstrapper>();
        }
    }
}
