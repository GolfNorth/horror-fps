using System;
using Game.Core.Logging;
using Game.Gameplay.Player.Input.Intents;
using Game.Input;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Player.Input
{
    /// <summary>
    /// Reads Unity Input System and writes intents to buffer.
    /// </summary>
    public sealed class PlayerInputService : IPlayerInput, ITickable, IInitializable, IDisposable
    {
        private const string LogTag = "PlayerInput";

        private readonly ILogService _log;
        private readonly IIntentBuffer _buffer;
        private InputSystem_Actions _inputActions;
        private bool _isEnabled;

        public bool IsEnabled => _isEnabled;

        [Inject]
        public PlayerInputService(ILogService log, IIntentBuffer buffer)
        {
            _log = log;
            _buffer = buffer;
        }

        public void Initialize()
        {
            _inputActions = new InputSystem_Actions();
            _inputActions.Enable();
            _isEnabled = true;

            SubscribeToEvents();

            _log.Info(LogTag, "Initialized");
        }

        public void Tick()
        {
            if (!_isEnabled) return;

            UpdateContinuousIntents();
        }

        public void Enable()
        {
            _inputActions?.Player.Enable();
            _isEnabled = true;
        }

        public void Disable()
        {
            _inputActions?.Player.Disable();
            _buffer.Clear();
            _isEnabled = false;
        }

        public void Dispose()
        {
            _inputActions?.Disable();
            _inputActions?.Dispose();
        }

        private void SubscribeToEvents()
        {
            _inputActions.Player.Jump.performed += _ => _buffer.Set(new JumpIntent());
            _inputActions.Player.Jump.canceled += _ => _buffer.Remove<JumpIntent>();

            _inputActions.Player.Attack.performed += _ => _buffer.Set(new AttackIntent());
            _inputActions.Player.Attack.canceled += _ => _buffer.Remove<AttackIntent>();

            _inputActions.Player.Interact.performed += _ => _buffer.Set(new InteractIntent());
            _inputActions.Player.Interact.canceled += _ => _buffer.Remove<InteractIntent>();

            _inputActions.Player.Sprint.started += _ => _buffer.Set(new SprintIntent());
            _inputActions.Player.Sprint.canceled += _ => _buffer.Remove<SprintIntent>();

            _inputActions.Player.Crouch.started += _ => _buffer.Set(new CrouchIntent());
            _inputActions.Player.Crouch.canceled += _ => _buffer.Remove<CrouchIntent>();
        }

        private void UpdateContinuousIntents()
        {
            var moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
            if (moveInput != Vector2.zero)
            {
                _buffer.Set(new MoveIntent(moveInput));
            }
            else
            {
                _buffer.Remove<MoveIntent>();
            }

            var lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
            if (lookInput != Vector2.zero)
            {
                _buffer.Set(new LookIntent(lookInput));
            }
            else
            {
                _buffer.Remove<LookIntent>();
            }
        }
    }
}
