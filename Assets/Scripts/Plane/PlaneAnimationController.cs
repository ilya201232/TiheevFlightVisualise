using System;
using System.Collections;
using ScriptableObjects;
using UnityEngine;

namespace Plane
{
    public class PlaneAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerActionData playerActionData;
        
        private Animator _animator;

        #region Animator Parameters Hashes

        private int _isGearDownBoolHash;
        private int _toggleLandingGearTriggerHash;
        
        private int _isFlapsDownBoolHash;
        private int _toggleFlapsTriggerHash;
        
        private int _isAirBreaksOutBoolHash;

        private int _aileronStateIntHash;
        private int _elevatorStateIntHash;
        private int _rudderStateIntHash;

        #endregion
        

        private bool _finishedAction;
        
        private bool _isGearDown;
        private bool _canToggleLandingGear;
        
        private bool _isFlapsDown;
        private bool _canToggleFlaps;
        
        private bool _isAirBreaksOut;

        private bool _isPitchingUp;
        private bool _isPitchingDown;
        private bool _isRollingLeft;
        private bool _isRollingRight;
        private bool _isYawingLeft;
        private bool _isYawingRight;


        private void Start()
        {
            _animator = GetComponent<Animator>();
            
            _isGearDownBoolHash = Animator.StringToHash("IsGearDown");
            _toggleLandingGearTriggerHash = Animator.StringToHash("ToggleLandingGear");
            
            _isFlapsDownBoolHash = Animator.StringToHash("IsFlapsDown");
            _toggleFlapsTriggerHash = Animator.StringToHash("ToggleFlaps");
            
            _isAirBreaksOutBoolHash = Animator.StringToHash("IsAirBreaksOut");

            _aileronStateIntHash = Animator.StringToHash("AileronState");
            _elevatorStateIntHash = Animator.StringToHash("ElevatorState");
            _rudderStateIntHash = Animator.StringToHash("RudderState");

            _finishedAction = false;
                
            _isGearDown = _animator.GetBool(_isGearDownBoolHash);
            _canToggleLandingGear = true;
            
            _isFlapsDown = _animator.GetBool(_isFlapsDownBoolHash);
            _canToggleFlaps = true;
            
            _isAirBreaksOut = _animator.GetBool(_isAirBreaksOutBoolHash);
            
        }

        private void Update()
        {
            switch (playerActionData.State)
            {
                case PlaneState.None:
                    _finishedAction = false;
                    
                    if (_isAirBreaksOut)
                    {
                        _isAirBreaksOut = false;
                        _animator.SetBool(_isAirBreaksOutBoolHash, _isAirBreaksOut);
                        StartCoroutine(SetReversedClip(1, "AirBreaksBack"));
                    }

                    if (_isPitchingUp)
                    {
                        _isPitchingUp = false;
                        _animator.SetInteger(_elevatorStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "ElevatorPitchUpToZero"));
                    } else if (_isPitchingDown)
                    {
                        _isPitchingDown = false;
                        _animator.SetInteger(_elevatorStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "ElevatorPitchDownToZero"));
                    } else if (_isRollingLeft)
                    {
                        _isRollingLeft = false;
                        _animator.SetInteger(_aileronStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "AileronsLeftTurnToZero"));
                    } else if (_isRollingRight)
                    {
                        _isRollingRight = false;
                        _animator.SetInteger(_aileronStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "AileronsRightTurnToZero"));
                    } else if (_isYawingLeft)
                    {
                        _isYawingLeft = false;
                        _animator.SetInteger(_rudderStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "RudderYawLeftToZero"));
                    } else if (_isYawingRight)
                    {
                        _isYawingRight = false;
                        _animator.SetInteger(_rudderStateIntHash, 0);
                        StartCoroutine(SetReversedClip(0, "RudderYawRightToZero"));
                    }
                    
                    break;
                case PlaneState.ToggleFlaps:
                    if (!_canToggleFlaps) return;

                    if (!_finishedAction)
                    {
                        _animator.SetTrigger(_toggleFlapsTriggerHash);
                        _canToggleFlaps = false;
                        
                        _finishedAction = true;
                    } 
                    
                    break;
                case PlaneState.ToggleLandingGear:
                    if (!_canToggleLandingGear) return;
                    
                    if (!_finishedAction)
                    {
                        _animator.SetTrigger(_toggleLandingGearTriggerHash);
                        
                        _finishedAction = true;
                    } 
                    
                    break;
                case PlaneState.ThrottleUp:
                    break;
                case PlaneState.ThrottleDown:
                    break;
                case PlaneState.Break:
                    if (!_isAirBreaksOut)
                    {
                        _isAirBreaksOut = true;
                        _animator.SetBool(_isAirBreaksOutBoolHash, _isAirBreaksOut);
                    }
                    break;
                case PlaneState.PitchUp:
                    if (!_isPitchingUp)
                    {
                        _isPitchingUp = true;
                        _animator.SetInteger(_elevatorStateIntHash, 1);
                    }
                    break;
                case PlaneState.PitchDown:
                    if (!_isPitchingDown)
                    {
                        _isPitchingDown = true;
                        _animator.SetInteger(_elevatorStateIntHash, -1);
                    }
                    break;
                case PlaneState.RollLeft:
                    if (!_isRollingLeft)
                    {
                        _isRollingLeft = true;
                        _animator.SetInteger(_aileronStateIntHash, -1);
                    }
                    break;
                case PlaneState.RollRight:
                    if (!_isRollingRight)
                    {
                        _isRollingRight = true;
                        _animator.SetInteger(_aileronStateIntHash, 1);
                    }
                    break;
                case PlaneState.YawLeft:
                    if (!_isYawingLeft)
                    {
                        _isYawingLeft = true;
                        _animator.SetInteger(_rudderStateIntHash, -1);
                    }
                    break;
                case PlaneState.YawRight:
                    if (!_isYawingRight)
                    {
                        _isYawingRight = true;
                        _animator.SetInteger(_rudderStateIntHash, 1);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnLandingGearToggleFinished()
        {
            if (!_isGearDown)
            {
                _canToggleLandingGear = true;
                
                _isGearDown = !_isGearDown;
                _animator.SetBool(_isGearDownBoolHash, _isGearDown);
            }
            else
            {
                _canToggleLandingGear = false;
            }
        }
        
        public void OnLandingGearToggleStarted()
        {
            if (!_isGearDown)
            {
                _canToggleLandingGear = false;
            }
            else
            {
                _canToggleLandingGear = true;
                
                _isGearDown = !_isGearDown;
                _animator.SetBool(_isGearDownBoolHash, _isGearDown);
            }
        }
        
        public void OnFlapsToggleFinished()
        {
            _canToggleFlaps = true;
            _isFlapsDown = !_isFlapsDown;
            _animator.SetBool(_isFlapsDownBoolHash, _isFlapsDown);
        }

        private IEnumerator SetReversedClip(int layerIndex, string reversedClipName)
        {
            
            var targetName = _animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;

            while (targetName.Equals(reversedClipName))
            {
                yield return null;
                targetName = _animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.name;
            }
            
            var targetNormalizedTime = 1 - Mathf.Clamp01(_animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime);

            _animator.Play(reversedClipName, layerIndex, targetNormalizedTime); 
            yield return null;
            
        }
    }
}