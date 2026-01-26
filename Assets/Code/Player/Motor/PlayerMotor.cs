using Game.Player.Configs;
using R3;
using UnityEngine;

namespace Game.Player.Motor
{
    /// <summary>
    /// Low-level physics-based motor for player movement.
    /// Handles Rigidbody interactions, ground detection, and velocity control.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMotor : MonoBehaviour
    {
        private Rigidbody _rigidbody;
        private CapsuleCollider _collider;
        private PlayerMovementConfig _config;

        private Vector3 _targetVelocity;
        private bool _isGrounded;
        private float _lastGroundedTime;
        private RaycastHit _groundHit;

        private readonly Subject<Unit> _onLanded = new();
        private readonly Subject<Unit> _onLeftGround = new();

        public bool IsGrounded => _isGrounded;
        public bool WasRecentlyGrounded => Time.time - _lastGroundedTime < (_config?.CoyoteTime ?? 0.15f);
        public Vector3 Velocity => _rigidbody.linearVelocity;
        public Vector3 GroundNormal => _isGrounded ? _groundHit.normal : Vector3.up;

        public Observable<Unit> OnLanded => _onLanded;
        public Observable<Unit> OnLeftGround => _onLeftGround;

        public void Initialize(PlayerMovementConfig config)
        {
            _config = config;
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<CapsuleCollider>();

            ConfigureRigidbody();
        }

        private void OnDestroy()
        {
            _onLanded.Dispose();
            _onLeftGround.Dispose();
        }

        private void ConfigureRigidbody()
        {
            _rigidbody.useGravity = false;
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            if (_config == null) return;

            UpdateGroundCheck();
            ApplyGravity();
            ApplyMovement();
        }

        private void UpdateGroundCheck()
        {
            var wasGrounded = _isGrounded;
            var origin = transform.position + Vector3.up * _collider.radius;
            var distance = _collider.radius + _config.GroundCheckDistance;

            _isGrounded = Physics.SphereCast(
                origin,
                _collider.radius * 0.9f,
                Vector3.down,
                out _groundHit,
                distance,
                _config.GroundLayer,
                QueryTriggerInteraction.Ignore);

            if (_isGrounded)
            {
                _lastGroundedTime = Time.time;

                if (!wasGrounded)
                {
                    _onLanded.OnNext(Unit.Default);
                }
            }
            else if (wasGrounded)
            {
                _onLeftGround.OnNext(Unit.Default);
            }
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _rigidbody.linearVelocity.y <= 0)
            {
                return;
            }

            var gravity = Physics.gravity;
            var multiplier = _rigidbody.linearVelocity.y < 0
                ? _config.FallMultiplier
                : _config.GravityMultiplier;

            var gravityForce = gravity * (multiplier - 1f);
            _rigidbody.AddForce(gravityForce, ForceMode.Acceleration);

            if (_rigidbody.linearVelocity.y < -_config.MaxFallSpeed)
            {
                var velocity = _rigidbody.linearVelocity;
                velocity.y = -_config.MaxFallSpeed;
                _rigidbody.linearVelocity = velocity;
            }
        }

        private void ApplyMovement()
        {
            var currentVelocity = _rigidbody.linearVelocity;
            var horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            var targetHorizontal = new Vector3(_targetVelocity.x, 0f, _targetVelocity.z);

            float acceleration;
            if (_isGrounded)
            {
                acceleration = targetHorizontal.magnitude > 0.01f
                    ? _config.Acceleration
                    : _config.Deceleration;
            }
            else
            {
                acceleration = _config.AirAcceleration * _config.AirControl;
            }

            var newHorizontal = Vector3.MoveTowards(
                horizontalVelocity,
                targetHorizontal,
                acceleration * Time.fixedDeltaTime);

            _rigidbody.linearVelocity = new Vector3(newHorizontal.x, currentVelocity.y, newHorizontal.z);
        }

        public void SetTargetVelocity(Vector3 velocity)
        {
            _targetVelocity = velocity;
        }

        public void Jump(float force)
        {
            var velocity = _rigidbody.linearVelocity;
            velocity.y = force;
            _rigidbody.linearVelocity = velocity;
        }

        public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            _rigidbody.AddForce(force, mode);
        }

        public void SetVelocity(Vector3 velocity)
        {
            _rigidbody.linearVelocity = velocity;
        }
    }
}
