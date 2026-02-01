using Game.Core.Modules;
using UnityEngine;
using VContainer;

namespace Game.Infrastructure.Assets
{
    [CreateAssetMenu(
        fileName = "AssetLoaderModule",
        menuName = "Game/Modules/Asset Loader")]
    public class AssetLoaderModule : ServicesModule
    {
        public override void Configure(IContainerBuilder builder)
        {
            builder.Register<AddressableAssetLoader>(Lifetime.Scoped).As<IAssetLoader>();
        }
    }
}
