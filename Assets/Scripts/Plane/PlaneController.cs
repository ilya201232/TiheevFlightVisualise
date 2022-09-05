using System;
using ScriptableObjects;
using UnityEngine;

namespace Plane
{
    public class PlaneController : MonoBehaviour
    {
        [SerializeField] private float throttleChangeSpeed;
        [SerializeField] private PlayerActionData playerActionData;
    
        private PlanePhysicsController _physicsController;

        private bool _lastIsToggleFlaps;
        private bool _lastIsToggleLandingGear;
    
        private void Start()
        {
            _lastIsToggleFlaps = false;
            _lastIsToggleLandingGear = false;
            _physicsController = GetComponent<PlanePhysicsController>();
        }

        private void Update()
        {
            switch (playerActionData.State)
            {
                case PlaneState.None:
                    ClearStates();
                    _lastIsToggleFlaps = false;
                    _lastIsToggleLandingGear = false;
                    break;
                case PlaneState.ToggleFlaps:
                    if (!_lastIsToggleFlaps) ToggleFlaps();
                    _lastIsToggleFlaps = true;
                    break;
                case PlaneState.ToggleLandingGear:
                    if (!_lastIsToggleLandingGear) ToggleLandingGear();
                    _lastIsToggleLandingGear = true;
                    break;
                case PlaneState.ThrottleUp:
                    ThrottleUp();
                    break;
                case PlaneState.ThrottleDown:
                    ThrottleDown();
                    break;
                case PlaneState.Break:
                    Break();
                    break;
                case PlaneState.PitchUp:
                    PitchUp();
                    break;
                case PlaneState.PitchDown:
                    PitchDown();
                    break;
                case PlaneState.RollLeft:
                    RollLeft();
                    break;
                case PlaneState.RollRight:
                    RollRight();
                    break;
                case PlaneState.YawLeft:
                    YawLeft();
                    break;
                case PlaneState.YawRight:
                    YawRight();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ClearStates()
        {
            _physicsController.SetTurn(new Vector3(0, 0, 0));
            _physicsController.SetBreak(false);
        }

        private void ThrottleUp()
        {
            _physicsController.ThrottleUp(Time.deltaTime);
        }
    
        private void ThrottleDown()
        {
            _physicsController.ThrottleDown(Time.deltaTime);
        }
    
        private void Break()
        {
            _physicsController.SetBreak(true);
        }
    
        private void ToggleFlaps()
        {
            _physicsController.ToggleFlaps();
        }
    
        private void ToggleLandingGear()
        {
            _physicsController.ToggleLandingGear();
        }
    
        private void PitchUp()
        {
            _physicsController.SetTurn(new Vector3(1, 0, 0));
        }
    
        private void PitchDown()
        {
            _physicsController.SetTurn(new Vector3(-1, 0, 0));
        }
    
        private void YawLeft()
        {
            _physicsController.SetTurn(new Vector3(0, -1, 0));
        }
    
        private void YawRight()
        {
            _physicsController.SetTurn(new Vector3(0, 1, 0));
        }
    
        private void RollLeft()
        {
            _physicsController.SetTurn(new Vector3(0, 0, -1));
        }
    
        private void RollRight()
        {
            _physicsController.SetTurn(new Vector3(0, 0, 1));
        }
    
    }
}