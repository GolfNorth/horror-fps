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
        [SerializeField] private LadderClimbAbility _ladderClimb;

        private IPlayerInput _input;
        private IConfigValue<float> _horizontalSensitivity;
        private IConfigValue<float> _verticalSensitivity;
        private IConfigValue<float> _minPitch;
        private IConfigValue<float> _maxPitch;
        private IConfigValue<float> _ladderMaxYawDeviation;

        private bool _wasClimbing;
        private float _ladderCenteringProgress;

        public float Yaw => _bodyRotation != null ? _bodyRotation.TargetYaw : 0f;
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
            _ladderMaxYawDeviation = config.Observe<float>($"{id}.ladder.max_look_yaw_deviation");
        }

        private void Start()
        {
            _bodyRotation.SetYawImmediate(transform.eulerAngles.y);
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
            if (_input == null || _horizontalSensitivity == null || _bodyRotation == null) return;

            var lookInput = _input.LookInput;

            // Body yaw - add delta to current target
            _bodyRotation.AddYaw(lookInput.x * _horizontalSensitivity.Value);

            // Handle ladder climbing rotation
            if (_ladderClimb != null && _ladderMaxYawDeviation != null)
            {
                var isClimbing = _ladderClimb.IsClimbing;

                // Detect climb start - reset centering
                if (isClimbing && !_wasClimbing)
                {
                    _ladderCenteringProgress = 0f;
                }
                _wasClimbing = isClimbing;

                if (isClimbing)
                {
                    var ladderYaw = _ladderClimb.LadderFacingYaw;
                    var currentYaw = _bodyRotation.TargetYaw;
                    var maxDeviation = _ladderMaxYawDeviation.Value;

                    // Smooth centering when starting to climb
                    const float centeringDuration = 0.3f;
                    if (_ladderCenteringProgress < 1f)
                    {
                        _ladderCenteringProgress += Time.deltaTime / centeringDuration;
                        var centeringStrength = 1f - Mathf.Clamp01(_ladderCenteringProgress);
                        currentYaw = Mathf.LerpAngle(currentYaw, ladderYaw, centeringStrength * 8f * Time.deltaTime);
                        _bodyRotation.TargetYaw = currentYaw;
                    }

                    // Clamp to allowed range
                    var deltaAngle = Mathf.DeltaAngle(ladderYaw, currentYaw);
                    var clampedDelta = Mathf.Clamp(deltaAngle, -maxDeviation, maxDeviation);
                    _bodyRotation.TargetYaw = ladderYaw + clampedDelta;
                }
            }

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
            _bodyRotation.SetYawImmediate(yaw);

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
            _ladderClimb = GetComponent<LadderClimbAbility>();

            var cam = GetComponentInChildren<CinemachineCamera>();
            if (cam != null)
            {
                _panTilt = cam.GetComponent<CinemachinePanTilt>();
            }
        }
    }
}
