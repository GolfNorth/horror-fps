using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Gameplay.Player.Factory
{
    /// <summary>
    /// Factory interface for creating and destroying player instances.
    /// </summary>
    public interface IPlayerFactory
    {
        /// <summary>
        /// Creates a new player instance at the specified position and rotation.
        /// Loads the prefab via Addressables if not already loaded.
        /// </summary>
        UniTask<GameObject> CreateAsync(Vector3 position, Quaternion rotation, CancellationToken cancellation = default);

        /// <summary>
        /// Destroys the specified player instance.
        /// </summary>
        void Destroy(GameObject player);
    }
}
