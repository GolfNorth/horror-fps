using Game.Core.Modules;
using Game.Gameplay.Character.Intents;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Input
{
    [CreateAssetMenu(
        fileName = "PlayerInputModule",
        menuName = "Game/Modules/Player Input")]
    public class PlayerInputModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<IntentBuffer>(Lifetime.Scoped).As<IIntentBuffer>();
            builder.RegisterEntryPoint<PlayerInputService>().As<IPlayerInput>();
        }
    }
}
