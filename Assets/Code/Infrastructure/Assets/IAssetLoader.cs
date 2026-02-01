using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Infrastructure.Assets
{
    /// <summary>
    /// Addressable asset loading abstraction.
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// Load an asset by address.
        /// </summary>
        UniTask<T> LoadAsync<T>(string address, CancellationToken cancellation = default)
            where T : Object;

        /// <summary>
        /// Load an asset by AssetReference.
        /// </summary>
        UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellation = default)
            where T : Object;

        /// <summary>
        /// Release a loaded asset.
        /// </summary>
        void Release<T>(T asset) where T : Object;

        /// <summary>
        /// Preload multiple assets.
        /// </summary>
        UniTask PreloadAsync(string[] addresses, CancellationToken cancellation = default);

        /// <summary>
        /// Instantiate a GameObject from AssetReference.
        /// Uses Addressables reference counting.
        /// </summary>
        UniTask<GameObject> InstantiateAsync(
            AssetReference reference,
            Vector3 position,
            Quaternion rotation,
            Transform parent = null,
            CancellationToken cancellation = default);

        /// <summary>
        /// Release an instantiated GameObject.
        /// Decrements reference count and destroys the instance.
        /// </summary>
        void ReleaseInstance(GameObject instance);
    }
}
