using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Infrastructure.Assets;
using UnityEngine;
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
        private readonly PlayerFactoryConfig _config;

        private GameObject _cachedPrefab;

        [Inject]
        public PlayerFactory(
            IObjectResolver resolver,
            IAssetLoader assetLoader,
            PlayerFactoryConfig config)
        {
            _resolver = resolver;
            _assetLoader = assetLoader;
            _config = config;
        }

        public async UniTask<GameObject> CreateAsync(
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            if (_cachedPrefab == null)
            {
                _cachedPrefab = await _assetLoader.LoadAsync<GameObject>(
                    _config.PlayerPrefabReference,
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
