using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Player.Factory;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Spawning
{
    /// <summary>
    /// Handles player spawning and despawning in the scene.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private bool _spawnOnStart = true;

        [SerializeField]
        private Transform _spawnPoint;

        private IPlayerFactory _factory;
        private GameObject _currentPlayer;
        private CancellationTokenSource _cts;

        /// <summary>
        /// The currently spawned player instance.
        /// </summary>
        public GameObject CurrentPlayer => _currentPlayer;

        /// <summary>
        /// Whether a player is currently spawned.
        /// </summary>
        public bool HasPlayer => _currentPlayer != null;

        [Inject]
        public void Construct(IPlayerFactory factory)
        {
            _factory = factory;

            if (_spawnOnStart)
            {
                SpawnAsync().Forget();
            }
        }

        /// <summary>
        /// Spawns a new player at the spawn point.
        /// If a player already exists, it will be despawned first.
        /// </summary>
        public async UniTask<GameObject> SpawnAsync(CancellationToken cancellation = default)
        {
            if (_currentPlayer != null)
            {
                Despawn();
            }

            var position = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
            var rotation = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

            _currentPlayer = await _factory.CreateAsync(position, rotation, cancellation);
            return _currentPlayer;
        }

        /// <summary>
        /// Spawns a new player at the specified position and rotation.
        /// If a player already exists, it will be despawned first.
        /// </summary>
        public async UniTask<GameObject> SpawnAsync(
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            if (_currentPlayer != null)
            {
                Despawn();
            }

            _currentPlayer = await _factory.CreateAsync(position, rotation, cancellation);
            return _currentPlayer;
        }

        /// <summary>
        /// Despawns the current player.
        /// </summary>
        public void Despawn()
        {
            if (_currentPlayer == null)
                return;

            _factory.Destroy(_currentPlayer);
            _currentPlayer = null;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            Despawn();
        }
    }
}