using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Input
{
    [CreateAssetMenu(
        fileName = "PlayerInputModule",
        menuName = "Game/Modules/Player Input")]
    public class PlayerInputModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerInputService>(Lifetime.Scoped).As<IPlayerInput>();
        }
    }
}
