using Game.Core.Configuration;
using Game.Gameplay.Environment;
using KinematicCharacterController;
using UnityEngine;
using VContainer;

namespace Game.Gameplay.Character.Abilities
{
    public class LadderClimbAbility : MovementAbility
    {
        private enum ClimbingState
        {
            None,
            Anchoring,
            Climbing,
            DeAnchoring
        }

        public override int Priority => 50;

        [SerializeField] private CharacterIdProvider _idProvider;
        [SerializeField] private LayerMask _ladderLayer;

        private IConfigValue<float> _climbSpeed;
        private IConfigValue<float> _attachDistance;
        private IConfigValue<float> _detachJumpForce;
        private IConfigValue<float> _topExitOffset;
        private IConfigValue<float> _detectionRadius;
        private IConfigValue<float> _anchoringDuration;
        private IConfigValue<float> _edgeThreshold;
        private IConfigValue<float> _snapStrength;
        private IConfigValue<float> _topExitHeightOffset;

        private Ladder _currentLadder;
        private float _climbInput;
        private bool _wantsToJumpOff;
        private ClimbingState _climbingState;
        private bool _wasActive;

        // Anchoring
        private float _anchoringTimer;
        private Vector3 _anchoringStartPosition;
        private Vector3 _anchoringTargetPosition;

        private readonly Collider[] _overlapResults = new Collider[4];

        public bool IsClimbing => _climbingState != ClimbingState.None;

        public Ladder CurrentLadder => _currentLadder;

        /// <summary>
        /// Returns the yaw angle the player should face when climbing (toward the ladder).
        /// </summary>
        public float LadderFacingYaw => _currentLadder != null
            ? Quaternion.LookRotation(_currentLadder.Forward).eulerAngles.y
            : 0f;

        [Inject]
        public void Construct(IConfigService config)
        {
            var id = _idProvider.CharacterId;
            _climbSpeed = config.Observe<float>($"{id}.ladder.climb_speed");
            _attachDistance = config.Observe<float>($"{id}.ladder.attach_distance");
            _detachJumpForce = config.Observe<float>($"{id}.ladder.detach_jump_force");
            _topExitOffset = config.Observe<float>($"{id}.ladder.top_exit_offset");
            _detectionRadius = config.Observe<float>($"{id}.ladder.detection_radius");
            _anchoringDuration = config.Observe<float>($"{id}.ladder.anchoring_duration");
            _edgeThreshold = config.Observe<float>($"{id}.ladder.edge_threshold");
            _snapStrength = config.Observe<float>($"{id}.ladder.snap_strength");
            _topExitHeightOffset = config.Observe<float>($"{id}.ladder.top_exit_height_offset");
        }

        public void SetClimbInput(float input)
        {
            _climbInput = Mathf.Clamp(input, -1f, 1f);
        }

        public void RequestAttach()
        {
            if (_climbingState != ClimbingState.None) return;

            var ladder = FindNearestLadder(transform.position);
            if (ladder != null)
            {
                AttachToLadder(ladder);
            }
        }

        public void RequestDetach(bool withJump = false)
        {
            if (_climbingState == ClimbingState.None) return;

            if (withJump)
            {
                _wantsToJumpOff = true;
            }
            else if (_climbingState == ClimbingState.Climbing)
            {
                _climbingState = ClimbingState.None;
                _currentLadder = null;
                            }
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_climbSpeed == null) return false;

            var isActive = _climbingState != ClimbingState.None;

            // Handle climbing state transitions for motor settings
            if (isActive && !_wasActive)
            {
                motor.SetMovementCollisionsSolvingActivation(false);
                motor.SetGroundSolvingActivation(false);
            }
            else if (!isActive && _wasActive)
            {
                motor.SetMovementCollisionsSolvingActivation(true);
                motor.SetGroundSolvingActivation(true);
            }
            _wasActive = isActive;

            if (!isActive)
            {
                return false;
            }

            if (_currentLadder == null)
            {
                _climbingState = ClimbingState.None;
                return false;
            }

            // Handle jump off request
            if (_wantsToJumpOff)
            {
                _wantsToJumpOff = false;
                var jumpDir = (-_currentLadder.Forward + Vector3.up).normalized;
                currentVelocity = jumpDir * _detachJumpForce.Value;
                _climbingState = ClimbingState.None;
                _currentLadder = null;
                                motor.ForceUnground();
                return true;
            }

            switch (_climbingState)
            {
                case ClimbingState.Anchoring:
                    currentVelocity = UpdateAnchoring(motor, deltaTime);
                    break;

                case ClimbingState.Climbing:
                    currentVelocity = UpdateClimbing(motor, deltaTime);
                    break;

                case ClimbingState.DeAnchoring:
                    currentVelocity = UpdateDeAnchoring(motor, deltaTime);
                    break;
            }

            motor.ForceUnground();
            return true;
        }


        private Vector3 UpdateAnchoring(KinematicCharacterMotor motor, float deltaTime)
        {
            _anchoringTimer += deltaTime;
            var t = Mathf.Clamp01(_anchoringTimer / _anchoringDuration.Value);

            var targetPos = Vector3.Lerp(_anchoringStartPosition, _anchoringTargetPosition, t);
            var velocity = motor.GetVelocityForMovePosition(motor.TransientPosition, targetPos, deltaTime);

            if (_anchoringTimer >= _anchoringDuration.Value)
            {
                _climbingState = ClimbingState.Climbing;
            }

            return velocity;
        }

        private Vector3 UpdateClimbing(KinematicCharacterMotor motor, float deltaTime)
        {
            // Check exit conditions
            var position = motor.TransientPosition;

            if (_currentLadder.IsAtTop(position, _edgeThreshold.Value) && _climbInput > 0f)
            {
                StartDeAnchoringToTop();
                return Vector3.zero;
            }

            if (_currentLadder.IsAtBottom(position, _edgeThreshold.Value) && _climbInput < 0f)
            {
                _climbingState = ClimbingState.None;
                _currentLadder = null;
                                return Vector3.zero;
            }

            // Climb movement
            var climbVelocity = _currentLadder.transform.up * (_climbInput * _climbSpeed.Value);

            // Snap to ladder center
            var targetPosition = _currentLadder.GetClosestPointOnLadder(motor.TransientPosition);
            var ladderOffset = -_currentLadder.Forward * _attachDistance.Value;
            targetPosition += ladderOffset;

            var toTarget = targetPosition - motor.TransientPosition;
            toTarget.y = 0f;

            var snapVelocity = toTarget / deltaTime * _snapStrength.Value;
            return climbVelocity + snapVelocity;
        }

        private Vector3 UpdateDeAnchoring(KinematicCharacterMotor motor, float deltaTime)
        {
            _anchoringTimer += deltaTime;
            var t = Mathf.Clamp01(_anchoringTimer / _anchoringDuration.Value);

            var targetPos = Vector3.Lerp(_anchoringStartPosition, _anchoringTargetPosition, t);
            var velocity = motor.GetVelocityForMovePosition(motor.TransientPosition, targetPos, deltaTime);

            if (_anchoringTimer >= _anchoringDuration.Value)
            {
                _climbingState = ClimbingState.None;
                _currentLadder = null;
            }

            return velocity;
        }

        private void AttachToLadder(Ladder ladder)
        {
            _currentLadder = ladder;
            _climbInput = 0f;

            // Calculate target position on ladder
            var targetPos = ladder.GetClosestPointOnLadder(transform.position);
            targetPos += -ladder.Forward * _attachDistance.Value;

            StartAnchoring(targetPos);
        }

        private void StartAnchoring(Vector3 targetPosition)
        {
            _climbingState = ClimbingState.Anchoring;
            _anchoringTimer = 0f;
            _anchoringStartPosition = transform.position;
            _anchoringTargetPosition = targetPosition;
        }

        private void StartDeAnchoringToTop()
        {
            var exitPosition = _currentLadder.TopPoint + _currentLadder.Forward * _topExitOffset.Value;
            exitPosition.y = _currentLadder.TopPoint.y + _topExitHeightOffset.Value;

            _climbingState = ClimbingState.DeAnchoring;
            _anchoringTimer = 0f;
            _anchoringStartPosition = transform.position;
            _anchoringTargetPosition = exitPosition;
        }

        private Ladder FindNearestLadder(Vector3 position)
        {
            var count = Physics.OverlapSphereNonAlloc(
                position,
                _detectionRadius.Value,
                _overlapResults,
                _ladderLayer);

            Ladder nearest = null;
            var nearestDistance = float.MaxValue;

            for (var i = 0; i < count; i++)
            {
                var ladder = _overlapResults[i].GetComponentInParent<Ladder>();
                if (ladder == null) continue;

                var closestPoint = ladder.GetClosestPointOnLadder(position);
                var distance = Vector3.Distance(position, closestPoint);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = ladder;
                }
            }

            return nearest;
        }

        private void Reset()
        {
            _idProvider = GetComponentInParent<CharacterIdProvider>();
            _ladderLayer = LayerMask.GetMask("Ladder");
        }
    }
}
