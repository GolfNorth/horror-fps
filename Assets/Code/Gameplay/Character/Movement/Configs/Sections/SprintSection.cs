using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Movement.Configs.Sections
{
    [Serializable]
    public struct SprintSection : IConfigSection
    {
        public string Key => "sprint";
        public string DisplayName => "Character/Sprint";

        [ConfigKey("speed")]
        [SerializeField] private float _speed;

        [ConfigKey("acceleration")]
        [SerializeField] private float _acceleration;

        public static SprintSection Default => new SprintSection
        {
            _speed = 8f,
            _acceleration = 15f
        };
    }
}
