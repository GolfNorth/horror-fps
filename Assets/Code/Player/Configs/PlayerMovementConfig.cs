using Game.Core.Configuration;
using UnityEngine;

namespace Game.Player.Configs
{
    /// <summary>
    /// Configuration for player movement parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMovementConfig", menuName = "Game/Player/Movement Config")]
    public class PlayerMovementConfig : GameConfig
    {
        [Header("Ground Movement")]
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 8f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _deceleration = 10f;

        [Header("Air Movement")]
        [SerializeField] private float _airControl = 0.3f;
        [SerializeField] private float _airAcceleration = 5f;

        [Header("Jump")]
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _coyoteTime = 0.15f;
        [SerializeField] private float _jumpBufferTime = 0.1f;

        [Header("Gravity")]
        [SerializeField] private float _gravityMultiplier = 2f;
        [SerializeField] private float _fallMultiplier = 2.5f;
        [SerializeField] private float _maxFallSpeed = 20f;

        [Header("Ground Check")]
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private LayerMask _groundLayer = ~0;

        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float Acceleration => _acceleration;
        public float Deceleration => _deceleration;
        public float AirControl => _airControl;
        public float AirAcceleration => _airAcceleration;
        public float JumpForce => _jumpForce;
        public float CoyoteTime => _coyoteTime;
        public float JumpBufferTime => _jumpBufferTime;
        public float GravityMultiplier => _gravityMultiplier;
        public float FallMultiplier => _fallMultiplier;
        public float MaxFallSpeed => _maxFallSpeed;
        public float GroundCheckDistance => _groundCheckDistance;
        public LayerMask GroundLayer => _groundLayer;
    }
}
