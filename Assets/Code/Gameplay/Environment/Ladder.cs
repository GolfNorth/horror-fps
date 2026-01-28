using UnityEngine;

namespace Game.Gameplay.Environment
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] private Transform _bottomPoint;
        [SerializeField] private Transform _topPoint;
        [SerializeField] private float _width = 0.5f;

        public Vector3 BottomPoint => _bottomPoint != null ? _bottomPoint.position : transform.position;
        public Vector3 TopPoint => _topPoint != null ? _topPoint.position : transform.position + Vector3.up * 3f;
        public Vector3 Forward => transform.forward;
        public float Width => _width;

        public Vector3 GetClimbDirection()
        {
            return (TopPoint - BottomPoint).normalized;
        }

        public float GetHeight()
        {
            return Vector3.Distance(BottomPoint, TopPoint);
        }

        public Vector3 GetClosestPointOnLadder(Vector3 position)
        {
            var bottom = BottomPoint;
            var top = TopPoint;
            var ladderDir = (top - bottom).normalized;
            var toPosition = position - bottom;
            var projection = Vector3.Dot(toPosition, ladderDir);
            projection = Mathf.Clamp(projection, 0f, GetHeight());
            return bottom + ladderDir * projection;
        }

        public float GetProgressOnLadder(Vector3 position)
        {
            var closest = GetClosestPointOnLadder(position);
            return Vector3.Distance(BottomPoint, closest) / GetHeight();
        }

        public bool IsAtTop(Vector3 position, float threshold = 0.1f)
        {
            return GetProgressOnLadder(position) >= 1f - threshold;
        }

        public bool IsAtBottom(Vector3 position, float threshold = 0.1f)
        {
            return GetProgressOnLadder(position) <= threshold;
        }

        private void OnDrawGizmosSelected()
        {
            var bottom = BottomPoint;
            var top = TopPoint;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(bottom, top);
            Gizmos.DrawWireSphere(bottom, 0.1f);
            Gizmos.DrawWireSphere(top, 0.1f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(bottom, Forward * 0.5f);
            Gizmos.DrawRay(top, Forward * 0.5f);
        }

        private void Reset()
        {
            if (_bottomPoint == null)
            {
                var bottomGo = new GameObject("BottomPoint");
                bottomGo.transform.SetParent(transform);
                bottomGo.transform.localPosition = Vector3.zero;
                _bottomPoint = bottomGo.transform;
            }

            if (_topPoint == null)
            {
                var topGo = new GameObject("TopPoint");
                topGo.transform.SetParent(transform);
                topGo.transform.localPosition = Vector3.up * 3f;
                _topPoint = topGo.transform;
            }
        }
    }
}
