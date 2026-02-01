using Game.Core.Modules;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.Ticking
{
    [CreateAssetMenu(
        fileName = "TickServiceModule",
        menuName = "Game/Modules/Tick Service")]
    public class TickServiceModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentOnNewGameObject<TickService>(
                Lifetime.Singleton,
                "TickService"
            ).As<ITickService>();
        }
    }
}
