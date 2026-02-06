using Game.Core.Configuration;
using Game.Core.Ticking;
using Game.Gameplay.Character.Actions;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class CameraPitchAbility : TickableBehaviour, IInitializable, ILateTickable
    {
        [SerializeField] private CinemachinePanTilt _panTilt;

        private IActionBuffer _actions;
        private IConfigService _config;
        private IConfigValue<float> _minPitch;
        private IConfigValue<float> _maxPitch;

        private float _pitch;

        [Inject]
        public void Construct(IActionBuffer actions, IConfigService config)
        {
            _actions = actions;
            _config = config;
        }

        public void Initialize()
        {
            _minPitch = _config.Observe<float>("look.min_pitch");
            _maxPitch = _config.Observe<float>("look.max_pitch");
        }

        public void LateTick()
        {
            if (_panTilt == null || _minPitch == null) return;

            if (_actions.TryGet<LookAction>(out var look))
            {
                _pitch -= look.Delta.y;
                _pitch = Mathf.Clamp(_pitch, _minPitch.Value, _maxPitch.Value);
            }

            _panTilt.TiltAxis.Value = _pitch;
        }

        private void Reset()
        {
            _panTilt = GetComponentInChildren<CinemachinePanTilt>();
        }
    }
}
