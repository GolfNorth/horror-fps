using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Core.Time
{
    [CreateAssetMenu(
        fileName = "GameTimeServiceModule",
        menuName = "Game/Modules/Game Time Service")]
    public class GameTimeServiceModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameTimeService>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}
