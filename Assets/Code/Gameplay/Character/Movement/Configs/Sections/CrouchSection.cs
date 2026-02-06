using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Movement.Configs.Sections
{
    [Serializable]
    public struct CrouchSection : IConfigSection
    {
        public string Key => "crouch";
        public string DisplayName => "Character/Crouch";

        [ConfigKey("speed")]
        [SerializeField] private float _speed;

        [ConfigKey("deceleration")]
        [SerializeField] private float _deceleration;

        [ConfigKey("height_ratio")]
        [SerializeField, Range(0.3f, 0.9f)] private float _heightRatio;

        public static CrouchSection Default => new CrouchSection()
        {
            _speed = 2.5f,
            _deceleration = 20f,
            _heightRatio = 0.5f
        };
    }
}
