using UnityEngine;

namespace Camera
{
    public abstract class RotationAroundPointBaseClass : MonoBehaviour
    {
        // TODO : Use curve for sensitivity setting
        
        [SerializeField] private Vector2 axisMultiplier;
        
        [SerializeField] private float yAxisAngleLimiter;

        [SerializeField] private bool invertXAxis;
        [SerializeField] private bool invertYAxis;
        
        private Vector3 _startPosition;
        private Vector3 _currentPosition;

        private Vector3 _lastMousePosition;
        private bool _canBeginMoving;

        protected abstract Vector3 GetCurrentPosition();

        protected abstract void SetPosition(Vector3 position);
        
        protected abstract bool IsActive();
        
        public virtual void Start()
        {
            _startPosition = GetCurrentPosition();
            _currentPosition = GetCurrentPosition();
        }
        
        private void FixedUpdate()
        {
            if (!IsActive()) return;

            if (Input.GetMouseButton(1))
            {
                if (_canBeginMoving)
                {
                    // _currentPosition = GetCurrentPosition();
                    
                    Vector2 delta = Input.mousePosition - _lastMousePosition;
                    var closetAxisVector = Utils.GetClosetAxisVector(_currentPosition);
                    
                    Debug.Log("Norm Pos: " + _currentPosition.normalized);
                    Debug.Log("Axis: " + closetAxisVector);

                    var xAxis = Vector3.zero;
                    var yAxis  = Vector3.zero;
                    
                    if (closetAxisVector.Equals(Vector3.up))
                    {
                        Debug.LogWarning("Reached Up position!");
                        xAxis = Vector3.forward;
                        yAxis = Vector3.right;
                    }
                    else if (closetAxisVector.Equals(Vector3.down))
                    {
                        Debug.LogWarning("Reached Down position!");
                        xAxis = Vector3.forward;
                        yAxis = Vector3.right;
                    }
                    else if (closetAxisVector.Equals(Vector3.left))
                    {
                        xAxis = Vector3.up;
                        yAxis = Vector3.back;
                    }
                    else if (closetAxisVector.Equals(Vector3.right))
                    {
                        xAxis = Vector3.up;
                        yAxis = Vector3.forward;
                    }
                    else if (closetAxisVector.Equals(Vector3.forward))
                    {
                        xAxis = Vector3.up;
                        yAxis = Vector3.left;
                    }
                    else if (closetAxisVector.Equals(Vector3.back))
                    {
                        xAxis = Vector3.up;
                        yAxis = Vector3.right;
                    }

                    var newCurrentPosition = Quaternion.AngleAxis(delta.x * Time.fixedDeltaTime * axisMultiplier.x 
                                                                  * (invertXAxis ? -1 : 1), xAxis) *
                                       _currentPosition;
                    newCurrentPosition =
                        Quaternion.AngleAxis(delta.y * Time.fixedDeltaTime * axisMultiplier.y * (invertYAxis ? -1 : 1), yAxis) *
                        newCurrentPosition;

                    var resultAngle =
                        Mathf.Abs(Vector3.SignedAngle(newCurrentPosition.normalized, Vector3.up, Vector3.forward)) - 90;

                    
                    if (Mathf.Abs(resultAngle) < yAxisAngleLimiter)
                    {
                        SetPosition(newCurrentPosition);
                        _currentPosition = newCurrentPosition;
                    }
                    
                }

                _lastMousePosition = Input.mousePosition;
                _canBeginMoving = true;
            }
            else
            {
                _canBeginMoving = false;
            }
        }
    }
}