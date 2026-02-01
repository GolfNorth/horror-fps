using System;
using Game.Core.Configuration;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Gameplay.Character.Factory
{
    /// <summary>
    /// Config section for a character prefab.
    /// Key format: {characterId}.prefab
    /// </summary>
    [Serializable]
    public struct CharacterSection : IConfigSection
    {
        public string Key => _id;
        public string DisplayName => "Character/Prefab";

        [SerializeField] private string _id;

        [ConfigKey("prefab")]
        [SerializeField] private AssetReference _prefab;
    }
}
