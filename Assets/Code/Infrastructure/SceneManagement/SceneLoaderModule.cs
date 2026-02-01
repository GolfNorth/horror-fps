using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Infrastructure.SceneManagement
{
    [CreateAssetMenu(
        fileName = "SceneLoaderModule",
        menuName = "Game/Modules/Scene Loader")]
    public class SceneLoaderModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<SceneLoader>(Lifetime.Scoped).As<ISceneLoader>();
        }
    }
}
