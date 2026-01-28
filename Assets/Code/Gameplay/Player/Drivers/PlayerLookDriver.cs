using Game.Core.Configuration;
using Game.Gameplay.Character;
using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerLookDriver : MonoBehaviour
    {
        [SerializeField] private BodyRotationAbility _bodyRotation;
        [SerializeField] private CinemachinePanTilt _panTilt;
        [SerializeField] private CharacterIdProvider _idProvider;

        private IPlayerInput _input;
        private IConfigValue<float> _horizontalSensitivity;
        private IConfigValue<float> _verticalSensitivity;
        private IConfigValue<float> _minPitch;
        private IConfigValue<float> _maxPitch;

        private float _yaw;

        public float Yaw => _yaw;
        public float Pitch => _panTilt != null ? _panTilt.TiltAxis.Value : 0f;

        [Inject]
        public void Construct(IPlayerInput input, IConfigService config)
        {
            _input = input;

            var id = _idProvider.CharacterId;
            _horizontalSensitivity = config.Observe<float>($"{id}.look.horizontal_sensitivity");
            _verticalSensitivity = config.Observe<float>($"{id}.look.vertical_sensitivity");
            _minPitch = config.Observe<float>($"{id}.look.min_pitch");
            _maxPitch = config.Observe<float>($"{id}.look.max_pitch");
        }

        private void Start()
        {
            _yaw = transform.eulerAngles.y;
            ConfigurePanTilt();
            LockCursor();
        }

        private void OnEnable()
        {
            LockCursor();
        }

        private void OnDisable()
        {
            UnlockCursor();
        }

        private void Update()
        {
            if (_input == null || _horizontalSensitivity == null) return;

            var lookInput = _input.LookInput;

            // Body yaw
            _yaw += lookInput.x * _horizontalSensitivity.Value;
            _bodyRotation.SetTargetYaw(_yaw);

            // Camera pitch
            if (_panTilt != null)
            {
                var pitchDelta = -lookInput.y * _verticalSensitivity.Value;
                _panTilt.TiltAxis.Value = Mathf.Clamp(
                    _panTilt.TiltAxis.Value + pitchDelta,
                    _minPitch.Value,
                    _maxPitch.Value);
            }
        }

        private void ConfigurePanTilt()
        {
            if (_panTilt == null || _minPitch == null) return;

            _panTilt.PanAxis.Range = new Vector2(-180f, 180f);
            _panTilt.PanAxis.Wrap = true;
            _panTilt.PanAxis.Value = 0f;

            _panTilt.TiltAxis.Range = new Vector2(_minPitch.Value, _maxPitch.Value);
            _panTilt.TiltAxis.Wrap = false;
            _panTilt.TiltAxis.Value = 0f;
        }

        public void SetRotation(float yaw, float pitch)
        {
            _yaw = yaw;
            _bodyRotation.SetTargetYaw(_yaw);

            if (_panTilt != null && _minPitch != null)
            {
                _panTilt.TiltAxis.Value = Mathf.Clamp(pitch, _minPitch.Value, _maxPitch.Value);
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

        private void Reset()
        {
            _bodyRotation = GetComponent<BodyRotationAbility>();
            _idProvider = GetComponentInParent<CharacterIdProvider>();

            var cam = GetComponentInChildren<CinemachineCamera>();
            if (cam != null)
            {
                _panTilt = cam.GetComponent<CinemachinePanTilt>();
            }
        }
    }
}
