using System;
using Game.Core.Configuration;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Gameplay.Player.Factory
{
    [Serializable]
    public struct PlayerFactorySection : IConfigSection
    {
        public string Key => "factory";
        public string DisplayName => "Player/Factory";

        [ConfigKey("prefab")]
        [SerializeField] private AssetReference _prefab;
    }
}
