using Game.Core.Configuration;
using Game.Infrastructure.Assets;
using Game.Infrastructure.SceneManagement;
using Game.Input;
using Game.Player;
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
        [SerializeField] private GameConfig[] _configs;

        [Header("Player")]
        [SerializeField] private PlayerController _player;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterConfigs(builder);
            RegisterInfrastructureServices(builder);
            RegisterInputServices(builder);
            RegisterPlayer(builder);
        }

        private void RegisterConfigs(IContainerBuilder builder)
        {
            foreach (var config in _configs)
            {
                if (config != null)
                {
                    builder.RegisterInstance(config).As(config.GetType());
                }
            }
        }

        private static void RegisterInfrastructureServices(IContainerBuilder builder)
        {
            builder.Register<IAssetLoader, AddressableAssetLoader>(Lifetime.Scoped);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Scoped);
        }

        private static void RegisterInputServices(IContainerBuilder builder)
        {
            builder.Register<PlayerInputService>(Lifetime.Scoped).AsImplementedInterfaces();
        }

        private void RegisterPlayer(IContainerBuilder builder)
        {
            if (_player == null) return;

            builder.RegisterComponent(_player);

            foreach (var ability in _player.Abilities)
            {
                if (ability != null)
                {
                    builder.RegisterComponent(ability);
                }
            }

            builder.RegisterBuildCallback(resolver =>
            {
                resolver.InjectGameObject(_player.gameObject);
            });
        }
    }
}
