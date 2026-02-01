using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Factory
{
    [CreateAssetMenu(
        fileName = "CharacterFactoryModule",
        menuName = "Game/Modules/Character Factory")]
    public class CharacterFactoryModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<CharacterFactory>(Lifetime.Scoped).As<ICharacterFactory>();
        }
    }
}
