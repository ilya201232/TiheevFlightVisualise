using System;
using Cinemachine;
using UnityEngine;

namespace Camera
{
    public class POVCameraController : RotationAroundPointBaseClass
    {
        [SerializeField]
        private Transform followObjectTransform;
        
        private CinemachineVirtualCamera _virtualCamera;

        public override void Start()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            base.Start();
        }
        
        protected override Vector3 GetCurrentPosition()
        {
            return followObjectTransform.localPosition - _virtualCamera.transform.localPosition;
        }

        protected override void SetPosition(Vector3 position)
        {
            followObjectTransform.localPosition = _virtualCamera.transform.localPosition + position;
        }
        
        protected override bool IsActive()
        {
            return CinemachineCore.Instance.IsLive(_virtualCamera);
        }
    }
}