using Plane;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class PlayerActionData : ScriptableObject, ISerializationCallbackReceiver
    {
        public PlaneState State { get; private set; }

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            ClearAction();
        }

        private void ClearAction()
        {
            State = PlaneState.None;
        }
        
        public void SetToggleFlapsAction()
        {
            State = PlaneState.ToggleFlaps;
        }
        
        public void SetToggleLandingGearAction()
        {
            State = PlaneState.ToggleLandingGear;
        }
        
        public void SetThrottleUpAction()
        {
            State = PlaneState.ThrottleUp;
        }
        
        public void SetThrottleDownAction()
        {
            State = PlaneState.ThrottleDown;
        }
        
        public void SetBreakAction()
        {
            State = PlaneState.Break;
        }
        
        public void SetPitchUpAction()
        {
            State = PlaneState.PitchUp;
        }
        
        public void SetPitchDownAction()
        {
            State = PlaneState.PitchDown;
        }
        
        public void SetRollLeftAction()
        {
            State = PlaneState.RollLeft;
        }
        
        public void SetRollRightAction()
        {
            State = PlaneState.RollRight;
        }
        
        public void SetYawLeftAction()
        {
            State = PlaneState.YawLeft;
        }
        
        public void SetYawRightAction()
        {
            State = PlaneState.YawRight;
        }
    }
}