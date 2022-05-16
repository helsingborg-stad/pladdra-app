using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Utility
{
    public class TransformChangedHandler : MonoBehaviour
    {
        private Vector3 _lastPosition = Vector3.zero;
        private PositionChangedEvent _onPositionChanged;

        public class PositionChangedEvent: UnityEvent<Vector3> { }

        public PositionChangedEvent OnPositionChanged
        {
            get
            {
                if (_onPositionChanged == null)
                {
                    _onPositionChanged = new PositionChangedEvent();
                }
                return _onPositionChanged;
            }
        }
        private void Update()
        {
            if (transform.localPosition != _lastPosition)
            {
                _lastPosition = transform.localPosition;
                _onPositionChanged?.Invoke(transform.localPosition);
            }
        }
    }
}