using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Movement.Configs.Sections
{
    [Serializable]
    public struct MovementSection : IConfigSection
    {
        public string Key => "movement";
        public string DisplayName => "Character/Movement";

        [ConfigKey("walk_speed")]
        [SerializeField] private float _walkSpeed;

        [ConfigKey("acceleration")]
        [SerializeField] private float _acceleration;

        [ConfigKey("deceleration")]
        [SerializeField] private float _deceleration;

        [ConfigKey("air_acceleration")]
        [SerializeField] private float _airAcceleration;

        [ConfigKey("air_control")]
        [SerializeField] private float _airControl;

        public static MovementSection Default => new MovementSection()
        {
            _walkSpeed = 5f,
            _acceleration = 10f,
            _deceleration = 10f,
            _airAcceleration = 5f,
            _airControl = 0.3f
        };
    }
}
