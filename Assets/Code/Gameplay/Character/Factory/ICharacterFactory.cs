using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Factory for creating characters from prefabs with CharacterScope.
    /// Loads prefabs via Addressables based on character ID from config.
    /// </summary>
    public interface ICharacterFactory
    {
        /// <summary>
        /// Creates a character by ID. Loads prefab via Addressables from config.
        /// </summary>
        /// <param name="characterId">Character ID defined in CharacterPrefabsSection</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Created character's root GameObject</returns>
        UniTask<GameObject> CreateAsync(
            string characterId,
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default);

        /// <summary>
        /// Destroys a character and its scope.
        /// </summary>
        void Destroy(GameObject character);
    }
}
