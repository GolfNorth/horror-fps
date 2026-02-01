using Game.Core.Modules;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Core.Coroutines
{
    [CreateAssetMenu(
        fileName = "CoroutineServiceModule",
        menuName = "Game/Modules/Coroutine Service")]
    public class CoroutineServiceModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentOnNewGameObject<CoroutineRunner>(
                Lifetime.Singleton,
                "CoroutineRunner"
            ).As<ICoroutineRunner>();
        }
    }
}
