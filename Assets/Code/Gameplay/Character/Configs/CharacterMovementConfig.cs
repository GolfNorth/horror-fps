using Game.Core.Configuration;
using UnityEngine;

namespace Game.Gameplay.Character.Configs
{
    [CreateAssetMenu(fileName = "CharacterMovementConfig", menuName = "Game/Character/Movement Config")]
    public class CharacterMovementConfig : GameConfig
    {
        [Header("Walking")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 10f;

        [Header("Air Control")]
        [SerializeField] private float _airAcceleration = 5f;
        [SerializeField] private float _airControl = 0.3f;

        [Header("Sprint")]
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _sprintAcceleration = 15f;

        [Header("Crouch")]
        [SerializeField] private float _crouchSpeed = 2.5f;
        [SerializeField] private float _crouchDeceleration = 20f;
        [SerializeField, Range(0.3f, 0.9f)] private float _crouchHeightRatio = 0.5f;

        [Header("Jump")]
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _coyoteTime = 0.15f;
        [SerializeField] private float _jumpBufferTime = 0.1f;

        [Header("Gravity")]
        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _maxFallSpeed = 20f;

        // Walking
        public float WalkSpeed => _walkSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;

        // Air
        public float AirAcceleration => _airAcceleration;
        public float AirControl => _airControl;

        // Sprint
        public float SprintSpeed => _sprintSpeed;
        public float SprintAcceleration => _sprintAcceleration;

        // Crouch
        public float CrouchSpeed => _crouchSpeed;
        public float CrouchDeceleration => _crouchDeceleration;
        public float CrouchHeightRatio => _crouchHeightRatio;

        // Jump
        public float JumpForce => _jumpForce;
        public float CoyoteTime => _coyoteTime;
        public float JumpBufferTime => _jumpBufferTime;

        // Gravity
        public float GravityMultiplier => _gravityMultiplier;
        public float FallMultiplier => _fallMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;
    }
}
