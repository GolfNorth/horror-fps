using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core.Configuration;
using Game.Infrastructure.Assets;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Factory that creates characters from prefabs with CharacterScope.
    /// Loads prefabs via Addressables based on character ID from config.
    /// Config keys: {characterId}.prefab
    /// </summary>
    public sealed class CharacterFactory : ICharacterFactory
    {
        private readonly LifetimeScope _parentScope;
        private readonly IAssetLoader _assetLoader;
        private readonly IConfigService _config;
        private readonly Dictionary<string, GameObject> _prefabCache = new();
        private readonly Dictionary<GameObject, CharacterScope> _instances = new();

        [Inject]
        public CharacterFactory(
            LifetimeScope parentScope,
            IAssetLoader assetLoader,
            IConfigService config)
        {
            _parentScope = parentScope;
            _assetLoader = assetLoader;
            _config = config;
        }

        public async UniTask<GameObject> CreateAsync(
            string characterId,
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            var prefab = await GetOrLoadPrefabAsync(characterId, cancellation);
            if (prefab == null)
            {
                return null;
            }

            return Instantiate(prefab, position, rotation);
        }

        public void Destroy(GameObject character)
        {
            if (character == null) return;

            _instances.Remove(character, out _);
            Object.Destroy(character);
        }

        private async UniTask<GameObject> GetOrLoadPrefabAsync(
            string characterId,
            CancellationToken cancellation)
        {
            if (_prefabCache.TryGetValue(characterId, out var cached))
            {
                return cached;
            }

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

            var prefab = await _assetLoader.LoadAsync<GameObject>(reference, cancellation);
            if (prefab == null)
            {
                Debug.LogError($"Failed to load prefab for character: {characterId}");
                return null;
            }

            if (prefab.GetComponent<CharacterScope>() == null)
            {
                Debug.LogError($"Character prefab '{prefab.name}' must have CharacterScope component");
                return null;
            }

            _prefabCache[characterId] = prefab;
            return prefab;
        }

        private GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            using (LifetimeScope.EnqueueParent(_parentScope))
            {
                var instance = Object.Instantiate(prefab, position, rotation);
                var scope = instance.GetComponent<CharacterScope>();

                scope.Container.InjectGameObject(instance);
                _instances[instance] = scope;

                return instance;
            }
        }
    }
}
