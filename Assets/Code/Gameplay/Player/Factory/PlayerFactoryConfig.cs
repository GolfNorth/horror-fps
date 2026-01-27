using Game.Core.Configuration;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Gameplay.Player.Factory
{
    /// <summary>
    /// Configuration for player factory.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerFactoryConfig", menuName = "Game/Player/Factory Config")]
    public class PlayerFactoryConfig : GameConfig
    {
        [SerializeField] private AssetReference _playerPrefabReference;

        public AssetReference PlayerPrefabReference => _playerPrefabReference;
    }
}
