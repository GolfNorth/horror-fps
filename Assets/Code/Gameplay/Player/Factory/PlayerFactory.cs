using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Configuration;
using Game.Infrastructure.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Factory
{
    /// <summary>
    /// Factory for creating player instances with dependency injection.
    /// Loads player prefab via Addressables.
    /// </summary>
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IObjectResolver _resolver;
        private readonly IAssetLoader _assetLoader;
        private readonly IConfigValue<AssetReference> _prefabReference;

        private GameObject _cachedPrefab;

        [Inject]
        public PlayerFactory(
            IObjectResolver resolver,
            IAssetLoader assetLoader,
            IConfigService config)
        {
            _resolver = resolver;
            _assetLoader = assetLoader;
            _prefabReference = config.Observe<AssetReference>("player.factory.prefab");
        }

        public async UniTask<GameObject> CreateAsync(
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            if (_cachedPrefab == null)
            {
                _cachedPrefab = await _assetLoader.LoadAsync<GameObject>(
                    _prefabReference.Value,
                    cancellation);
            }

            var instance = Object.Instantiate(_cachedPrefab, position, rotation);
            _resolver.InjectGameObject(instance);
            return instance;
        }

        public void Destroy(GameObject player)
        {
            if (player != null)
            {
                Object.Destroy(player);
            }
        }
    }
}
