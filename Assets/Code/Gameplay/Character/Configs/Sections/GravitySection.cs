using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Configs.Sections
{
    [Serializable]
    public struct GravitySection : IConfigSection
    {
        public string Key => "gravity";
        public string DisplayName => "Character/Gravity";

        [ConfigKey("multiplier")]
        [SerializeField] private float _multiplier;

        [ConfigKey("fall_multiplier")]
        [SerializeField] private float _fallMultiplier;

        [ConfigKey("max_fall_speed")]
        [SerializeField] private float _maxFallSpeed;

        public static GravitySection Default => new GravitySection
        {
            _multiplier = 2f,
            _fallMultiplier = 2.5f,
            _maxFallSpeed = 20f
        };
    }
}
