using System;
using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Configs.Sections
{
    [Serializable]
    public struct LadderSection : IConfigSection
    {
        public string Key => "ladder";
        public string DisplayName => "Character/Ladder";

        [ConfigKey("climb_speed")]
        [SerializeField] private float _climbSpeed;

        [ConfigKey("attach_distance")]
        [SerializeField] private float _attachDistance;

        [ConfigKey("detach_jump_force")]
        [SerializeField] private float _detachJumpForce;

        [ConfigKey("top_exit_offset")]
        [SerializeField] private float _topExitOffset;

        [ConfigKey("max_look_yaw_deviation")]
        [SerializeField] private float _maxLookYawDeviation;

        [ConfigKey("detection_radius")]
        [SerializeField] private float _detectionRadius;

        [ConfigKey("anchoring_duration")]
        [SerializeField] private float _anchoringDuration;

        [ConfigKey("edge_threshold")]
        [SerializeField] private float _edgeThreshold;

        [ConfigKey("snap_strength")]
        [SerializeField] private float _snapStrength;

        [ConfigKey("top_exit_height_offset")]
        [SerializeField] private float _topExitHeightOffset;

        public static LadderSection Default => new LadderSection()
        {
            _climbSpeed = 3f,
            _attachDistance = 0.5f,
            _detachJumpForce = 5f,
            _topExitOffset = 0.5f,
            _maxLookYawDeviation = 60f,
            _detectionRadius = 1f,
            _anchoringDuration = 0.2f,
            _edgeThreshold = 0.05f,
            _snapStrength = 0.5f,
            _topExitHeightOffset = 0.1f
        };
    }
}
