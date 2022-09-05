using UnityEngine;

/*
The MIT License (MIT)

Copyright (c) 2021 Ryan Vazquez

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

 */

namespace Plane
{
    public class PlanePhysicsController : MonoBehaviour
    {
        [SerializeField] private float maxThrust;
        [SerializeField] private float throttleSpeedChange;

        [Header("Drag")] 
        [SerializeField] private AnimationCurve dragForward;
        [SerializeField] private AnimationCurve dragBackwards;
        [SerializeField] private AnimationCurve dragLeft;
        [SerializeField] private AnimationCurve dragRight;
        [SerializeField] private AnimationCurve dragTop;
        [SerializeField] private AnimationCurve dragBottom;
        [SerializeField] private float flapsDrag;
        [SerializeField] private float airbrakeDrag;
        [SerializeField] private float landingGearDrag;
    
        [Header("Lift")] 
        [SerializeField] private float liftPower;
        [SerializeField] private AnimationCurve liftAOACurve;
        [SerializeField] private float flapsLiftPower;
        [SerializeField] private float flapsAOABias;
        [SerializeField] private float rudderPower;
        [SerializeField] private AnimationCurve rudderAOACurve;

        [Header("InducedDrag")]
        [SerializeField] private float inducedDragPower;
        [SerializeField] private AnimationCurve inducedDragCurve;
        [SerializeField] private AnimationCurve rudderInducedDragCurve;
    
        [Header("Steering")]
        [SerializeField] private Vector3 turnSpeed;
        [SerializeField] private Vector3 turnAcceleration;
        [SerializeField] private AnimationCurve steeringCurve;
    
        [Header("G Limits")]
        [SerializeField] private float gLimitPitchUp;
        [SerializeField] private float gLimitPitchDown;
        [SerializeField] private float gLimitYawLeft;
        [SerializeField] private float gLimitYawRight;
        [SerializeField] private float gLimitRollLeft;
        [SerializeField] private float gLimitRollRight;

        private float _lastRollingDirection;
    
        private Rigidbody _rigidbody;
        private float _throttle;
        private Vector3 _velocity;
        private Vector3 _localVelocity;
        private Vector3 _localAngularVelocity;
        private Vector3 _localGForce;
        private float _angleOfAttack;
        private float _angleOfAttackYaw;
    
        private bool _airbrakeDeployed;
        private bool _flapsDeployed;
        private bool _landingGearDeployed;
    
        private Vector3 _lastVelocity;
        private Vector3 _controlInput;

    
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _throttle = 0f;
            _flapsDeployed = true;
            _landingGearDeployed = true;
        }
    
        private void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
        
            UpdateStateVariables();
            CalculateGForce(deltaTime);
        
            UpdateThrust();
            UpdateLift();
        
            UpdateSteering(deltaTime);

            UpdateDrag();
        }

        #region Physics Control Methods

        public void ThrottleUp(float deltaTime)
        {
            const float targetThrottle = 1f;
        
            var diff = targetThrottle - _throttle;
            var delta = Mathf.Clamp(diff, 0, throttleSpeedChange * deltaTime);
        
            _throttle = Mathf.Clamp(_throttle + delta, 0, targetThrottle);
        }
    
        public void ThrottleDown(float deltaTime)
        {
            const float targetThrottle = 0f;
        
            var delta = Mathf.Clamp(_throttle, 0, throttleSpeedChange * deltaTime);
        
            _throttle = Mathf.Clamp(_throttle - delta, targetThrottle, 1);
        }
    
        public void SetTurn(Vector3 controlVector)
        {
            _controlInput = controlVector;
        }

        public void SetBreak(bool isDeployed)
        {
            _airbrakeDeployed = isDeployed;
        }
    
        public void ToggleFlaps()
        {
            _flapsDeployed = !_flapsDeployed;
        }
    
        public void ToggleLandingGear()
        {
            _landingGearDeployed = !_landingGearDeployed;
        }

        #endregion
    
        private void UpdateStateVariables()
        {
            var invRotation = Quaternion.Inverse(_rigidbody.rotation);

            _velocity = _rigidbody.velocity;
            _localVelocity = invRotation * _velocity;
            _localAngularVelocity = invRotation * _rigidbody.angularVelocity;

            CalculateAngleOfAttack();
        }

        private void CalculateAngleOfAttack()
        {
            if (_localVelocity.sqrMagnitude < 0.1f) {
                _angleOfAttack = 0;
                _angleOfAttackYaw = 0;
                return;
            }
            
            _angleOfAttack = Mathf.Atan2(-_localVelocity.y, _localVelocity.z);
            _angleOfAttackYaw = Mathf.Atan2(_localVelocity.x, _localVelocity.z);
        }

        private void CalculateGForce(float deltaTime)
        {
            var invRotation = Quaternion.Inverse(_rigidbody.rotation);
            var acceleration = (_velocity - _lastVelocity) * deltaTime;
            _localGForce = invRotation * acceleration;
            _lastVelocity = _velocity;
        }
    
        private void UpdateThrust()
        {
            _rigidbody.AddRelativeForce(_throttle * maxThrust * Vector3.forward);
        }
    
        private void UpdateLift()
        {
            if (_localVelocity.sqrMagnitude < 1f) return;
            
            var curFlapsLiftPower = _flapsDeployed ? flapsLiftPower : 0;
            var curFlapsAOABias = _flapsDeployed ? flapsAOABias : 0;
        
            var liftForce = CalculateLift(
                _angleOfAttack + (curFlapsAOABias * Mathf.Deg2Rad), Vector3.right,
                liftPower + curFlapsLiftPower,
                liftAOACurve,
                inducedDragCurve
            );
        
            var yawForce = CalculateLift(_angleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);
        
            _rigidbody.AddRelativeForce(liftForce);
            _rigidbody.AddRelativeForce(yawForce);
        }
    
        private Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve,
            AnimationCurve inducedDragCurve)
        {
            var liftVelocity = Vector3.ProjectOnPlane(_localVelocity, rightAxis);
            var liftVelocity2 = liftVelocity.sqrMagnitude;

            var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
            var liftForce = liftVelocity2 * liftCoefficient * liftPower;

            var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
            var lift = liftDirection * liftForce;

            var dragForce = liftCoefficient * liftCoefficient;
            var dragDirection = -liftVelocity.normalized;
            var curInducedDrag = dragDirection * (liftVelocity2 * dragForce * inducedDragPower * inducedDragCurve.Evaluate(Mathf.Max(0, _localVelocity.z)));

            return lift + curInducedDrag;
        }
    
        private void UpdateSteering(float deltaTime)
        {
            var speed = Mathf.Max(0, _localVelocity.z);
            var steeringPower = steeringCurve.Evaluate(speed);

            var gForceScaling = CalculateGLimiter(_controlInput, turnSpeed * (Mathf.Deg2Rad * steeringPower));
        
            var targetAngularVel = Vector3.Scale(_controlInput, turnSpeed * (steeringPower * gForceScaling));
            var angularVel = _localAngularVelocity * Mathf.Rad2Deg;

            var correction = new Vector3(
                CalculateSteering(deltaTime, angularVel.x, targetAngularVel.x, turnAcceleration.x * steeringPower),
                CalculateSteering(deltaTime, angularVel.y, targetAngularVel.y, turnAcceleration.y * steeringPower),
                CalculateSteering(deltaTime, angularVel.z, targetAngularVel.z, turnAcceleration.z * steeringPower)
            );
            
            _rigidbody.AddRelativeTorque(correction * Mathf.Deg2Rad, ForceMode.VelocityChange);
        }
        
        private float CalculateSteering(float deltaTime, float angularVelocity, float targetVelocity, float acceleration) {
            var error = targetVelocity - angularVelocity;
            var accel = acceleration * deltaTime;
            return Mathf.Clamp(error, -accel, accel);
        }

        private float CalculateGLimiter(Vector3 controlInput, Vector3 maxAngularVelocity) {
            var maxInput = controlInput.normalized;
            var limit = CalculateGForceLimit(maxInput);
            var maxGForce = EstimateGForce(Vector3.Scale(maxInput, maxAngularVelocity), _localVelocity);
            if (maxGForce.magnitude > limit.magnitude) {
                return limit.magnitude / maxGForce.magnitude;
            }
            return 1;
        }
    
        private Vector3 CalculateGForceLimit(Vector3 input) {
            return Utils.CustomVectorScale(input,
                gLimitPitchDown, gLimitPitchUp, // Because input is inverted for pitching!
                gLimitYawRight, gLimitYawLeft,
                gLimitRollRight, gLimitRollLeft
            ) * Mathf.PI;
        }

        private Vector3 EstimateGForce(Vector3 angularVelocity, Vector3 velocity)
        {
            return Vector3.Cross(angularVelocity, velocity);
        }

        private void UpdateDrag()
        {
            var localVel = _localVelocity;
            var localVel2 = localVel.sqrMagnitude;

            var curAirbrakeDrag = _airbrakeDeployed ? airbrakeDrag : 0;
            var curFlapsDrag = _flapsDeployed ? flapsDrag : 0;
            var curLandingGearDrag = _landingGearDeployed ? landingGearDrag : 0;

            var coefficient = Utils.CustomVectorScale(localVel.normalized,
                dragRight.Evaluate(Mathf.Abs(localVel.x)), Mathf.Abs(dragLeft.Evaluate(localVel.x)),
                dragTop.Evaluate(Mathf.Abs(localVel.y)), dragBottom.Evaluate(Mathf.Abs(localVel.y)),
                dragForward.Evaluate(Mathf.Abs(localVel.z)) + curAirbrakeDrag + curFlapsDrag + curLandingGearDrag,
                dragBackwards.Evaluate(Mathf.Abs(localVel.z)));

            var drag = coefficient.magnitude * localVel2 * -localVel.normalized;

            _rigidbody.AddRelativeForce(drag);
        }

    
    }
}