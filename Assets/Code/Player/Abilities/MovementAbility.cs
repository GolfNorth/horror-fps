using System;
using Game.Input;
using Game.Player.Configs;
using Game.Player.Motor;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game.Player.Abilities
{
    public class MovementAbility : PlayerAbilityBase
    {
        public override PlayerAbilityType Type => PlayerAbilityType.Movement | PlayerAbilityType.Jump | PlayerAbilityType.Sprint;

        [SerializeField] private PlayerMotor _motor;
        [SerializeField] private CinemachineCamera _virtualCamera;

        private PlayerMovementConfig _config;
        private IPlayerInput _input;
        private PlayerController _controller;
        private Transform _cameraTransform;

        private float _jumpBufferTimer;
        private bool _jumpConsumed;
        private IDisposable _subscriptions;

        public bool IsSprinting { get; private set; }
        public bool IsMoving => _input?.MoveInput.sqrMagnitude > 0.01f;

        [Inject]
        public void Construct(PlayerMovementConfig config, IPlayerInput input, PlayerController controller)
        {
            _config = config;
            _input = input;
            _controller = controller;
        }

        public override void Initialize()
        {
            if (_motor == null)
                _motor = GetComponent<PlayerMotor>();

            if (_virtualCamera == null)
                _virtualCamera = GetComponentInChildren<CinemachineCamera>();

            _cameraTransform = _virtualCamera != null ? _virtualCamera.transform : transform;

            var d = Disposable.CreateBuilder();
            _input.OnJump.Subscribe(_ => OnJumpInput()).AddTo(ref d);
            _motor.OnLanded.Subscribe(_ => _jumpConsumed = false).AddTo(ref d);
            _subscriptions = d.Build();
        }

        private void OnDestroy() => _subscriptions?.Dispose();

        public override void Tick(float deltaTime)
        {
            UpdateJumpBuffer(deltaTime);
            UpdateSprint();
            UpdateMovement();
            TryJump();
        }

        public override void OnBlocked() => _motor.SetTargetVelocity(Vector3.zero);

        private void UpdateJumpBuffer(float deltaTime)
        {
            if (_jumpBufferTimer > 0)
                _jumpBufferTimer -= deltaTime;
        }

        private void UpdateSprint()
        {
            var blocked = _controller.IsBlocked(PlayerAbilityType.Sprint);
            IsSprinting = !blocked && _input.IsSprinting && IsMoving && _motor.IsGrounded;
        }

        private void UpdateMovement()
        {
            var moveInput = _input.MoveInput;
            if (moveInput.sqrMagnitude < 0.01f)
            {
                _motor.SetTargetVelocity(Vector3.zero);
                return;
            }

            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            var direction = forward * moveInput.y + right * moveInput.x;
            var speed = IsSprinting ? _config.SprintSpeed : _config.WalkSpeed;
            _motor.SetTargetVelocity(direction * speed);
        }

        private void OnJumpInput()
        {
            _jumpBufferTimer = _config.JumpBufferTime;
            _jumpConsumed = false;
        }

        private void TryJump()
        {
            if (_jumpConsumed || _jumpBufferTimer <= 0) return;
            if (_controller.IsBlocked(PlayerAbilityType.Jump)) return;

            if (_motor.IsGrounded || _motor.WasRecentlyGrounded)
            {
                _motor.Jump(_config.JumpForce);
                _jumpConsumed = true;
                _jumpBufferTimer = 0;
            }
        }

        private void Reset()
        {
            _motor = GetComponent<PlayerMotor>();
            _virtualCamera = GetComponentInChildren<CinemachineCamera>();
        }
    }
}
