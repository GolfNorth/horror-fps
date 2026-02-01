using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Factory
{
    [CreateAssetMenu(
        fileName = "PlayerFactoryModule",
        menuName = "Game/Modules/Player Factory")]
    public class PlayerFactoryModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<PlayerFactory>(Lifetime.Scoped).As<IPlayerFactory>();
        }
    }
}
