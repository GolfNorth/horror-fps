using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Configuration;
using Game.Infrastructure.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Factory that creates characters from prefabs with CharacterScope.
    /// Uses Addressables InstantiateAsync for proper reference counting.
    /// Config keys: {characterId}.prefab
    /// </summary>
    public sealed class CharacterFactory : ICharacterFactory
    {
        private readonly IAssetLoader _assetLoader;
        private readonly IConfigService _config;

        [Inject]
        public CharacterFactory(IAssetLoader assetLoader, IConfigService config)
        {
            _assetLoader = assetLoader;
            _config = config;
        }

        public async UniTask<GameObject> CreateAsync(
            string characterId,
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            var key = $"factory.{characterId}.prefab";
            if (!_config.HasKey(key))
            {
                Debug.LogError($"Character prefab config not found: {key}");
                return null;
            }

            var reference = _config.GetValue<AssetReference>(key);
            if (reference == null || !reference.RuntimeKeyIsValid())
            {
                Debug.LogError($"Invalid AssetReference for character: {characterId}");
                return null;
            }

            var instance = await _assetLoader.InstantiateAsync(reference, position, rotation, null, cancellation);
            if (instance == null)
            {
                Debug.LogError($"Failed to instantiate character: {characterId}");
                return null;
            }

            if (instance.GetComponent<CharacterScope>() == null)
            {
                Debug.LogError($"Character prefab '{instance.name}' must have CharacterScope component");
                _assetLoader.ReleaseInstance(instance);
                return null;
            }

            return instance;
        }

        public void Destroy(GameObject character)
        {
            if (character == null) return;

            _assetLoader.ReleaseInstance(character);
        }
    }
}
