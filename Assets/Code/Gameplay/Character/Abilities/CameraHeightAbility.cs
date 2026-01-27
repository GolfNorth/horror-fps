using KinematicCharacterController;
using UnityEngine;

namespace Game.Gameplay.Character.Abilities
{
    public class CameraHeightAbility : MonoBehaviour
    {
        [SerializeField] private KinematicCharacterMotor _motor;
        [SerializeField] private Transform _cameraTarget;

        [Header("Settings")]
        [SerializeField] private float _offsetFromTop = 0.2f;
        [SerializeField] private float _transitionSpeed = 10f;

        private float _currentHeight;

        private void Awake()
        {
            if (_motor != null)
            {
                _currentHeight = _motor.Capsule.height - _offsetFromTop;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTarget == null || _motor == null) return;

            var targetHeight = _motor.Capsule.height - _offsetFromTop;

            _currentHeight = Mathf.MoveTowards(
                _currentHeight,
                targetHeight,
                _transitionSpeed * Time.deltaTime);

            var pos = _cameraTarget.localPosition;
            pos.y = _currentHeight;
            _cameraTarget.localPosition = pos;
        }

        private void Reset()
        {
            _motor = GetComponent<KinematicCharacterMotor>();
        }
    }
}
