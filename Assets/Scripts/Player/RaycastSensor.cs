using UnityEngine;

namespace Player
{
    public class RaycastSensor
    {
        public enum CastDir { Forward, Right, Up, Backward, Left, Down }
        
        public float CastLength = 1f;
        public LayerMask Layermask = 255;
        
        private Vector3 _origin = Vector3.zero;
        private Transform _tr;
        private CastDir _castDir;
        private RaycastHit _hitInfo;

        public RaycastSensor(Transform playerTransform)
        {
            _tr = playerTransform;
        }
        
        public float Distance => _hitInfo.distance;
        public Vector3 Normal => _hitInfo.normal;
        public Vector3 Position => _hitInfo.point;
        public Collider Collider => _hitInfo.collider;
        public Transform Transform => _hitInfo.transform;

        public void SetCastDir(CastDir dir) => _castDir = dir;
        public void SetCastOrigin(Vector3 pos) => _origin = _tr.InverseTransformPoint(pos);  // inverseTransformPoint - world to local. why?

        public void Cast()
        {
            Vector3 worldOrigin = _tr.TransformPoint(_origin);  // TransformPoint - local to world. why?
            Vector3 worldDir = GetCastDir();

            Physics.Raycast(worldOrigin, worldDir, out _hitInfo, CastLength, Layermask, QueryTriggerInteraction.Ignore);
        }

        public bool HasDetectedHit() => _hitInfo.collider != null;

        private Vector3 GetCastDir()
        {
            return _castDir switch
            {
                CastDir.Forward => _tr.forward,
                CastDir.Right => _tr.right,
                CastDir.Up => _tr.up,
                CastDir.Backward => -_tr.forward,
                CastDir.Left => -_tr.right,
                CastDir.Down => -_tr.up,
                _ => Vector3.one
            };
        }
    }
}

////// examples
// /private void RecalibrateSensor()
// {
//     _sensor ??= new RaycastSensor(_tr);
//             
//     _sensor.SetCastOrigin(_col.bounds.center);
//     _sensor.SetCastDir(RaycastSensor.CastDir.Down);
//     RecalculateSensorLayerMask();
//
//     // ?????????
//     const float safetyDistanceFactor = 0.001f; // small factor added to prevent clipping issues when the sensor range is calculated
//
//     // ??????????????????????????????????????????????????????????????????????????
//     float length = colliderHeight * (1f - stepHeightRation) * 0.5f + colliderHeight * stepHeightRation;
//     _baseSensorRange = length * (1f + safetyDistanceFactor) * _tr.localScale.x;
//     _sensor.CastLength = length * _tr.localScale.x;
// }

// public void CheckForGround()
// {
//     if (_currentLayer != gameObject.layer)
//     {
//         RecalculateSensorLayerMask();
//     }
//             
//     _currentGroundAdjustmentVelocity = Vector3.zero;
//     // ?????????
//     _sensor.CastLength = _isUsingExtendedSensorRange
//         ? _baseSensorRange + colliderHeight * _tr.localScale.x * stepHeightRation
//         : _baseSensorRange;
//     _sensor.Cast();
//
//     _isGrounded = _sensor.HasDetectedHit();
//     if (!_isGrounded) return;
//
//     float distance = _sensor.Distance;
//     float upperLimit = colliderHeight * _tr.localScale.x * (1f - stepHeightRation) * 0.5f;  // ограничение на высоту? макс высота - где он должен быть?
//     float middle = upperLimit + colliderHeight * _tr.localScale.x * stepHeightRation;  // макс высота с учетом stepHeightRation
//     float distanceToGo = middle - distance;  // разница между тем где игрок сейчас и где он должен быть
//
//     _currentGroundAdjustmentVelocity = _tr.up * (distanceToGo / Time.fixedDeltaTime);
// }