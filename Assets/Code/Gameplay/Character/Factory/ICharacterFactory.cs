using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Factory for creating characters from prefabs with CharacterScope.
    /// </summary>
    public interface ICharacterFactory
    {
        /// <summary>
        /// Creates a character from prefab at the given position.
        /// Prefab must have CharacterScope component configured.
        /// </summary>
        /// <param name="prefab">Character prefab with CharacterScope</param>
        /// <param name="position">Spawn position</param>
        /// <param name="rotation">Spawn rotation</param>
        /// <param name="cancellation">Cancellation token</param>
        /// <returns>Created character's root GameObject</returns>
        UniTask<GameObject> CreateAsync(
            GameObject prefab,
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default);

        /// <summary>
        /// Destroys a character and its scope.
        /// </summary>
        void Destroy(GameObject character);
    }
}
