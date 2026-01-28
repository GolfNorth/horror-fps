using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Player.Configs.Sections
{
    [Serializable]
    public struct LookSection : IConfigSection
    {
        public string Key => "look";
        public string DisplayName => "Player/Look";

        [ConfigKey("horizontal_sensitivity")]
        [SerializeField] private float _horizontalSensitivity;

        [ConfigKey("vertical_sensitivity")]
        [SerializeField] private float _verticalSensitivity;

        [ConfigKey("min_pitch")]
        [SerializeField] private float _minPitch;

        [ConfigKey("max_pitch")]
        [SerializeField] private float _maxPitch;

        public static LookSection Default => new LookSection()
        {
            _horizontalSensitivity = 2f,
            _verticalSensitivity = 2f,
            _minPitch = -89f,
            _maxPitch = 89f
        };
    }
}
