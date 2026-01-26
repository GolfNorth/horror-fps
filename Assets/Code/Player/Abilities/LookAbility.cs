using Game.Input;
using Game.Player.Configs;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game.Player.Abilities
{
    public class LookAbility : PlayerAbilityBase
    {
        public override PlayerAbilityType Type => PlayerAbilityType.Look;

        [SerializeField] private CinemachinePanTilt _panTilt;

        private PlayerLookConfig _config;
        private IPlayerInput _input;
        private Transform _bodyTransform;

        private float _yaw;

        public float Pitch => _panTilt != null ? _panTilt.TiltAxis.Value : 0f;
        public float Yaw => _yaw;

        [Inject]
        public void Construct(PlayerLookConfig config, IPlayerInput input)
        {
            _config = config;
            _input = input;
        }

        public override void Initialize()
        {
            _bodyTransform = transform;

            if (_panTilt == null)
            {
                var virtualCamera = GetComponentInChildren<CinemachineCamera>();
                if (virtualCamera != null)
                {
                    _panTilt = virtualCamera.GetComponent<CinemachinePanTilt>();
                }
            }

            if (_panTilt != null)
            {
                ConfigurePanTilt();
            }

            _yaw = _bodyTransform.eulerAngles.y;
            LockCursor();
        }

        private void ConfigurePanTilt()
        {
            _panTilt.PanAxis.Range = new Vector2(-180f, 180f);
            _panTilt.PanAxis.Wrap = true;
            _panTilt.PanAxis.Value = 0f;

            _panTilt.TiltAxis.Range = new Vector2(_config.MinPitch, _config.MaxPitch);
            _panTilt.TiltAxis.Wrap = false;
            _panTilt.TiltAxis.Value = 0f;
        }

        private void OnEnable()
        {
            LockCursor();
        }

        private void OnDisable()
        {
            UnlockCursor();
        }

        public override void Tick(float deltaTime)
        {
            var lookInput = _input.LookInput;

            // Body yaw rotation
            var yawDelta = lookInput.x * _config.HorizontalSensitivity;
            _yaw += yawDelta;
            _bodyTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);

            // Camera pitch via Cinemachine
            if (_panTilt != null)
            {
                var pitchDelta = -lookInput.y * _config.VerticalSensitivity;
                _panTilt.TiltAxis.Value = Mathf.Clamp(
                    _panTilt.TiltAxis.Value + pitchDelta,
                    _config.MinPitch,
                    _config.MaxPitch);
            }
        }

        private static void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void SetRotation(float yaw, float pitch)
        {
            _yaw = yaw;
            _bodyTransform.rotation = Quaternion.Euler(0f, _yaw, 0f);

            if (_panTilt != null)
            {
                _panTilt.TiltAxis.Value = Mathf.Clamp(pitch, _config.MinPitch, _config.MaxPitch);
            }
        }

        private void Reset()
        {
            var virtualCamera = GetComponentInChildren<CinemachineCamera>();
            if (virtualCamera != null)
            {
                _panTilt = virtualCamera.GetComponent<CinemachinePanTilt>();
            }
        }
    }
}
