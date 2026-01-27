using Game.Gameplay.Character.Abilities;
using Game.Gameplay.Player.Input;
using R3;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Player.Drivers
{
    public class PlayerLookDriver : MonoBehaviour
    {
        [SerializeField] private BodyRotationAbility _bodyRotation;
        [SerializeField] private CinemachinePanTilt _panTilt;

        [Header("Sensitivity")]
        [SerializeField] private float _horizontalSensitivity = 2f;
        [SerializeField] private float _verticalSensitivity = 2f;

        [Header("Pitch Constraints")]
        [SerializeField] private float _minPitch = -89f;
        [SerializeField] private float _maxPitch = 89f;

        private IPlayerInput _input;
        private float _yaw;

        public float Yaw => _yaw;
        public float Pitch => _panTilt != null ? _panTilt.TiltAxis.Value : 0f;

        [Inject]
        public void Construct(IPlayerInput input)
        {
            _input = input;

            //Observable.EveryUpdate().Subscribe(Tick).AddTo(this);
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
            if (_input == null) return;

            var lookInput = _input.LookInput;

            // Body yaw
            _yaw += lookInput.x * _horizontalSensitivity;
            _bodyRotation.SetTargetYaw(_yaw);

            // Camera pitch
            if (_panTilt != null)
            {
                var pitchDelta = -lookInput.y * _verticalSensitivity;
                _panTilt.TiltAxis.Value = Mathf.Clamp(
                    _panTilt.TiltAxis.Value + pitchDelta,
                    _minPitch,
                    _maxPitch);
            }
        }

        private void ConfigurePanTilt()
        {
            if (_panTilt == null) return;

            _panTilt.PanAxis.Range = new Vector2(-180f, 180f);
            _panTilt.PanAxis.Wrap = true;
            _panTilt.PanAxis.Value = 0f;

            _panTilt.TiltAxis.Range = new Vector2(_minPitch, _maxPitch);
            _panTilt.TiltAxis.Wrap = false;
            _panTilt.TiltAxis.Value = 0f;
        }

        public void SetRotation(float yaw, float pitch)
        {
            _yaw = yaw;
            _bodyRotation.SetTargetYaw(_yaw);

            if (_panTilt != null)
            {
                _panTilt.TiltAxis.Value = Mathf.Clamp(pitch, _minPitch, _maxPitch);
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

            var cam = GetComponentInChildren<CinemachineCamera>();
            if (cam != null)
            {
                _panTilt = cam.GetComponent<CinemachinePanTilt>();
            }
        }
    }
}
