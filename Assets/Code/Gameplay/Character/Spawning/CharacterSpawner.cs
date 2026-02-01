using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Gameplay.Character.Factory;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Spawning
{
    /// <summary>
    /// Handles character spawning and despawning in the scene.
    /// </summary>
    public class CharacterSpawner : MonoBehaviour
    {
        [SerializeField] private string _characterId = "player";
        [SerializeField] private bool _spawnOnStart = true;
        [SerializeField] private Transform _spawnPoint;

        private ICharacterFactory _factory;
        private GameObject _current;
        private CancellationTokenSource _cts;

        public string CharacterId => _characterId;
        public GameObject Current => _current;
        public bool HasCharacter => _current != null;

        [Inject]
        public void Construct(ICharacterFactory factory)
        {
            _factory = factory;

            if (_spawnOnStart)
            {
                SpawnAsync().Forget();
            }
        }

        public async UniTask<GameObject> SpawnAsync(CancellationToken cancellation = default)
        {
            if (_current != null)
            {
                Despawn();
            }

            var position = _spawnPoint != null ? _spawnPoint.position : Vector3.zero;
            var rotation = _spawnPoint != null ? _spawnPoint.rotation : Quaternion.identity;

            _current = await _factory.CreateAsync(_characterId, position, rotation, cancellation);
            return _current;
        }

        public async UniTask<GameObject> SpawnAsync(
            Vector3 position,
            Quaternion rotation,
            CancellationToken cancellation = default)
        {
            if (_current != null)
            {
                Despawn();
            }

            _current = await _factory.CreateAsync(_characterId, position, rotation, cancellation);
            return _current;
        }

        public void Despawn()
        {
            if (_current == null)
                return;

            _factory.Destroy(_current);
            _current = null;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            Despawn();
        }
    }
}
