using Game.Core.Configuration;
using Game.Gameplay.Character.Actions;
using Game.Gameplay.Environment;
using KinematicCharacterController;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay.Character.Movement.Abilities
{
    public class LadderClimbAbility : MovementAbility, IInitializable
    {
        private enum ClimbingState
        {
            None,
            Anchoring,
            Climbing,
            DeAnchoring
        }

        public override int Priority => 50;

        [SerializeField] private LayerMask _ladderLayer;

        private IActionBuffer _actions;
        private IConfigService _config;
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
        private ClimbingState _climbingState;
        private bool _wasActive;

        // Anchoring
        private float _anchoringTimer;
        private Vector3 _anchoringStartPosition;
        private Vector3 _anchoringTargetPosition;

        private readonly Collider[] _overlapResults = new Collider[4];

        public bool IsClimbing => _climbingState != ClimbingState.None;

        public Ladder CurrentLadder => _currentLadder;

        public float LadderFacingYaw => _currentLadder != null
            ? Quaternion.LookRotation(_currentLadder.Forward).eulerAngles.y
            : 0f;

        [Inject]
        public void Construct(IConfigService config, IActionBuffer actions)
        {
            _config = config;
            _actions = actions;
        }

        public void Initialize()
        {
            _climbSpeed = _config.Observe<float>("ladder.climb_speed");
            _attachDistance = _config.Observe<float>("ladder.attach_distance");
            _detachJumpForce = _config.Observe<float>("ladder.detach_jump_force");
            _topExitOffset = _config.Observe<float>("ladder.top_exit_offset");
            _detectionRadius = _config.Observe<float>("ladder.detection_radius");
            _anchoringDuration = _config.Observe<float>("ladder.anchoring_duration");
            _edgeThreshold = _config.Observe<float>("ladder.edge_threshold");
            _snapStrength = _config.Observe<float>("ladder.snap_strength");
            _topExitHeightOffset = _config.Observe<float>("ladder.top_exit_height_offset");
        }

        public override bool UpdateVelocity(
            KinematicCharacterMotor motor,
            ref Vector3 currentVelocity,
            float deltaTime)
        {
            if (_climbSpeed == null) return false;

            // Try to attach when not climbing and interact action is present
            if (_climbingState == ClimbingState.None && _actions.Has<InteractAction>())
            {
                var ladder = FindNearestLadder(transform.position);
                if (ladder != null)
                {
                    AttachToLadder(ladder);
                }
            }

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

            // Jump off ladder
            if (_actions.Has<JumpAction>())
            {
                var jumpDir = (-_currentLadder.Forward + Vector3.up).normalized;
                currentVelocity = jumpDir * _detachJumpForce.Value;
                _climbingState = ClimbingState.None;
                _currentLadder = null;
                motor.ForceUnground();
                return true;
            }

            // Read climb input from move action Y axis
            var climbInput = 0f;
            if (_actions.TryGet<MoveAction>(out var move))
            {
                climbInput = Mathf.Clamp(move.Direction.y, -1f, 1f);
            }

            switch (_climbingState)
            {
                case ClimbingState.Anchoring:
                    currentVelocity = UpdateAnchoring(motor, deltaTime);
                    break;

                case ClimbingState.Climbing:
                    currentVelocity = UpdateClimbing(motor, climbInput, deltaTime);
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

        private Vector3 UpdateClimbing(KinematicCharacterMotor motor, float climbInput, float deltaTime)
        {
            var position = motor.TransientPosition;

            if (_currentLadder.IsAtTop(position, _edgeThreshold.Value) && climbInput > 0f)
            {
                StartDeAnchoringToTop();
                return Vector3.zero;
            }

            if (_currentLadder.IsAtBottom(position, _edgeThreshold.Value) && climbInput < 0f)
            {
                _climbingState = ClimbingState.None;
                _currentLadder = null;
                return Vector3.zero;
            }

            var climbVelocity = _currentLadder.transform.up * (climbInput * _climbSpeed.Value);

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
            _ladderLayer = LayerMask.GetMask("Ladder");
        }
    }
}
