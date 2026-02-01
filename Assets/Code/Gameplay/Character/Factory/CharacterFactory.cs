using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Factory that creates characters from prefabs with CharacterScope.
    /// Each prefab must have CharacterScope component configured in Inspector.
    /// </summary>
    public sealed class CharacterFactory : ICharacterFactory
    {
        private readonly LifetimeScope _parentScope;
        private readonly Dictionary<GameObject, CharacterScope> _characterScopes = new();

        [Inject]
        public CharacterFactory(LifetimeScope parentScope)
        {
            _parentScope = parentScope;
        }

        public UniTask<GameObject> CreateAsync(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            // Prefab must have CharacterScope
            if (prefab.GetComponent<CharacterScope>() == null)
            {
                Debug.LogError($"Character prefab '{prefab.name}' must have CharacterScope component");
                return UniTask.FromResult<GameObject>(null);
            }

            // Enqueue parent so CharacterScope picks it up during Awake
            using (LifetimeScope.EnqueueParent(_parentScope))
            {
                var instance = Object.Instantiate(prefab, position, rotation);
                var scope = instance.GetComponent<CharacterScope>();

                // Inject dependencies into all child components
                scope.Container.InjectGameObject(instance);

                // Track for cleanup
                _characterScopes[instance] = scope;

                return UniTask.FromResult(instance);
            }
        }

        public void Destroy(GameObject character)
        {
            if (character == null) return;

            _characterScopes.Remove(character, out _);

            Object.Destroy(character);
        }
    }
}
