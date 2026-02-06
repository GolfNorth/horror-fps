using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Movement.Configs.Sections
{
    [Serializable]
    public struct JumpSection : IConfigSection
    {
        public string Key => "jump";
        public string Label => "Character/Jump";

        [ConfigKey("force")]
        [SerializeField] private float _force;

        [ConfigKey("coyote_time")]
        [SerializeField] private float _coyoteTime;

        [ConfigKey("buffer_time")]
        [SerializeField] private float _bufferTime;

        public static JumpSection Default => new JumpSection
        {
            _force = 7f,
            _coyoteTime = 0.15f,
            _bufferTime = 0.1f
        };
    }
}
