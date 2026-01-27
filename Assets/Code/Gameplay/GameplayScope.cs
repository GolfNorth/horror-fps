using Game.Gameplay.Player.Factory;
using Game.Gameplay.Player.Input;
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
            RegisterInputServices(builder);
            RegisterPlayerFactory(builder);
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

        private static void RegisterPlayerFactory(IContainerBuilder builder)
        {
            builder.Register<PlayerFactory>(Lifetime.Scoped).As<IPlayerFactory>();
        }
    }
}
