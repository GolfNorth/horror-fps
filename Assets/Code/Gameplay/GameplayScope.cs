using Game.Gameplay.Character.Factory;
using Game.Gameplay.Player.Factory;
using Game.Infrastructure.Assets;
using Game.Infrastructure.SceneManagement;
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
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterInfrastructureServices(builder);
            RegisterFactories(builder);
        }

        private static void RegisterInfrastructureServices(IContainerBuilder builder)
        {
            builder.Register<IAssetLoader, AddressableAssetLoader>(Lifetime.Scoped);
            builder.Register<ISceneLoader, SceneLoader>(Lifetime.Scoped);
        }

        private static void RegisterFactories(IContainerBuilder builder)
        {
            builder.Register<CharacterFactory>(Lifetime.Scoped).As<ICharacterFactory>();
            builder.Register<PlayerFactory>(Lifetime.Scoped).As<IPlayerFactory>();
        }
    }
}
